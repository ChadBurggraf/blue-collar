//-----------------------------------------------------------------------
// <copyright file="EnqueueJobRecord.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;

    /// <summary>
    /// Represents a job record used for enqueueing jobs.
    /// </summary>
    public sealed class EnqueueJobRecord
    {
        /// <summary>
        /// Gets or sets the serialized job data.
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// Gets or sets the job's display name.
        /// </summary>
        public string JobName { get; set; }

        /// <summary>
        /// Gets or sets the job's type string.
        /// </summary>
        public string JobType { get; set; }

        /// <summary>
        /// Gets or sets the name of the queue the job is targeting.
        /// </summary>
        public string QueueName { get; set; }
    }
}
