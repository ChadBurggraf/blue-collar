//-----------------------------------------------------------------------
// <copyright file="ApplicationCoordinator.cs" company="Tasty Codes">
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
    using System.Threading;
    using NLog;

    /// <summary>
    /// Coordinates a set of <see cref="ApplicationProcess"/>es.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", MessageId = "es", Justification = "Reviewed.")]
    public sealed class ApplicationCoordinator : IDisposable
    {
        private readonly object locker = new object();
        private List<ApplicationProcess> applications = new List<ApplicationProcess>();
        private Logger logger;
        private string exePath;
        private bool isRunning, disposed;
        
        /// <summary>
        /// Initializes a new instance of the ApplicationCoordinator class.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        public ApplicationCoordinator(Logger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger", "logger cannot be null.");
            }

            this.logger = logger;
        }

        /// <summary>
        /// Initializes a new instance of the ApplicationCoordinator class.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="exePath">The path of the Collar.exe path to use for applications.</param>
        public ApplicationCoordinator(Logger logger, string exePath)
            : this(logger)
        {
            this.exePath = exePath;
        }

        /// <summary>
        /// Finalizes an instance of the ApplicationCoordinator class.
        /// </summary>
        ~ApplicationCoordinator()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Gets the count of applications currently being coordinated.
        /// </summary>
        public int Count
        {
            get
            {
                lock (this.locker)
                {
                    return this.applications.Count;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the application manager is running.
        /// </summary>
        public bool IsRunning
        {
            get
            {
                lock (this.locker)
                {
                    return this.isRunning;
                }
            }
        }

        /// <summary>
        /// Disposes of resources used by this instance.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets a collection of application paths currently being coordinated by this instance.
        /// </summary>
        /// <returns>A collection of application paths.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Performance.")]
        public IEnumerable<string> GetCoordinatedApplicationPaths()
        {
            lock (this.locker)
            {
                return this.applications.Select(a => a.Path).ToArray();
            }
        }

        /// <summary>
        /// Starts or refreshes all applications, creating and pruning existing
        /// applications from the given element collection as necessary.
        /// </summary>
        /// <param name="elements">The application element collection to use when building the
        /// application process list.</param>
        public void StartAndRefresh(IEnumerable<ApplicationElement> elements)
        {
            if (elements == null)
            {
                throw new ArgumentNullException("elements", "elements cannot be null.");
            }

            ApplicationProcess[] apps;

            lock (this.locker)
            {
                this.isRunning = true;
                this.CreateRefreshAndPruneApplications(elements);
                apps = this.applications.ToArray();
            }

            this.logger.Info(CultureInfo.InvariantCulture, "Starting {0} applications.", apps.Length);

            foreach (ApplicationProcess application in apps)
            {
                ThreadPool.QueueUserWorkItem(this.StartApplication, application);
            }
        }

        /// <summary>
        /// Stops all applications.
        /// </summary>
        public void Stop()
        {
            this.Stop(false);
        }

        /// <summary>
        /// Raises an application's Exited event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void ApplicationExited(object sender, EventArgs e)
        {
            ApplicationProcess application = (ApplicationProcess)sender;
            this.logger.Info(CultureInfo.InvariantCulture, "Application at '{0}' has exited and will be re-started.", application.Path);
            this.StartApplication(sender);
        }

        /// <summary>
        /// Creates the <see cref="ApplicationProcess"/> list, refreshes any existing applications, 
        /// and prunes any orphaned applications from the existing list.
        /// </summary>
        /// <param name="elements">The <see cref="ApplicationElement"/> collection defining
        /// the current application list to manage.</param>
        [SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "The application object is disposed or added to a containing collection along all exception paths before losing scope.")]
        private void CreateRefreshAndPruneApplications(IEnumerable<ApplicationElement> elements)
        {
            List<ApplicationProcess> pruning = this.applications
                .Where(a => !elements.Any(e => a.Path.Equals(e.ApplicationPath, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            List<ApplicationProcess> existing = this.applications
                .Where(a => elements.Any(e => a.Path.Equals(e.ApplicationPath, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            List<ApplicationElement> creating = elements
                .Where(e => !this.applications.Any(a => e.ApplicationPath.Equals(a.Path, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            // Prune removed applications.
            foreach (ApplicationProcess application in pruning)
            {
                application.Exited -= new EventHandler(this.ApplicationExited);
                application.Stop(true);
                application.Dispose();
            }

            // Stop and re-initialize changed applications.
            foreach (ApplicationProcess application in existing)
            {
                ApplicationElement element = elements.Where(e => application.Path.Equals(e.ApplicationPath, StringComparison.OrdinalIgnoreCase)).First();

                if (!(application.ConfigPath ?? string.Empty).Equals(element.ApplicationConfigPath, StringComparison.OrdinalIgnoreCase)
                    || application.Force32Bit != element.Force32Bit
                    || application.FrameworkVersion != element.Framework)
                {
                    if (this.SetApplicationProperties(application, element))
                    {
                        application.Stop(true);
                    }
                }
            }

            // Create added applications.
            foreach (ApplicationElement element in creating)
            {
                ApplicationProcess application = null;

                try
                {
                    try
                    {
                        application = new ApplicationProcess(this.logger, element.ApplicationPath, this.exePath);
                    }
                    catch (ArgumentException ex)
                    {
                        this.logger.Error(
                            new ConfigurationErrorsException(
                                "The value for 'path' contains invalid characters.",
                                ex,
                                element.ElementInformation.Source,
                                element.ElementInformation.LineNumber));
                    }

                    if (application != null)
                    {
                        application.Exited += new EventHandler(this.ApplicationExited);

                        if (this.SetApplicationProperties(application, element))
                        {
                            existing.Add(application);
                        }
                    }
                }
                catch
                {
                    if (application != null)
                    {
                        application.Dispose();
                    }

                    throw;
                }
            }

            lock (this.locker)
            {
                this.applications = existing;
            }
        }

        /// <summary>
        /// Sets the given application's configurable properties from the given configuration element.
        /// </summary>
        /// <param name="application">The application to initialize.</param>
        /// <param name="element">The configuration element to initialize the application from.</param>
        /// <returns>True if the application was initialized successfully, false otherwise.</returns>
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "configPath", Justification = "Reviewed.")]
        private bool SetApplicationProperties(ApplicationProcess application, ApplicationElement element)
        {
            bool success = true;
            application.Force32Bit = element.Force32Bit;

            try
            {
                application.ConfigPath = element.ApplicationConfigPath;
            }
            catch (ArgumentException ex)
            {
                success = false;
                this.logger.Error(
                    new ConfigurationErrorsException(
                        "The value for 'configPath' contains invalid characters.",
                        ex,
                        element.ElementInformation.Source,
                        element.ElementInformation.LineNumber));
            }

            try
            {
                application.FrameworkVersion = element.Framework;
            }
            catch (ArgumentException ex)
            {
                success = false;
                this.logger.Error(
                    new ConfigurationErrorsException(
                        "The value for 'framework' must be one of: '3.5', '4.0'.",
                        ex,
                        element.ElementInformation.Source,
                        element.ElementInformation.LineNumber));
            }

            return success;
        }

        /// <summary>
        /// Starts an application.
        /// </summary>
        /// <param name="context">The application to start.</param>
        private void StartApplication(object context)
        {
            ApplicationProcess application = (ApplicationProcess)context;
            bool retry = true;

            while (retry)
            {
                lock (this.locker)
                {
                    if (this.IsRunning && this.applications.IndexOf(application) >= 0 && !application.IsRunning)
                    {
                        retry = !application.Start();
                    }
                    else
                    {
                        retry = false;
                    }
                }
            }
        }

        /// <summary>
        /// Stops all applications.
        /// </summary>
        /// <param name="disposing">A value indicating whether to dispose of the stopped applications.</param>
        private void Stop(bool disposing)
        {
            lock (this.locker)
            {
                if (!this.IsRunning || disposing)
                {
                    this.isRunning = false;
                    this.logger.Info(CultureInfo.InvariantCulture, "Stopping {0} applications.", this.applications.Count);

                    foreach (ApplicationProcess application in this.applications)
                    {
                        application.Exited -= new EventHandler(this.ApplicationExited);
                        application.Stop(true);

                        if (disposing)
                        {
                            application.Dispose();
                        }
                    }
                }

                if (disposing)
                {
                    this.applications.Clear();
                }
            }
        }

        /// <summary>
        /// Disposes of resources used by this instance.
        /// </summary>
        /// <param name="disposing">A value indicating whether to dispose of managed resources.</param>
        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.Stop(true);
                }

                this.disposed = true;
            }
        }
    }
}
