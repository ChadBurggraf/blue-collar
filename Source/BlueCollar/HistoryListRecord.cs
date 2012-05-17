//-----------------------------------------------------------------------
// <copyright file="HistoryListRecord.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Represents a history record in a list.
    /// </summary>
    public class HistoryListRecord
    {
        private DateTime finishedOn, queuedOn, startedOn;

        /// <summary>
        /// Gets or sets the date the job finished on.
        /// </summary>
        public DateTime FinishedOn
        {
            get
            {
                return this.finishedOn;
            }

            set
            {
                this.finishedOn = value.NormalizeToUtc();
            }
        }

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
        /// Gets or sets the date the job was queued on.
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
        /// Gets or sets the date the job started on.
        /// </summary>
        public DateTime StartedOn
        {
            get
            {
                return this.startedOn;
            }

            set
            {
                this.startedOn = value.NormalizeToUtc();
            }
        }

        /// <summary>
        /// Gets or sets the execution status.
        /// </summary>
        public HistoryStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the job's try number.
        /// </summary>
        public long TryNumber { get; set; }
    }
}
