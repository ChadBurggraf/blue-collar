//-----------------------------------------------------------------------
// <copyright file="SignalsRecord.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;

    /// <summary>
    /// Represents signals polled by a worker.
    /// </summary>
    public sealed class SignalsRecord
    {
        private WorkerSignal workerSignal;
        private WorkingSignal workingSignal;
        private string workerSignalString, workingSignalString;

        /// <summary>
        /// Gets or sets the worker's queue names.
        /// </summary>
        public string QueueNames { get; set; }

        /// <summary>
        /// Gets or sets the worker signal.
        /// </summary>
        public WorkerSignal WorkerSignal
        {
            get
            {
                return this.workerSignal;
            }

            set
            {
                this.workerSignal = value;
                this.workerSignalString = value.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the worker signal value as a string.
        /// </summary>
        public string WorkerSignalString
        {
            get
            {
                return this.workerSignalString;
            }

            set
            {
                this.workerSignalString = value;
                this.workerSignal = value.AsEnum<WorkerSignal>();
            }
        }

        /// <summary>
        /// Gets or sets the working signal.
        /// </summary>
        public WorkingSignal WorkingSignal
        {
            get
            {
                return this.workingSignal;
            }

            set
            {
                this.workingSignal = value;
                this.workingSignalString = value.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the working signal value as a string.
        /// </summary>
        public string WorkingSignalString
        {
            get
            {
                return this.workingSignalString;
            }

            set
            {
                this.workingSignalString = value;
                this.workingSignal = value.AsEnum<WorkingSignal>();
            }
        }
    }
}
