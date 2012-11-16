//-----------------------------------------------------------------------
// <copyright file="Service.cs" company="Tasty Codes">
//     Copyright (c) 2012 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Service
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Security;
    using System.Security.Permissions;
    using System.ServiceProcess;
    using System.Threading;
    using NLog;

    /// <summary>
    /// <see cref="ServiceBase"/> implementation of the Blue Collar service.
    /// </summary>
    [SecurityCritical]
    [SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "Reviewed.")]
    public partial class Service : ServiceBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly object locker = new object();
        private ApplicationCoordinator coordinator = new ApplicationCoordinator(Logger);
        private BlueCollar.FileSystemWatcher watcher;
        private bool isRunning, disposed;

        /// <summary>
        /// Initializes a new instance of the Service class.
        /// </summary>
        public Service()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Disposes of resources used by this instance.
        /// </summary>
        /// <param name="disposing">A value indicating whether to dispose of managed resources.</param>
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        protected override void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.DestroyWatcher();

                    if (this.components != null)
                    {
                        this.components.Dispose();
                        this.components = null;
                    }

                    if (this.coordinator != null)
                    {
                        this.coordinator.Dispose();
                        this.coordinator = null;
                    }
                }

                this.disposed = true;
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Runs when a Continue command is sent to the service by the Service Control Manager (SCM). 
        /// Specifies actions to take when a service resumes normal functioning after being paused.
        /// </summary>
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        protected override void OnContinue()
        {
            IEnumerable<ApplicationElement> applications = GetApplicationElements();

            lock (this.locker)
            {
                this.isRunning = true;
                this.coordinator.StartAndRefresh(applications);
                this.CreateWatcher();
            }

            base.OnContinue();
        }

        /// <summary>
        /// Executes when a Pause command is sent to the service by the Service Control Manager (SCM). 
        /// Specifies actions to take when a service pauses.
        /// </summary>
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        protected override void OnPause()
        {
            lock (this.locker)
            {
                this.isRunning = false;
                this.DestroyWatcher();
                this.coordinator.Stop();
            }

            base.OnPause();
        }

        /// <summary>
        /// Executes when a Start command is sent to the service by the Service Control Manager (SCM) 
        /// or when the operating system starts (for a service that starts automatically). 
        /// Specifies actions to take when the service starts.
        /// </summary>
        /// <param name="args">Data passed by the start command. </param>
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        protected override void OnStart(string[] args)
        {
            IEnumerable<ApplicationElement> applications = GetApplicationElements();

            lock (this.locker)
            {
                this.isRunning = true;
                this.coordinator.StartAndRefresh(applications);
                this.CreateWatcher();
            }
        }

        /// <summary>
        /// Executes when a Stop command is sent to the service by the Service Control Manager (SCM). 
        /// Specifies actions to take when a service stops running.
        /// </summary>
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        protected override void OnStop()
        {
            lock (this.locker)
            {
                this.isRunning = false;
                this.DestroyWatcher();
                this.coordinator.Stop();
            }
        }

        /// <summary>
        /// Gets a collection of application elements from the configuration.
        /// </summary>
        /// <returns>A collection of application elements.</returns>
        private static IEnumerable<ApplicationElement> GetApplicationElements()
        {
            IEnumerable<ApplicationElement> applications;

            try
            {
                BlueCollarServiceSection.Refresh();
                applications = BlueCollarServiceSection.Section.Applications;
            }
            catch (ConfigurationErrorsException ex)
            {
                Logger.ErrorException("The configuration file is invalid. Please edit the configuration and re-save the file in order to load updated configuration information.", ex);
                applications = new ApplicationElement[0];
            }

            return applications;
        }

        /// <summary>
        /// Creates the <see cref="Diagnostics.FileSystemWatcher"/> to watch for configuration changes.
        /// </summary>
        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        private void CreateWatcher()
        {
            this.DestroyWatcher();

            this.watcher = new BlueCollar.FileSystemWatcher(Path.GetDirectoryName(BlueCollarServiceSection.Section.ElementInformation.Source), Path.GetFileName(BlueCollarServiceSection.Section.ElementInformation.Source));
            this.watcher.Operation += new FileSystemEventHandler(this.WatcherOperation);
            this.watcher.Mode = FileSystemWatcherMode.IndividualFiles;
            this.watcher.Threshold = 1000;
            this.watcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// Destroys the <see cref="Diagnostics.FileSystemWatcher"/>.
        /// </summary>
        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        private void DestroyWatcher()
        {
            if (this.watcher != null)
            {
                this.watcher.Dispose();
                this.watcher = null;
            }
        }

        /// <summary>
        /// Raises the <see cref="Diagnostics.FileSystemWatcher"/>'s Operation event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void WatcherOperation(object sender, FileSystemEventArgs e)
        {
            lock (this.locker)
            {
                if (this.isRunning)
                {
                    Logger.Info(CultureInfo.InvariantCulture, "A change was detected in the configuration file at '{0}'. Refreshing the application list.", BlueCollarServiceSection.Section.ElementInformation.Source);
                    this.coordinator.StartAndRefresh(GetApplicationElements());
                }
            }
        }
    }
}
