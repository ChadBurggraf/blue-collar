//-----------------------------------------------------------------------
// <copyright file="EmptyLogger.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;

    /// <summary>
    /// Implements <see cref="ILogger"/> as a stub of no-ops.
    /// </summary>
    public sealed class EmptyLogger : ILogger
    {
        /// <summary>
        /// Logs a debug message.
        /// </summary>
        /// <param name="format">The format of the message to log.</param>
        /// <param name="args">The format arguments.</param>
        public void Debug(string format, params object[] args)
        {
        }

        /// <summary>
        /// Logs an error.
        /// </summary>
        /// <param name="format">The format of the message to log.</param>
        /// <param name="args">The format arguments.</param>
        public void Error(string format, params object[] args)
        {
        }

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="ex">The exception to log.</param>
        public void Error(Exception ex)
        {
        }

        /// <summary>
        /// Logs an error.
        /// </summary>
        /// <param name="ex">The exception to log.</param>
        /// <param name="format">The format of the message to log.</param>
        /// <param name="args">The format arguments.</param>
        public void Error(Exception ex, string format, params object[] args)
        {
        }

        /// <summary>
        /// Logs an information message.
        /// </summary>
        /// <param name="format">The format of the message to log.</param>
        /// <param name="args">The format arguments.</param>
        public void Info(string format, params object[] args)
        {
        }

        /// <summary>
        /// Logs a warning.
        /// </summary>
        /// <param name="format">The format of the message to log.</param>
        /// <param name="args">The format arguments.</param>
        public void Warn(string format, params object[] args)
        {
        }
    }
}
