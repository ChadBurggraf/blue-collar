//-----------------------------------------------------------------------
// <copyright file="Machine.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Text.RegularExpressions;
    using System.Threading;

    /// <summary>
    /// Provides machine coordination services, managing all workers
    /// on a machine for an application.
    /// </summary>
    public sealed class Machine : IDisposable
    {
        #region Private Fields

        private static readonly string MachineAddress = GetMachineAddress();
        private static readonly string MachineName = Environment.MachineName;
        private List<Worker> workers = new List<Worker>();
        private Thread runThread;
        private ILogger logger;
        private IRepositoryFactory repositoryFactory;
        private string address, applicationName, name;
        private int heartbeat, workerHeartbeat;
        private bool disposed, ensureDefaultWorker, schedulerEnabled;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the Machine class.
        /// </summary>
        public Machine()
            : this(new NullLogger())
        {
        }

        /// <summary>
        /// Initializes a new instance of the Machine class.
        /// </summary>
        /// <param name="logger">The logger to use when logging messages.</param>
        public Machine(ILogger logger) : this(
            logger,
            new ConfigurationRepositoryFactory(),
            BlueCollarSection.Section.ApplicationName, 
            Machine.Address, 
            Machine.Name, 
            BlueCollarSection.Section.Machine.Heartbeat, 
            BlueCollarSection.Section.WorkerHeartbeat,
            BlueCollarSection.Section.SchedulerEnabled,
            BlueCollarSection.Section.Machine.EnsureDefaultWorker)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Machine class.
        /// </summary>
        /// <param name="logger">The logger to use when logging messages.</param>
        /// <param name="repositoryFactory">The repository factory to use when creating repositories for data access.</param>
        /// <param name="applicationName">The name of the application to manage workers for.</param>
        /// <param name="address">The address of the machine.</param>
        /// <param name="name">The name of the machine.</param>
        /// <param name="heartbeat">The heartbeat, in seconds, to use when polling for additions or removal of workers.</param>
        /// <param name="workerHeartbeat">The heartbeat, in seconds, the workers should use when polling for work.</param>
        /// <param name="schedulerEnabled">A value indicating whether the job scheduler is enabled.</param>
        /// <param name="ensureDefaultWorker">A value indicating whether to ensure at least one worker exists, creating it if necessary.</param>
        public Machine(ILogger logger, IRepositoryFactory repositoryFactory, string applicationName, string address, string name, int heartbeat, int workerHeartbeat, bool schedulerEnabled, bool ensureDefaultWorker)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger", "logger cannot be null.");
            }

            if (repositoryFactory == null)
            {
                throw new ArgumentNullException("repositoryFactory", "repositoryFactory cannot be null.");
            }

            if (string.IsNullOrEmpty(applicationName))
            {
                throw new ArgumentNullException("applicationName", "applicationName must contain a value.");
            }

            if (string.IsNullOrEmpty(address) && string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Either name or address or both must contain a value.", "address");
            }

            if (heartbeat < 1)
            {
                throw new ArgumentOutOfRangeException("heartbeat", "heartbeat must be greater than 0.");
            }

            if (workerHeartbeat < 1)
            {
                throw new ArgumentOutOfRangeException("workerHeartbeat", "workerHeartbeat must be greater than 0.");
            }

            this.logger = logger;
            this.repositoryFactory = repositoryFactory;
            this.applicationName = applicationName;
            this.address = address;
            this.name = name;
            this.heartbeat = heartbeat * 1000;
            this.workerHeartbeat = workerHeartbeat;
            this.schedulerEnabled = schedulerEnabled;
            this.ensureDefaultWorker = ensureDefaultWorker;

            this.runThread = new Thread(this.RunLoop);
            this.runThread.Start();
        
            this.logger.Info("Machine {0} ({1}) has started for application '{2}'.", this.name, this.address, this.applicationName);
            this.logger.Debug("Machine {0} ({1}) is using repository factory '{2}'.", this.name, this.address, this.repositoryFactory.GetType().ToString());
        }

        /// <summary>
        /// Finalizes an instance of the Machine class.
        /// </summary>
        ~Machine()
        {
            this.Dispose(false);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current machine's IP address.
        /// </summary>
        public static string Address
        {
            get { return MachineAddress; }
        }

        /// <summary>
        /// Gets the current machine's name.
        /// </summary>
        public static string Name
        {
            get { return MachineName; }
        }

        #endregion

        #region Public Instance Methods

        /// <summary>
        /// Disposes of resources used by this instance.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA1816:CallGCSuppressFinalizeCorrectly", Justification = "GC.SuppressFinalize(object) is called indirectly.")]
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Disposes of resources used by this instance.
        /// </summary>
        /// <param name="force">A value indicating whether to force workers to stop.</param>
        [SuppressMessage("Microsoft.Usage", "CA1816:CallGCSuppressFinalizeCorrectly", Justification = "This method is called by Dispose().")]
        public void Dispose(bool force)
        {
            this.Dispose(force, true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Internal Static Methods

        /// <summary>
        /// Gets the current machine's private IP address.
        /// </summary>
        /// <returns>The current machine's private IP address, or null if it could not be determined.</returns>
        internal static string GetPrivateMachineAddress()
        {
            IPAddress address = Dns.GetHostEntry(Dns.GetHostName())
                .AddressList
                .Where(a => a.AddressFamily == AddressFamily.InterNetwork)
                .FirstOrDefault();

            return address != null ? address.ToString() : null;
        }

        /// <summary>
        /// Gets the current machine's public IP address.
        /// </summary>
        /// <returns>The current machine's public IP address, or null if it could not be determined.</returns>
        internal static string GetPublicMachineAddress()
        {
            string content = null, result = null;
            WebRequest request = WebRequest.Create("http://checkip.dyndns.org/");

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        Stream stream = null;

                        try
                        {
                            stream = response.GetResponseStream();

                            using (StreamReader reader = new StreamReader(stream))
                            {
                                stream = null;
                                content = reader.ReadToEnd();
                            }
                        }
                        finally
                        {
                            if (stream != null)
                            {
                                stream.Dispose();
                            }
                        }
                    }
                }
            }
            catch (WebException)
            {
            }

            if (!string.IsNullOrEmpty(content))
            {
                Match match = Regex.Match(content, @"\d+\.\d+\.\d+\.\d+");

                if (match.Success)
                {
                    result = match.Value;
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the current machine's IP address.
        /// </summary>
        /// <returns>The current machine's IP address.</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We don't want exceptions of any type to bubble up from calls to this method.")]
        internal static string GetMachineAddress()
        {
            string address = null;

            try
            {
                address = GetPublicMachineAddress();
            }
            catch
            {
            }

            if (string.IsNullOrEmpty(address))
            {
                try
                {
                    address = GetPrivateMachineAddress();
                }
                catch
                {
                }
            }

            return !string.IsNullOrEmpty(address) ? address : "127.0.0.1";
        }

        #endregion

        #region Internal Instance Methods

        /// <summary>
        /// Refreshes the worker list.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We want to continue no matter the reason for Worker.Dispose() or Worker.Start() exceptions.")]
        internal void RefreshWorkers()
        {
            List<WorkerRecord> records;

            using (IRepository repository = this.repositoryFactory.Create())
            {
                records = repository.GetWorkers(this.applicationName, this.address, this.name, null).ToList();

                this.logger.Debug(
                    "Machine {0} ({1}) loaded {2} worker records from repository '{3}', using connection string '{4}'.",
                    this.name,
                    this.address,
                    records.Count,
                    repository.GetType().ToString(),
                    repository.ConnectionString);
            }

            lock (this)
            {
                List<Worker> newWorkers = new List<Worker>();
                List<Worker> removeWorkers = new List<Worker>();

                // Figure out which of the current workers get to stay alive.
                foreach (Worker worker in this.workers) 
                {
                    WorkerRecord record = records.Where(r => r.Id == worker.Id).FirstOrDefault();

                    if (record != null)
                    {
                        newWorkers.Add(worker);
                        records.Remove(record);
                    }
                    else
                    {
                        removeWorkers.Add(worker);
                    }
                }

                // Prune orphaned current workers.
                foreach (Worker worker in removeWorkers)
                {
                    try
                    {
                        worker.Dispose();
                    }
                    catch (Exception ex)
                    {
                        this.logger.Error(ex);
                    }
                }

                // Create workers for all of the new records. Records corresponding
                // to existing workers where pruned earlier.
                foreach (WorkerRecord record in records)
                {
                    Worker worker = null;

                    try
                    {
                        worker = new Worker(
                            this.applicationName,
                            record.Id.Value,
                            record.Name,
                            QueueNameFilters.Parse(record.QueueNames),
                            this.workerHeartbeat,
                            this.schedulerEnabled,
                            this.repositoryFactory,
                            this.logger);

                        newWorkers.Add(worker);

                        if (record.Status == WorkerStatus.Working || record.Startup == WorkerStartupType.Automatic)
                        {
                            worker.Start();
                        }
                    }
                    catch (Exception ex)
                    {
                        if (worker != null)
                        {
                            newWorkers.Remove(worker);
                            worker.Dispose();
                        }

                        this.logger.Error(ex);
                    }
                }

                // Ensure a default worker if necessary.
                if (newWorkers.Count == 0 && this.ensureDefaultWorker)
                {
                    Worker worker = this.CreateDefaultWorker();
                    newWorkers.Add(worker);
                    worker.Start();
                }

                this.workers = newWorkers;
                this.logger.Debug("Machine {0} ({1}) refreshed its worker list and is now tracking {2} workers.", this.name, this.address, this.workers.Count);
            }
        }

        /// <summary>
        /// Implements the machine run loop.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We want to continue the run loop no matter what unless a ThreadAbortException is thrown.")]
        internal void RunLoop()
        {
            while (true)
            {
                try
                {
                    this.RefreshWorkers();
                }
                catch (ThreadAbortException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    this.logger.Error(ex);
                }

                Thread.Sleep(this.heartbeat.Randomize());
            }
        }

        #endregion

        #region Private Instance Methods

        /// <summary>
        /// Creates a default worker.
        /// </summary>
        /// <returns>The created worker.</returns>
        private Worker CreateDefaultWorker()
        {
            WorkerRecord record = new WorkerRecord()
            {
                ApplicationName = this.applicationName,
                MachineAddress = Machine.Address,
                MachineName = Machine.Name,
                Name = "Default",
                Signal = WorkerSignal.None,
                Status = WorkerStatus.Working,
                Startup = WorkerStartupType.Automatic,
                UpdatedOn = DateTime.UtcNow
            };

            using (IRepository repository = this.repositoryFactory.Create())
            {
                repository.CreateWorker(record, null);
            }

            return new Worker(
                this.applicationName,
                record.Id.Value,
                record.Name,
                QueueNameFilters.Parse(record.QueueNames),
                this.workerHeartbeat,
                this.schedulerEnabled,
                this.repositoryFactory,
                this.logger);
        }

        /// <summary>
        /// Disposes of resources used by this instance.
        /// </summary>
        /// <param name="force">A value indicating whether to force workers to stop.</param>
        /// <param name="disposing">A value indicating whether to dispose of managed resources.</param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We don't care if Thread.Abort() fails for any reason.")]
        private void Dispose(bool force, bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    lock (this)
                    {
                        if (this.runThread != null && this.runThread.IsAlive)
                        {
                            try
                            {
                                this.runThread.Abort();
                                this.runThread = null;
                            }
                            catch (Exception ex)
                            {
                                this.logger.Error(ex);
                            }
                        }

                        if (this.workers != null)
                        {
                            foreach (Worker worker in this.workers)
                            {
                                try
                                {
                                    worker.Disposing -= this.WorkerDisposing;

                                    if (force)
                                    {
                                        worker.Dispose();
                                    }
                                    else
                                    {
                                        worker.Stop(false);
                                        worker.Dispose();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    this.logger.Error(ex);
                                }
                            }

                            this.workers = null;
                        }
                    }

                    this.logger.Debug("Machine {0} ({1}) as been disposed.", this.name, this.address);
                }

                this.disposed = true;
            }
        }

        /// <summary>
        /// Handles a worker's Disposing event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void WorkerDisposing(object sender, EventArgs e)
        {
            lock (this)
            {
                Worker worker = sender as Worker;

                if (worker != null && this.workers.Contains(worker))
                {
                    this.workers.Remove(worker);
                }
            }
        }

        #endregion
    }
}
