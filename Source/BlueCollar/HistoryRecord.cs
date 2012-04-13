//-----------------------------------------------------------------------
// <copyright file="HistoryRecord.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;

    /// <summary>
    /// Represents a processed job in the persistent store.
    /// </summary>
    public sealed class HistoryRecord : JobRecordBase
    {
        private DateTime finishedOn, startedOn;

        /// <summary>
        /// Gets or sets the string value of the exception that occurred, if applicable.
        /// </summary>
        public string Exception { get; set; }

        /// <summary>
        /// Gets or sets the date the job finished processing.
        /// </summary>
        public DateTime FinishedOn
        {
            get { return this.finishedOn; }
            set { this.finishedOn = value.NormalizeToUtc(); }
        }

        /// <summary>
        /// Gets or sets the date the job started work.
        /// </summary>
        public DateTime StartedOn
        {
            get { return this.startedOn; }
            set { this.startedOn = value.NormalizeToUtc(); }
        }

        /// <summary>
        /// Gets or sets the job's status.
        /// </summary>
        public HistoryStatus Status { get; set; }

        /// <summary>
        /// Gets the job's status as a string.
        /// </summary>
        public string StatusString
        {
            get { return this.Status.ToString(); }
        }

        /// <summary>
        /// Gets or sets the ID of the worker the job is being processed by.
        /// </summary>
        public long WorkerId { get; set; }
    }
}
