//-----------------------------------------------------------------------
// <copyright file="WorkerRecord.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Represents a worker in the persistent store.
    /// </summary>
    public sealed class WorkerRecord
    {
        private DateTime? lockedUpdatedOn;
        private DateTime updatedOn;

        /// <summary>
        /// Gets or sets the name of the application the worker does work for.
        /// </summary>
        [JsonIgnore]
        public string ApplicationName { get; set; }

        /// <summary>
        /// Gets or sets the ID of the worker.
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
        /// Gets or sets the address of the machine the worker operates on.
        /// </summary>
        public string MachineAddress { get; set; }

        /// <summary>
        /// Gets or sets the name of the machine the worker operates on.
        /// </summary>
        public string MachineName { get; set; }

        /// <summary>
        /// Gets or sets the display name of the worker.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the names of the queues the worker processes.
        /// </summary>
        public string QueueNames { get; set; }

        /// <summary>
        /// Gets or sets the worker's signal value.
        /// </summary>
        public WorkerSignal Signal { get; set; }

        /// <summary>
        /// Gets the worker's signal value as a string.
        /// </summary>
        [JsonIgnore]
        public string SignalString
        {
            get { return this.Signal.ToString(); }
        }

        /// <summary>
        /// Gets or sets the worker's status.
        /// </summary>
        public WorkerStatus Status { get; set; }

        /// <summary>
        /// Gets the worker's status as a string.
        /// </summary>
        [JsonIgnore]
        public string StatusString
        {
            get { return this.Status.ToString(); }
        }

        /// <summary>
        /// Gets or sets the worker's startup type.
        /// </summary>
        public WorkerStartupType Startup { get; set; }

        /// <summary>
        /// Gets the worker's startup type as a string.
        /// </summary>
        [JsonIgnore]
        public string StartupString
        {
            get { return this.Startup.ToString(); }
        }

        /// <summary>
        /// Gets or sets the date the worker information was last updated.
        /// </summary>
        public DateTime UpdatedOn
        {
            get { return this.updatedOn; }
            set { this.updatedOn = value.NormalizeToUtc(); }
        }
    }
}