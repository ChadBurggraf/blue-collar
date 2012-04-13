//-----------------------------------------------------------------------
// <copyright file="CountsRecord.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;

    /// <summary>
    /// Represents a set of counts.
    /// </summary>
    public sealed class CountsRecord
    {
        /// <summary>
        /// Gets or sets the history count.
        /// </summary>
        public long HistoryCount { get; set; }

        /// <summary>
        /// Gets or sets the queue count.
        /// </summary>
        public long QueueCount { get; set; }

        /// <summary>
        /// Gets or sets the schedule count.
        /// </summary>
        public long ScheduleCount { get; set; }

        /// <summary>
        /// Gets or sets the working count.
        /// </summary>
        public long WorkingCount { get; set; }

        /// <summary>
        /// Gets or sets the worker count.
        /// </summary>
        public long WorkerCount { get; set; }
    }
}
