//-----------------------------------------------------------------------
// <copyright file="WorkingRecord.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;

    /// <summary>
    /// Represents a working job in the persistent store.
    /// </summary>
    public class WorkingRecord : JobRecordBase
    {
        private DateTime? lockedUpdatedOn;
        private DateTime startedOn;

        /// <summary>
        /// Gets or sets a value indicating whether the record is locked.
        /// </summary>
        public bool Locked { get; set; }

        /// <summary>
        /// Gets or sets the date the value of <see cref="Locked"/> was last updated.
        /// </summary>
        public DateTime? LockedUpdatedOn
        {
            get { return this.lockedUpdatedOn; }
            set { this.lockedUpdatedOn = value.NormalizeToUtc(); }
        }

        /// <summary>
        /// Gets or sets the job's signal value.
        /// </summary>
        public WorkingSignal Signal { get; set; }

        /// <summary>
        /// Gets the job's signal value as a string.
        /// </summary>
        public string SignalString
        {
            get { return this.Signal.ToString(); }
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
        /// Gets or sets the ID of the worker the job is being processed by.
        /// </summary>
        public long WorkerId { get; set; }
    }
}
