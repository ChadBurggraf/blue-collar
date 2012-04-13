//-----------------------------------------------------------------------
// <copyright file="ILogger.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Interface definition for loggers.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Logs a debug message.
        /// </summary>
        /// <param name="format">The format of the message to log.</param>
        /// <param name="args">The format arguments.</param>
        void Debug(string format, params object[] args);

        /// <summary>
        /// Logs an error.
        /// </summary>
        /// <param name="format">The format of the message to log.</param>
        /// <param name="args">The format arguments.</param>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", Justification = "Not currently enjoying any of the alternatives.")]
        void Error(string format, params object[] args);

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="ex">The exception to log.</param>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", Justification = "Not currently enjoying any of the alternatives.")]
        void Error(Exception ex);

        /// <summary>
        /// Logs an error.
        /// </summary>
        /// <param name="ex">The exception to log.</param>
        /// <param name="format">The format of the message to log.</param>
        /// <param name="args">The format arguments.</param>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", Justification = "Not currently enjoying any of the alternatives.")]
        void Error(Exception ex, string format, params object[] args);

        /// <summary>
        /// Logs an information message.
        /// </summary>
        /// <param name="format">The format of the message to log.</param>
        /// <param name="args">The format arguments.</param>
        void Info(string format, params object[] args);

        /// <summary>
        /// Logs a warning.
        /// </summary>
        /// <param name="format">The format of the message to log.</param>
        /// <param name="args">The format arguments.</param>
        void Warn(string format, params object[] args);
    }
}
