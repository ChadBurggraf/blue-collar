//-----------------------------------------------------------------------
// <copyright file="EventLogger.cs" company="Tasty Codes">
//     Copyright (c) 2012 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Implements <see cref="ILogger"/> by raising events.
    /// </summary>
    public sealed class EventLogger : MarshalByRefObject, ILogger
    {
        /// <summary>
        /// Event raised when a log event occurs.
        /// </summary>
        public event EventHandler<EventLoggerEventArgs> Log;

        /// <summary>
        /// Logs a debug message.
        /// </summary>
        /// <param name="format">The format of the message to log.</param>
        /// <param name="args">The format arguments.</param>
        public void Debug(string format, params object[] args)
        {
            this.RaiseLog(EventLoggerEventType.Debug, null, format, args);
        }

        /// <summary>
        /// Logs an error.
        /// </summary>
        /// <param name="format">The format of the message to log.</param>
        /// <param name="args">The format arguments.</param>
        public void Error(string format, params object[] args)
        {
            this.RaiseLog(EventLoggerEventType.Error, null, format, args);
        }

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="ex">The exception to log.</param>
        public void Error(Exception ex)
        {
            this.RaiseLog(EventLoggerEventType.Error, ex, null, null);
        }

        /// <summary>
        /// Logs an error.
        /// </summary>
        /// <param name="ex">The exception to log.</param>
        /// <param name="format">The format of the message to log.</param>
        /// <param name="args">The format arguments.</param>
        public void Error(Exception ex, string format, params object[] args)
        {
            this.RaiseLog(EventLoggerEventType.Error, ex, format, args);
        }

        /// <summary>
        /// Logs an information message.
        /// </summary>
        /// <param name="format">The format of the message to log.</param>
        /// <param name="args">The format arguments.</param>
        public void Info(string format, params object[] args)
        {
            this.RaiseLog(EventLoggerEventType.Info, null, format, args);
        }

        /// <summary>
        /// Logs a warning.
        /// </summary>
        /// <param name="format">The format of the message to log.</param>
        /// <param name="args">The format arguments.</param>
        public void Warn(string format, params object[] args)
        {
            this.RaiseLog(EventLoggerEventType.Warn, null, format, args);
        }

        /// <summary>
        /// Raises the <see cref="Log"/> event on this instance.
        /// </summary>
        /// <param name="eventType">The event type to raise.</param>
        /// <param name="ex">The exception to raise the event for, if applicable.</param>
        /// <param name="format">The message format to use.</param>
        /// <param name="args">The message format arguments.</param>
        private void RaiseLog(EventLoggerEventType eventType, Exception ex, string format, params object[] args)
        {
            if (this.Log != null)
            {
                string message = null;

                if (!string.IsNullOrEmpty(format))
                {
                    message = string.Format(CultureInfo.InvariantCulture, format, args);
                }

                this.Log(this, new EventLoggerEventArgs(eventType, message, ex));
            }
        }
    }
}
