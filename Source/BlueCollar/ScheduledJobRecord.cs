//-----------------------------------------------------------------------
// <copyright file="ScheduledJobRecord.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents a scheduled job in the persistent store.
    /// </summary>
    public sealed class ScheduledJobRecord
    {
        /// <summary>
        /// Gets or sets the serialized job data.
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// Gets or sets the scheduled job's ID.
        /// </summary>
        public long? Id { get; set; }

        /// <summary>
        /// Gets or sets the job's type string.
        /// </summary>
        public string JobType { get; set; }

        /// <summary>
        /// Gets or sets the schedule the scheduled job belongs to.
        /// </summary>
        [JsonIgnore]
        public ScheduleRecord Schedule { get; set; }

        /// <summary>
        /// Gets or sets the ID of the schedule the scheduled job belongs to.
        /// </summary>
        public long ScheduleId { get; set; }
    }
}