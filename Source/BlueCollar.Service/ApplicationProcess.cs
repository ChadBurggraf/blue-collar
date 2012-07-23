//-----------------------------------------------------------------------
// <copyright file="ApplicationProcess.cs" company="Tasty Codes">
//     Copyright (c) 2012 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Service
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Text.RegularExpressions;
    using BlueCollar;
    using NLog;

    /// <summary>
    /// Represents an application and its associated process.
    /// </summary>
    public sealed class ApplicationProcess : IDisposable
    {
        private static readonly Regex FrameworkExp = new Regex(@"^3\.5|4\.0$", RegexOptions.Compiled);
        private readonly object locker = new object();
        private Logger logger;
        private string basePath, configPath, exePath, frameworkVersion, path;
        private bool force32Bit, isRunning, disposed;
        private int? threshold;
        private ProcessStartInfo startInfo;
        private Process process;

        /// <summary>
        /// Initializes a new instance of the ApplicationProcess class.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="path">The path of the application.</param>
        public ApplicationProcess(Logger logger, string path)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger", "logger cannot be null.");
            }

            this.logger = logger;
            this.Path = path;
        }

        /// <summary>
        /// Initializes a new instance of the ApplicationProcess class.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="path">The path of the application.</param>
        /// <param name="exePath">The path of the Collar.exe executable to use.</param>
        public ApplicationProcess(Logger logger, string path, string exePath)
            : this(logger, path)
        {
            this.exePath = exePath;
        }

        /// <summary>
        /// Finalizes an instance of the ApplicationProcess class.
        /// </summary>
        ~ApplicationProcess()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Event raised when this instance's process as exited.
        /// </summary>
        public event EventHandler Exited;

        /// <summary>
        /// Event raised when this instance is asked to kill its process,
        /// and the safe kill operation times out.
        /// </summary>
        public event EventHandler KillTimeout;

        /// <summary>
        /// Gets the base path to use when resolving process paths.
        /// The base path is the directory of the currently-executing assembly.
        /// </summary>
        public string BasePath
        {
            get
            {
                if (this.basePath == null)
                {
                    this.basePath = System.IO.Path.GetFullPath(System.IO.Path.GetDirectoryName(GetType().Assembly.Location));
                }

                return this.basePath;
            }
        }

        /// <summary>
        /// Gets or sets the configuration path of the application.
        /// </summary>
        public string ConfigPath
        {
            get
            {
                return this.configPath ?? string.Empty;
            }

            set
            {
                string c = (value ?? string.Empty).Trim();

                if (!string.IsNullOrEmpty(c) && c.IndexOfAny(System.IO.Path.GetInvalidPathChars()) >= 0)
                {
                    throw new ArgumentException("value contains invalid path characters.", "value");
                }

                this.configPath = c;
                this.startInfo = null;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to force the application to 32-bit.
        /// </summary>
        public bool Force32Bit
        {
            get
            {
                return this.force32Bit;
            }

            set
            {
                this.force32Bit = value;
                this.exePath = null;
                this.startInfo = null;
            }
        }

        /// <summary>
        /// Gets or sets the framework version to use for the application.
        /// </summary>
        public string FrameworkVersion
        {
            get
            {
                return this.frameworkVersion ?? (this.frameworkVersion = "4.0");
            }

            set
            {
                if (!FrameworkExp.IsMatch(value))
                {
                    throw new ArgumentException("value must be one of 3.5 or 4.0.", "value");
                }

                this.frameworkVersion = value;
                this.exePath = null;
                this.startInfo = null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance's application is currently running.
        /// </summary>
        public bool IsRunning
        {
            get { return this.isRunning; }
        }

        /// <summary>
        /// Gets or sets the path of the application.
        /// </summary>
        public string Path
        {
            get
            {
                return this.path;
            }

            set
            {
                string v = (value ?? string.Empty).Trim();

                if (string.IsNullOrEmpty(v))
                {
                    throw new ArgumentNullException("value", "value must contain a value.");
                }

                if (v.IndexOfAny(System.IO.Path.GetInvalidPathChars()) >= 0)
                {
                    throw new ArgumentException("value contains invalid path characters.", "value");
                }

                this.path = v;
                this.startInfo = null;
            }
        }

        /// <summary>
        /// Gets or sets the threshold, in milliseconds, to compress file system events into.
        /// </summary>
        public int Threshold
        {
            get
            {
                return (int)(this.threshold ?? (this.threshold = 2000));
            }

            set
            {
                if (value < 500)
                {
                    throw new ArgumentOutOfRangeException("value", "value must be greater than or equal to 500.");
                }

                this.threshold = value;
                this.startInfo = null;
            }
        }

        /// <summary>
        /// Gets the path of the Collar.exe executable to use.
        /// </summary>
        private string ExePath
        {
            get
            {
                if (this.exePath == null)
                {
                    string dir;

                    if (this.Force32Bit)
                    {
                        if (this.FrameworkVersion == "3.5")
                        {
                            dir = System.IO.Path.Combine("x86", "Net35");
                        }
                        else
                        {
                            dir = System.IO.Path.Combine("x86", "Net40");
                        }
                    }
                    else
                    {
                        if (this.FrameworkVersion == "3.5")
                        {
                            dir = System.IO.Path.Combine("AnyCPU", "Net35");
                        }
                        else
                        {
                            dir = System.IO.Path.Combine("AnyCPU", "Net40");
                        }
                    }

                    this.exePath = System.IO.Path.Combine(System.IO.Path.Combine(this.BasePath, dir), "Collar.exe");
                }

                return this.exePath;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance's process's
        /// <see cref="Process.HasExited"/> flag is set.
        /// </summary>
        private bool ProcessHasExited
        {
            get
            {
                if (this.process != null)
                {
                    try
                    {
                        return this.process.HasExited;
                    }
                    catch (InvalidOperationException)
                    {
                    }
                    catch (Win32Exception)
                    {
                    }
                    catch (NotSupportedException)
                    {
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// Gets the <see cref="ProcessStartInfo"/> to  use when building the process.
        /// </summary>
        private ProcessStartInfo StartInfo
        {
            get
            {
                if (this.startInfo == null)
                {
                    string args = string.Format(
                        CultureInfo.InvariantCulture,
                        @"/app:""{0}"" /config:""{1}"" /thresh:{2}",
                        this.Path,
                        this.ConfigPath,
                        this.Threshold);

                    try
                    {
                        int pid = Process.GetCurrentProcess().Id;

                        if (pid > 0)
                        {
                            args += string.Format(CultureInfo.InvariantCulture, " /pid:{0}", pid);
                        }
                    }
                    catch (PlatformNotSupportedException)
                    {
                    }
                    catch (InvalidOperationException)
                    {
                    }

                    this.startInfo = new ProcessStartInfo()
                    {
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        RedirectStandardError = true,
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        FileName = this.ExePath,
                        Arguments = args
                    };
                }

                return this.startInfo;
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
        /// Starts this instance's application's process, if it is not started already.
        /// </summary>
        /// <returns>True if the process started successfully, false otherwise.</returns>
        public bool Start()
        {
            bool success = false;

            lock (this.locker)
            {
                if (!this.isRunning)
                {
                    if (this.process != null)
                    {
                        this.KillProcess();
                    }

                    this.process = new Process();
                    this.process.StartInfo = this.StartInfo;
                    
                    try
                    {
                        success = this.process.Start();
                        this.process.Exited += new EventHandler(this.ProcessExited);
                        this.process.EnableRaisingEvents = true;
                        this.isRunning = true;
                    }
                    catch (Win32Exception ex)
                    {
                        this.logger.Error(CultureInfo.InvariantCulture, "{0}\n\n{1}", ex.Message, ex.StackTrace);
                    }
                }
                else
                {
                    success = true;
                }
            }

            return success;
        }

        /// <summary>
        /// Stops this instance's process.
        /// </summary>
        /// <param name="force">A value indicating whether to force the process to exit immediately.</param>
        public void Stop(bool force)
        {
            lock (this.locker)
            {
                if (force)
                {
                    this.KillProcess();
                    this.logger.Info("The application at '{0}' has been forced to exit.", this.Path);
                }
                else if (this.process != null)
                {
                    this.process.StandardInput.WriteLine("exit");
                    this.logger.Info("The application at '{0}' has been issued an exit command.", this.Path);
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
                    this.KillProcess();
                }

                this.disposed = true;
            }
        }

        /// <summary>
        /// Kills this instance's process.
        /// </summary>
        private void KillProcess()
        {
            lock (this.locker)
            {
                if (this.process != null)
                {
                    try
                    {
                        if (!this.ProcessHasExited)
                        {
                            try
                            {
                                System.Threading.Thread.Sleep(10000);

                                // Try to ensure cleanup during shutdown, while forcing all workers
                                // to abandon their work.
                                new Action(
                                    () =>
                                    {
                                        this.process.Exited -= new EventHandler(this.ProcessExited);
                                        this.process.StandardInput.WriteLine("force");
                                        this.process.WaitForExit();
                                    }).InvokeWithTimeout(10000);
                            }
                            catch (TimeoutException)
                            {
                                if (this.KillTimeout != null)
                                {
                                    this.KillTimeout(this, EventArgs.Empty);
                                }
                            }
                        }

                        if (!this.ProcessHasExited)
                        {
                            try
                            {
                                this.process.Kill();
                            }
                            catch (Win32Exception ex)
                            {
                                this.logger.Error(CultureInfo.InvariantCulture, "{0}\n\n{1}", ex.Message, ex.StackTrace);
                            }
                            catch (InvalidOperationException ex)
                            {
                                this.logger.Error(CultureInfo.InvariantCulture, "{0}\n\n{1}", ex.Message, ex.StackTrace);
                            }
                        }
                    }
                    finally
                    {
                        this.isRunning = false;
                        
                        if (this.process != null)
                        {
                            this.process.Dispose();
                            this.process = null;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Raises the process' Exited event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void ProcessExited(object sender, EventArgs e)
        {
            int exitCode;
            string output;

            lock (this.locker)
            {
                exitCode = this.process.ExitCode;
                output = this.process.StandardError.ReadToEnd();

                if (string.IsNullOrEmpty(output))
                {
                    output = this.process.StandardOutput.ReadToEnd();
                }

                this.isRunning = false;
                this.process.Dispose();
                this.process = null;
            }

            if (exitCode == 0)
            {
                if (!string.IsNullOrEmpty(output))
                {
                    this.logger.Info(output);
                }

                this.logger.Info("The application at '{0}' has exited.", this.Path);
            }
            else
            {
                if (!string.IsNullOrEmpty(output))
                {
                    this.logger.Error(output);
                }

                this.logger.Error("The application at '{0}' has exited with an error.", this.Path);
            }

            if (this.Exited != null)
            {
                this.Exited(this, EventArgs.Empty);
            }
        }
    }
}
