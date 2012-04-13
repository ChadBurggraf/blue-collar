//-----------------------------------------------------------------------
// <copyright file="EventLoggerEventType.cs" company="Tasty Codes">
//     Copyright (c) 2012 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;

    /// <summary>
    /// Defines the event types for <see cref="EventLogger"/> events.
    /// </summary>
    public enum EventLoggerEventType
    {
        /// <summary>
        /// Identifies a debug event.
        /// </summary>
        Debug,

        /// <summary>
        /// Identifies an error event.
        /// </summary>
        Error,

        /// <summary>
        /// Identifies an info event.
        /// </summary>
        Info,

        /// <summary>
        /// Identifies a warn event.
        /// </summary>
        Warn
    }
}
