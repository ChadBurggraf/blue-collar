//-----------------------------------------------------------------------
// <copyright file="QueueRecord.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;

    /// <summary>
    /// Represents a queued job in the persistent store.
    /// </summary>
    public sealed class QueueRecord : JobRecordBase
    {
        private DateTime? lockedUpdatedOn;

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
    }
}
