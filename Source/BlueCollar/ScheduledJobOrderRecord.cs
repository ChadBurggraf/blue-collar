//-----------------------------------------------------------------------
// <copyright file="ScheduledJobOrderRecord.cs" company="Tasty Codes">
//     Copyright (c) 2012 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;

    /// <summary>
    /// Represents a desired order number for a scheduled job.
    /// </summary>
    public sealed class ScheduledJobOrderRecord
    {
        /// <summary>
        /// Gets or sets the scheduled job's ID.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the scheduled job's order number.
        /// </summary>
        public long Number { get; set; }

        /// <summary>
        /// Gets or sets the ID of the schedule the scheduled job belongs to.
        /// </summary>
        public long ScheduleId { get; set; }
    }
}
