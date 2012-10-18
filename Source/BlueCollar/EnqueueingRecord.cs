//-----------------------------------------------------------------------
// <copyright file="EnqueueingRecord.cs" company="Tasty Codes">
//     Copyright (c) 2012 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Represents a schedule enqueueing record.
    /// </summary>
    public sealed class EnqueueingRecord
    {
        private DateTime? enqueueingUpdatedOn;

        /// <summary>
        /// Gets or sets a value indicating whether the schedule is currently being enqueued.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Enqueueing", Justification = "The spelling is correct.")]
        public bool Enqueueing { get; set; }

        /// <summary>
        /// Gets or sets the date the value of <see cref="Enqueueing"/> was last updated.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Enqueueing", Justification = "The spelling is correct.")]
        public DateTime? EnqueueingUpdatedOn
        {
            get { return this.enqueueingUpdatedOn; }
            set { this.enqueueingUpdatedOn = value.NormalizeToUtc(); }
        }
    }
}
