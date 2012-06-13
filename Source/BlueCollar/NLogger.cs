//-----------------------------------------------------------------------
// <copyright file="NLogger.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;
    using System.Globalization;
    using NLog;

    /// <summary>
    /// Implements <see cref="ILogger"/> using NLog.
    /// </summary>
    public sealed class NLogger : ILogger
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Logs a debug message.
        /// </summary>
        /// <param name="format">The format of the message to log.</param>
        /// <param name="args">The format arguments.</param>
        public void Debug(string format, params object[] args)
        {
            Logger.Debug(CultureInfo.InvariantCulture, format, args);
        }

        /// <summary>
        /// Logs an error.
        /// </summary>
        /// <param name="format">The format of the message to log.</param>
        /// <param name="args">The format arguments.</param>
        public void Error(string format, params object[] args)
        {
            Logger.Error(CultureInfo.InvariantCulture, format, args);
        }

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="ex">The exception to log.</param>
        public void Error(Exception ex)
        {
            if (ex != null)
            {
                Logger.Error(CultureInfo.InvariantCulture, "{0}\n\n{1}", ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        /// Logs an error.
        /// </summary>
        /// <param name="ex">The exception to log.</param>
        /// <param name="format">The format of the message to log.</param>
        /// <param name="args">The format arguments.</param>
        public void Error(Exception ex, string format, params object[] args)
        {
            if (ex != null)
            {
                string message = !string.IsNullOrEmpty(format)
                    ? string.Format(CultureInfo.CurrentCulture, format + "\n\n", args)
                    : string.Empty;

                Logger.Error(CultureInfo.InvariantCulture, "{0}{1}\n\n{2}", message, ex.Message, ex.StackTrace);
            }
            else
            {
                this.Error(format, args);
            }
        }

        /// <summary>
        /// Logs an information message.
        /// </summary>
        /// <param name="format">The format of the message to log.</param>
        /// <param name="args">The format arguments.</param>
        public void Info(string format, params object[] args)
        {
            Logger.Info(CultureInfo.InvariantCulture, format, args);
        }

        /// <summary>
        /// Logs a warning.
        /// </summary>
        /// <param name="format">The format of the message to log.</param>
        /// <param name="args">The format arguments.</param>
        public void Warn(string format, params object[] args)
        {
            Logger.Warn(CultureInfo.InvariantCulture, format, args);
        }
    }
}
