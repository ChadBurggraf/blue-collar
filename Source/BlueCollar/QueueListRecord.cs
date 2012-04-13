//-----------------------------------------------------------------------
// <copyright file="QueueListRecord.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;

    /// <summary>
    /// Represents a queue record in a list.
    /// </summary>
    public class QueueListRecord
    {
        private DateTime queuedOn;

        /// <summary>
        /// Gets or sets the record ID.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the job name.
        /// </summary>
        public string JobName { get; set; }

        /// <summary>
        /// Gets or sets the job type.
        /// </summary>
        public string JobType { get; set; }

        /// <summary>
        /// Gets or sets the name of the queue the job executed on.
        /// </summary>
        public string QueueName { get; set; }

        /// <summary>
        /// Gets or sets the date the record was queued.
        /// </summary>
        public DateTime QueuedOn
        {
            get
            {
                return this.queuedOn;
            }

            set
            {
                this.queuedOn = value.NormalizeToUtc();
            }
        }

        /// <summary>
        /// Gets or sets the name of the schedule the job was executed for.
        /// </summary>
        public string ScheduleName { get; set; }

        /// <summary>
        /// Gets or sets the job's try number.
        /// </summary>
        public long TryNumber { get; set; }
    }
}
