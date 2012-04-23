//-----------------------------------------------------------------------
// <copyright file="HistoryDetailsRecord.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;

    /// <summary>
    /// Represents additonal details not contained in a <see cref="HistoryListRecord"/>
    /// about a job's history entry.
    /// </summary>
    public sealed class HistoryDetailsRecord
    {
        /// <summary>
        /// Gets or sets the serialized data used to instantiate the job.
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// Gets or sets the exception that was thrown during job execution.
        /// </summary>
        public string Exception { get; set; }

        /// <summary>
        /// Gets or sets the record ID.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the date the job was queued on.
        /// </summary>
        public DateTime QueuedOn { get; set; }

        /// <summary>
        /// Gets or sets the address of the machine that executed the job.
        /// </summary>
        public string WorkerMachineAddress { get; set; }

        /// <summary>
        /// Gets or sets the name of the worker machine that executed the job.
        /// </summary>
        public string WorkerMachineName { get; set; }

        /// <summary>
        /// Gets or sets the name of the worker the executed the job.
        /// </summary>
        public string WorkerName { get; set; }
    }
}
