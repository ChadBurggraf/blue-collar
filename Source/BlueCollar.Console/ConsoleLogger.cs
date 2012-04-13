//-----------------------------------------------------------------------
// <copyright file="ConsoleLogger.cs" company="Tasty Codes">
//     Copyright (c) 2012 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Console
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;

    /// <summary>
    /// Provides logging services for the console.
    /// </summary>
    internal sealed class ConsoleLogger
    {
        private static readonly NLogger logger = new NLogger();
        private InputOptions options;

        /// <summary>
        /// Initializes a new instance of the ConsoleLogger class.
        /// </summary>
        /// <param name="options">The parsed <see cref="InputOptions"/> for the current execution context.</param>
        public ConsoleLogger(InputOptions options)
        {
            this.options = options;
        }

        /// <summary>
        /// Logs a debug message.
        /// </summary>
        /// <param name="format">The format of the message to log.</param>
        /// <param name="args">The format arguments.</param>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "All logging methods should have instance-access for consistency.")]
        public void Debug(string format, params object[] args)
        {
            logger.Debug(format, args);
        }

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="ex">The exception to log the error for, if applicable.</param>
        /// <param name="format">The format of the message to log.</param>
        /// <param name="args">The format arguments.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", Justification = "Not localizing yet.")]
        public void Error(Exception ex, string format, params object[] args)
        {
            string message = null;

            if (!string.IsNullOrEmpty(format))
            {
                message = string.Format(CultureInfo.InvariantCulture, format, args);

                if (ex != null) 
                {
                    logger.Error(ex, format, args);
                }
                else 
                {
                    logger.Error(format, args);
                }
            }
            else
            {
                if (ex != null)
                {
                    message = ex.Message;
                    logger.Error(ex);
                }
                else
                {
                    message = "An unknown error occurred.";
                    logger.Error(message);
                }
            }

            if (this.options.Verbose)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("ERROR: ");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine(message);
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Writes the help message to the console.
        /// </summary>
        public void Help()
        {
            Console.Out.WriteLine();
            this.options.OptionSet.WriteOptionDescriptions(Console.Out);
        }

        /// <summary>
        /// Logs an information message.
        /// </summary>
        /// <param name="format">The format of the message to log.</param>
        /// <param name="args">The format arguments.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", Justification = "Not localizing yet.")]
        public void Info(string format, params object[] args)
        {
            logger.Info(format, args);

            if (this.options.Verbose)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("INFO: ");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine(format, args);
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Writes an input error message to the console.
        /// </summary>
        public void InputError()
        {
            string message = this.options.ParseErrorMessage;

            if (string.IsNullOrEmpty(message))
            {
                if (this.options.ParseException != null)
                {
                    message = this.options.ParseException.Message;
                }
                else
                {
                    message = "An error occurred.";
                }
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(message);
            Console.Error.WriteLine();
            Console.ResetColor();
            this.options.OptionSet.WriteOptionDescriptions(Console.Error);
        }

        /// <summary>
        /// Logs an event from an <see cref="EventErrorLogger"/> to the file log,
        /// plus to the console if in verbose mode.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        public void Log(EventLoggerEventArgs e)
        {
            switch (e.EventType)
            {
                case EventLoggerEventType.Debug:
                    this.Debug(e.Message);
                    break;
                case EventLoggerEventType.Error:
                    this.Error(e.Exception, e.Message);
                    break;
                case EventLoggerEventType.Info:
                    this.Info(e.Message);
                    break;
                case EventLoggerEventType.Warn:
                    this.Warn(e.Message);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Logs a warning.
        /// </summary>
        /// <param name="format">The format of the message to log.</param>
        /// <param name="args">The format arguments.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", Justification = "Not localizing yet.")]
        public void Warn(string format, params object[] args)
        {
            logger.Warn(format, args);

            if (this.options.Verbose)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("WARN: ");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine(format, args);
                Console.ResetColor();
            }
        }
    }
}
