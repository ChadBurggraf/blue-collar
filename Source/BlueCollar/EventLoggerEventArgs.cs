//-----------------------------------------------------------------------
// <copyright file="EventLoggerEventArgs.cs" company="Tasty Codes">
//     Copyright (c) 2012 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;

    /// <summary>
    /// Provides event arguments for <see cref="EventLogger"/> events.
    /// </summary>
    [Serializable]
    public sealed class EventLoggerEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the EventLoggerEventArgs class.
        /// </summary>
        /// <param name="eventType">The event type.</param>
        /// <param name="message">The event message.</param>
        public EventLoggerEventArgs(EventLoggerEventType eventType, string message)
            : this(eventType, message, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the EventLoggerEventArgs class.
        /// </summary>
        /// <param name="eventType">The event type.</param>
        /// <param name="message">The event message.</param>
        /// <param name="ex">The event exception.</param>
        public EventLoggerEventArgs(EventLoggerEventType eventType, string message, Exception ex)
        {
            this.EventType = eventType;
            this.Message = message ?? string.Empty;
            this.Exception = ex;
        }

        /// <summary>
        /// Gets the event type.
        /// </summary>
        public EventLoggerEventType EventType { get; private set; }

        /// <summary>
        /// Gets the event exception, if applicable.
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// Gets the event message.
        /// </summary>
        public string Message { get; private set; }
    }
}
