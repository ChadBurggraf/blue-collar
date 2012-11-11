//-----------------------------------------------------------------------
// <copyright file="ScheduleRecord.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Represents a schedule.
    /// </summary>
    public sealed class ScheduleRecord
    {
        private DateTime startOn;
        private DateTime? endOn, lockedUpdatedOn;

        /// <summary>
        /// Initializes a new instance of the ScheduleRecord class.
        /// </summary>
        public ScheduleRecord()
        {
            this.ScheduledJobs = new List<ScheduledJobRecord>();
        }

        /// <summary>
        /// Gets or sets the name of the application the schedule enqueues jobs for.
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the schedule is enabled.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the date the schedule ends on.
        /// </summary>
        public DateTime? EndOn
        {
            get { return this.endOn; }
            set { this.endOn = value.NormalizeToUtc(); }
        }

        /// <summary>
        /// Gets or sets the schedule's ID.
        /// </summary>
        public long? Id { get; set; }

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
        /// Gets or sets the schedule's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the queue the schedule enqueues jobs to.
        /// </summary>
        public string QueueName { get; set; }

        /// <summary>
        /// Gets or sets the schedule's repeat type.
        /// </summary>
        public ScheduleRepeatType RepeatType { get; set; }

        /// <summary>
        /// Gets the schedule's repeat type as a string.
        /// </summary>
        public string RepeatTypeString
        {
            get { return this.RepeatType.ToString(); }
        }

        /// <summary>
        /// Gets or sets the schedule's repeat interval value.
        /// </summary>
        public long? RepeatValue { get; set; }

        /// <summary>
        /// Gets the schedule's scheduled job collection.
        /// </summary>
        public IList<ScheduledJobRecord> ScheduledJobs { get; private set; }

        /// <summary>
        /// Gets or sets the date the schedule takes effect on.
        /// </summary>
        public DateTime StartOn
        {
            get { return this.startOn; }
            set { this.startOn = value.NormalizeToUtc(); }
        }
    }
}
