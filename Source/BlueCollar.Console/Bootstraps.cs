//-----------------------------------------------------------------------
// <copyright file="Bootstraps.cs" company="Tasty Codes">
//     Copyright (c) 2012 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Console
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Remoting;
    using System.Security;
    using System.Security.Permissions;
    
    /// <summary>
    /// Provides bootstrapping services for creating a <see cref="Machine"/> in a secondary app domain.
    /// </summary>
    [SecurityCritical]
    [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
    public sealed class Bootstraps : IDisposable
    {
        private AppDomain domain;
        private EventLogger logger;
        private MachineProxy machineProxy;
        private List<BlueCollar.FileSystemWatcher> watchers;
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the Bootstraps class.
        /// </summary>
        /// <param name="applicationPath">The application path.</param>
        /// <param name="configPath">The configuration file path, if applicable.</param>
        /// <param name="threshold">The threshold, in milliseconds, to compress filesystem events into.</param>
        public Bootstraps(string applicationPath, string configPath, int threshold)
        {
            if (string.IsNullOrEmpty(applicationPath))
            {
                throw new ArgumentNullException("applicationPath", "applicationPath must contain a value.");
            }

            if (threshold < 500)
            {
                throw new ArgumentOutOfRangeException("threshold", "threshold must be greater than or equal to 500.");
            }

            if (!Path.IsPathRooted(applicationPath))
            {
                applicationPath = Path.GetFullPath(applicationPath);
            }

            this.ApplicationPath = applicationPath;
            this.ConfigPath = configPath ?? string.Empty;
            this.Threshold = threshold;
            this.watchers = new List<BlueCollar.FileSystemWatcher>();
        }

        /// <summary>
        /// Finalizes an instance of the Bootstraps class.
        /// </summary>
        ~Bootstraps()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Event raised when one or more application files changed, indicating
        /// that an app domain reload should be performed.
        /// </summary>
        public event EventHandler<FileSystemEventArgs> ApplicationFilesChanged;

        /// <summary>
        /// Event raised when the target application logs a message.
        /// </summary>
        public event EventHandler<EventLoggerEventArgs> Log;

        /// <summary>
        /// Gets the application path.
        /// </summary>
        public string ApplicationPath { get; private set; }

        /// <summary>
        /// Gets the configuration file path, if applicable.
        /// </summary>
        public string ConfigPath { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance's app domain is loaded.
        /// </summary>
        public bool IsLoaded { get; private set; }

        /// <summary>
        /// Gets the threshold, in milliseconds, to compress filesystem events into.
        /// </summary>
        public int Threshold { get; private set; }

        /// <summary>
        /// Disposes of resources used by this instance.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Pulls up this instance's app domain, if it is not already loaded.
        /// </summary>
        /// <returns>The result of the pull up operation.</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Passing all exceptions to the caller.")]
        public BootstrapsPullupResult PullUp()
        {
            BootstrapsPullupResult result = null;

            if (!this.IsLoaded)
            {
                try
                {
                    // Ensure cleanup if we got partway through this before.
                    this.Pushdown(true);

                    if (Directory.Exists(this.ApplicationPath))
                    {
                        string configPath = this.ConfigPath;

                        // Default to Web.config if not specified and a Web.config file exists.
                        if (string.IsNullOrEmpty(configPath))
                        {
                            string webConfigPath = Path.Combine(this.ApplicationPath, "Web.config");

                            if (File.Exists(webConfigPath))
                            {
                                configPath = webConfigPath;
                            }
                        }

                        // Root the config path relative to the application path.
                        if (!string.IsNullOrEmpty(configPath) && !Path.IsPathRooted(configPath))
                        {
                            configPath = Path.Combine(this.ApplicationPath, configPath);
                        }

                        if (string.IsNullOrEmpty(configPath) || File.Exists(configPath))
                        {
                            // Use the bin directory if the target is a web application.
                            string binPath = Path.Combine(this.ApplicationPath, "bin");
                            bool web = Directory.Exists(binPath)
                                && (string.IsNullOrEmpty(configPath)
                                || (!string.IsNullOrEmpty(configPath)
                                && "Web.config".Equals(Path.GetFileName(configPath), StringComparison.OrdinalIgnoreCase)));

                            AppDomainSetup setup = new AppDomainSetup();
                            setup.ApplicationBase = this.ApplicationPath;
                            setup.ShadowCopyDirectories = this.ApplicationPath;
                            setup.ShadowCopyFiles = "true";

                            if (!string.IsNullOrEmpty(configPath))
                            {
                                setup.ConfigurationFile = configPath;
                            }

                            if (web)
                            {
                                setup.PrivateBinPath = binPath;
                            }

                            this.domain = AppDomain.CreateDomain("Blue Collar Machine", AppDomain.CurrentDomain.Evidence, setup);

                            this.logger = new EventLogger();
                            this.logger.Log += new EventHandler<EventLoggerEventArgs>(this.LoggerLog);

#if NET35
                        this.machineProxy = (MachineProxy)this.domain.CreateInstanceAndUnwrap(
                            typeof(MachineProxy).Assembly.FullName,
                            typeof(MachineProxy).FullName,
                            false,
                            BindingFlags.Default,
                            null,
                            new object[] { this.logger },
                            null,
                            null,
                            AppDomain.CurrentDomain.Evidence);
#else
                            this.machineProxy = (MachineProxy)this.domain.CreateInstanceAndUnwrap(
                                typeof(MachineProxy).Assembly.FullName,
                                typeof(MachineProxy).FullName,
                                false,
                                BindingFlags.Default,
                                null,
                                new object[] { this.logger },
                                null,
                                null);
#endif
                            
                            // Create the watchers based on application type.
                            if (web)
                            {
                                this.watchers.Add(this.CreateWatcher(binPath, FileSystemWatcherMode.Directory, "*.dll"));

                                if (!string.IsNullOrEmpty(configPath))
                                {
                                    this.watchers.Add(this.CreateWatcher(Path.GetDirectoryName(configPath), FileSystemWatcherMode.IndividualFiles, Path.GetFileName(configPath)));
                                }

                                string appCodePath = Path.Combine(this.ApplicationPath, "App_Code");

                                if (Directory.Exists(appCodePath))
                                {
                                    this.watchers.Add(this.CreateWatcher(appCodePath, FileSystemWatcherMode.Directory, "*.*"));
                                }
                            }
                            else
                            {
                                this.watchers.Add(this.CreateWatcher(this.ApplicationPath, FileSystemWatcherMode.Directory, "*.dll"));
                            }

                            this.IsLoaded = true;
                        }
                        else
                        {
                            result = new BootstrapsPullupResult(BootstrapsPullupResultType.ConfigurationFileNotFound);
                        }
                    }
                    else
                    {
                        result = new BootstrapsPullupResult(BootstrapsPullupResultType.ApplicationDirectoryNotFound);
                    }
                }
                catch (Exception ex)
                {
                    this.Pushdown(true);
                    result = new BootstrapsPullupResult(BootstrapsPullupResultType.Exception, ex);
                }
            }

            return result ?? new BootstrapsPullupResult(BootstrapsPullupResultType.Success);
        }

        /// <summary>
        /// Destroys this instance app domain.
        /// </summary>
        /// <param name="force">A value indicating whether to force destruction immediately,
        /// without waiting for pending jobs to complete.</param>
        public void Pushdown(bool force)
        {
            try
            {
                new Action(
                    () =>
                    {
                        this.IsLoaded = false;
                        this.DestroyWatchers();

                        if (this.machineProxy != null)
                        {
                            try
                            {
                                this.machineProxy.Dispose(force);
                            }
                            catch (RemotingException)
                            {
                            }
                            finally
                            {
                                this.machineProxy = null;
                            }
                        }

                        if (this.domain != null)
                        {
                            AppDomain.Unload(this.domain);
                            this.domain = null;
                        }
                    }).InvokeWithTimeout(30000);
            }
            catch (TimeoutException)
            {
                this.watchers = new List<BlueCollar.FileSystemWatcher>();
                this.machineProxy = null;
                this.domain = null;
            }
        }

        /// <summary>
        /// Creates a <see cref="FileSystemWatcher"/>.
        /// </summary>
        /// <param name="path">The path to watch.</param>
        /// <param name="mode">The watch mode.</param>
        /// <param name="filter">The file search filter to use.</param>
        /// <returns>The created <see cref="FileSystemWatcher"/>.</returns>
        private BlueCollar.FileSystemWatcher CreateWatcher(string path, FileSystemWatcherMode mode, string filter)
        {
            BlueCollar.FileSystemWatcher watcher = null;

            try
            {
                watcher = new BlueCollar.FileSystemWatcher(path);
                watcher.Operation += new FileSystemEventHandler(this.WatcherOperation);
                watcher.Mode = mode;
                watcher.Filter = filter;
                watcher.Threshold = this.Threshold;
                watcher.EnableRaisingEvents = true;
            }
            catch
            {
                if (watcher != null)
                {
                    watcher.Dispose();
                }

                throw;
            }

            return watcher;
        }

        /// <summary>
        /// Destroys any <see cref="FileSystemWatcher"/>s this instance is maintaining.
        /// </summary>
        private void DestroyWatchers()
        {
            foreach (BlueCollar.FileSystemWatcher watcher in this.watchers)
            {
                watcher.Dispose();
            }

            this.watchers = new List<BlueCollar.FileSystemWatcher>();
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
                    this.Pushdown(true);
                }

                this.disposed = true;
            }
        }

        /// <summary>
        /// Raises the logger's Log event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void LoggerLog(object sender, EventLoggerEventArgs e)
        {
            if (this.Log != null)
            {
                this.Log(this, e);
            }
        }
        
        /// <summary>
        /// Raises the watcher's Operation event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void WatcherOperation(object sender, FileSystemEventArgs e)
        {
            if (this.ApplicationFilesChanged != null)
            {
                this.ApplicationFilesChanged(this, e);
            }
        }
    }
}
