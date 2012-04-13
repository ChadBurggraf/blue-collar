//-----------------------------------------------------------------------
// <copyright file="TestJob.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Test
{
    using System;
    using System.Threading;
    using Newtonsoft.Json;

    /// <summary>
    /// Implements <see cref="IJob"/> for testing.
    /// </summary>
    public class TestJob : IJob
    {
        private int retries, timeout, sleepDuration = 1;

        /// <summary>
        /// Gets or sets the serialized ID.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets the display name of the job.
        /// </summary>
        [JsonIgnore]
        public string Name
        {
            get { return "Test"; }
        }

        /// <summary>
        /// Gets or sets the maximum number of retries for the job.
        /// Use 0 for infinite, -1 for none.
        /// </summary>
        public int Retries
        {
            get { return this.retries; }
            set { this.retries = value; }
        }

        /// <summary>
        /// Gets or sets the sleep duration to use when simulating execution.
        /// </summary>
        public int SleepDuration
        {
            get { return this.sleepDuration; }
            set { this.sleepDuration = value; }
        }

        /// <summary>
        /// Gets or sets the maximum timeout, in milliseconds, the job is allowed to run in.
        /// Use 0 for infinite.
        /// </summary>
        public int Timeout
        {
            get { return this.timeout; }
            set { this.timeout = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to throw an exception during execution.
        /// </summary>
        public bool ThrowException { get; set; }

        /// <summary>
        /// Executes the job.
        /// </summary>
        public void Execute()
        {
            if (this.ThrowException)
            {
                throw new InvalidOperationException();
            }

            Thread.Sleep(this.SleepDuration);
        }
    }
}