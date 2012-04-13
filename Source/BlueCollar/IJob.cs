//-----------------------------------------------------------------------
// <copyright file="IJob.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;

    /// <summary>
    /// Interface definition for jobs.
    /// </summary>
    public interface IJob
    {
        /// <summary>
        /// Gets the display name of the job.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the maximum number of retries for the job.
        /// Use 0 for infinite, -1 for none.
        /// </summary>
        int Retries { get; }

        /// <summary>
        /// Gets the maximum timeout, in milliseconds, the job is allowed to run in.
        /// Use 0 for infinite.
        /// </summary>
        int Timeout { get; }

        /// <summary>
        /// Executes the job.
        /// </summary>
        void Execute();
    }
}
