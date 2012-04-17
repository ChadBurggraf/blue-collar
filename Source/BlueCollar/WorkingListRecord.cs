//-----------------------------------------------------------------------
// <copyright file="WorkingListRecord.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Represents a working job in a list.
    /// </summary>
    public class WorkingListRecord
    {
        private DateTime queuedOn, startedOn;

        /// <summary>
        /// Gets or sets the record's ID.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the job's display name.
        /// </summary>
        public string JobName { get; set; }

        /// <summary>
        /// Gets or sets the job's type string.
        /// </summary>
        public string JobType { get; set; }

        /// <summary>
        /// Gets or sets the date the job was queued.
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
        /// Gets or sets the name of the queue the job is targeting.
        /// </summary>
        public string QueueName { get; set; }

        /// <summary>
        /// Gets or sets the name of the schedule the job was enqueued on behalf of.
        /// </summary>
        public string ScheduleName { get; set; }

        /// <summary>
        /// Gets or sets the job's signal value.
        /// </summary>
        public WorkingSignal Signal { get; set; }

        /// <summary>
        /// Gets or sets the date the job started work.
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
        /// Gets or sets the try number of the job.
        /// </summary>
        public long TryNumber { get; set; }

        /// <summary>
        /// Gets or sets the worker's machine address.
        /// </summary>
        public string WorkerMachineAddress { get; set; }

        /// <summary>
        /// Gets or sets the worker's machine name.
        /// </summary>
        public string WorkerMachineName { get; set; }

        /// <summary>
        /// Gets or sets the worker's name.
        /// </summary>
        public string WorkerName { get; set; }
    }
}
