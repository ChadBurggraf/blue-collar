//-----------------------------------------------------------------------
// <copyright file="JobRecordBase.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;

    /// <summary>
    /// Represents the base class for job-identifying records.
    /// </summary>
    public abstract class JobRecordBase
    {
        private DateTime queuedOn;

        /// <summary>
        /// Gets or sets the name of the application the job belongs to.
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// Gets or sets the serialized job data.
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// Gets or sets the record's ID.
        /// </summary>
        public long? Id { get; set; }

        /// <summary>
        /// Gets or sets the job's display name.
        /// </summary>
        public string JobName { get; set; }

        /// <summary>
        /// Gets or sets the job's type string.
        /// </summary>
        public virtual string JobType { get; set; }

        /// <summary>
        /// Gets or sets the date the job was queued.
        /// </summary>
        public DateTime QueuedOn
        {
            get { return this.queuedOn; }
            set { this.queuedOn = value.NormalizeToUtc(); }
        }

        /// <summary>
        /// Gets or sets the name of the queue the job is targeting.
        /// </summary>
        public string QueueName { get; set; }

        /// <summary>
        /// Gets or sets the ID of the schedule the job is being queued on behalf of.
        /// </summary>
        public long? ScheduleId { get; set; }

        /// <summary>
        /// Gets or sets the try number of the job.
        /// </summary>
        public long TryNumber { get; set; }
    }
}
