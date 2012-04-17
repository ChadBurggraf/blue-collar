//-----------------------------------------------------------------------
// <copyright file="ScheduleListRecord.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Represents a schedule in a list.
    /// </summary>
    public sealed class ScheduleListRecord
    {
        private DateTime startOn;
        private DateTime? endOn;

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
        /// Gets or sets the schedule's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the schedule's job count.
        /// </summary>
        public long JobCount { get; set; }

        /// <summary>
        /// Gets or sets the queue the schedule enqueues jobs to.
        /// </summary>
        public string QueueName { get; set; }

        /// <summary>
        /// Gets or sets the schedule's repeat type.
        /// </summary>
        public ScheduleRepeatType RepeatType { get; set; }

        /// <summary>
        /// Gets or sets the schedule's repeat interval value.
        /// </summary>
        public long? RepeatValue { get; set; }

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
