//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Console
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Security;
    using System.Security.Permissions;
    using System.Threading;
    using System.Windows.Forms;

    /// <summary>
    /// Program entry point.
    /// </summary>
    [SecurityCritical]
    [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
    public static class Program
    {
        private static readonly object Locker = new object();
        private static bool isRunning;
        private static Thread inputThread;
        private static ConsoleLogger logger;
        private static InputOptions options;
        private static Bootstraps bootstraps;

        /// <summary>
        /// Application entry point.
        /// </summary>
        /// <param name="args">A collection of execution arguments.</param>
        /// <returns>The program's exit code.</returns>
        [SuppressMessage("Microsoft.Naming", "CA2204:LiteralsShouldBeSpelledCorrectly", Justification = "The spelling is correct.")]
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", Justification = "Not localizing yet.")]
        public static int Main(string[] args)
        {
            Console.CancelKeyPress += new ConsoleCancelEventHandler(ConsoleCancelKeyPress);

            options = InputOptions.Create(args);
            logger = new ConsoleLogger(options);
            isRunning = true;

            if (options.IsValid)
            {
                if (!options.Help)
                {
                    if (options.ParentProcessId > 0)
                    {
                        Process parentProcess = Process.GetProcessById(options.ParentProcessId);

                        if (parentProcess != null)
                        {
                            logger.Debug("Parent process was found with ID {0}. Subscribing to 'Exited' event.", options.ParentProcessId);
                            parentProcess.Exited += new EventHandler(ParentProcessExited);
                            parentProcess.EnableRaisingEvents = true;
                        }
                        else
                        {
                            logger.Debug("No parent process was found with ID {0}.", options.ParentProcessId);
                        }
                    }

                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Type 'exit' to shutdown gracefully, or Ctl+C to exit immediately.");
                    Console.ResetColor();
                    Console.WriteLine();

                    inputThread = new Thread(WaitForInput);
                    inputThread.Start();

                    PullupBootstraps();
                }
                else
                {
                    logger.Help();
                }
            }
            else
            {
                logger.InputError();
                return 1;
            }

            return 0;
        }

        /// <summary>
        /// Raises the bootstraps' ApplicationFilesChanged event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private static void BootstrapsApplicationFilesChanged(object sender, FileSystemEventArgs e)
        {
            logger.Info("A change was detected in '{0}'. Shutting down.", e.FullPath);
            
            lock (Locker)
            {
                if (bootstraps != null)
                {
                    bootstraps.Pushdown(false);
                    bootstraps.Dispose();
                    bootstraps = null;
                }
            }

            logger.Info("Re-starting the application at '{0}'.", options.ApplicationPath);
            PullupBootstraps();
        }

        /// <summary>
        /// Raises the bootstraps' Log event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private static void BootstrapsLog(object sender, EventLoggerEventArgs e)
        {
            logger.Log(e);
        }

        /// <summary>
        /// Raises the console's CancelKeyPress event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private static void ConsoleCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            lock (Locker)
            {
                if (bootstraps != null)
                {
                    bootstraps.Dispose();
                    bootstraps = null;
                }
            }
        }

        /// <summary>
        /// Raises the parent process' Exited event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private static void ParentProcessExited(object sender, EventArgs e)
        {
            logger.Info("The process for the application at '{0}' has been orphaned. Shutting down.", options.ApplicationPath);
            isRunning = false;

            lock (Locker)
            {
                if (bootstraps != null)
                {
                    bootstraps.Pushdown(false);
                    bootstraps.Dispose();
                    bootstraps = null;
                }

                if (inputThread != null)
                {
                    inputThread.IsBackground = true;
                }
            }
        }

        /// <summary>
        /// Attempts a Pullup() operation on the bootstraps instance indefinitely until it succeeds.
        /// </summary>
        private static void PullupBootstraps()
        {
            while (isRunning)
            {
                BootstrapsPullupResult result;

                lock (Locker)
                {
                    if (bootstraps == null)
                    {
                        bootstraps = new Bootstraps(options.ApplicationPath, options.ConfigPath, options.Threshold);
                        bootstraps.ApplicationFilesChanged += new EventHandler<FileSystemEventArgs>(BootstrapsApplicationFilesChanged);
                        bootstraps.Log += new EventHandler<EventLoggerEventArgs>(BootstrapsLog);
                    }

                    logger.Info("Starting the application at '{0}'.", bootstraps.ApplicationPath);
                    result  = bootstraps.PullUp();
                }

                if (result.ResultType == BootstrapsPullupResultType.Success)
                {
                    logger.Info("The application at '{0}' is running.", bootstraps.ApplicationPath);
                    break;
                }
                else
                {
                    switch (result.ResultType)
                    {
                        case BootstrapsPullupResultType.ApplicationDirectoryNotFound:
                            logger.Warn("The application directory '{0}' was not found. Trying again in 10 seconds.", bootstraps.ApplicationPath);
                            break;
                        case BootstrapsPullupResultType.ConfigurationFileNotFound:
                            logger.Warn("The configuration file '{0}' was not found. Trying again in 10 seconds.", bootstraps.ConfigPath);
                            break;
                        case BootstrapsPullupResultType.Exception:
                            logger.Error(result.Exception, "An exception occurred while starting the application. Trying again in 10 seconds.");
                            break;
                        default:
                            throw new NotImplementedException();
                    }

                    Thread.Sleep(10000);
                }
            }
        }

        /// <summary>
        /// Waits for input from the command line.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA2204:LiteralsShouldBeSpelledCorrectly", Justification = "The spelling is correct.")]
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", Justification = "Not localizing yet.")]
        private static void WaitForInput()
        {
            while (isRunning)
            {
                string input = (Console.ReadLine() ?? string.Empty).Trim().ToUpperInvariant();

                if (input == "EXIT")
                {
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Waiting for all workers to stop...");
                    Console.ResetColor();
                    Console.WriteLine();

                    isRunning = false;

                    lock (Locker)
                    {
                        if (bootstraps != null)
                        {
                            bootstraps.Pushdown(false);
                            bootstraps.Dispose();
                            bootstraps = null;
                        }
                    }
                }
                else if (input == "FORCE")
                {
                    isRunning = false;

                    lock (Locker)
                    {
                        if (bootstraps != null)
                        {
                            bootstraps.Dispose();
                            bootstraps = null;
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(input))
                {
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Unrecognized command.");
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Type 'exit' to shutdown gracefully, or Ctl+C to exit immediately.");
                    Console.ResetColor();
                    Console.WriteLine();
                }
            }
        }
    }
}
