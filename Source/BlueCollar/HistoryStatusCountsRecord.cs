//-----------------------------------------------------------------------
// <copyright file="HistoryStatusCountsRecord.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;

    /// <summary>
    /// Represents a breakdown of history record counts by status.
    /// </summary>
    public sealed class HistoryStatusCountsRecord
    {
        /// <summary>
        /// Gets or sets the count of canceled records.
        /// </summary>
        public long CanceledCount { get; set; }

        /// <summary>
        /// Gets or sets the count of failed records.
        /// </summary>
        public long FailedCount { get; set; }

        /// <summary>
        /// Gets or sets the count of interrupted records.
        /// </summary>
        public long InterruptedCount { get; set; }

        /// <summary>
        /// Gets or sets the count of succeeded records.
        /// </summary>
        public long SucceededCount { get; set; }

        /// <summary>
        /// Gets or sets the count of timed-out records.
        /// </summary>
        public long TimedOutCount { get; set; }

        /// <summary>
        /// Gets or sets the total history count.
        /// </summary>
        public long TotalCount { get; set; }
    }
}
