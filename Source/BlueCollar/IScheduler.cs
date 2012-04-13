//-----------------------------------------------------------------------
// <copyright file="IScheduler.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Interface definition for job schedulers.
    /// </summary>
    public interface IScheduler
    {
        /// <summary>
        /// Gets the date the scheduler last performed an equeue operation.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling is correct.")]
        DateTime? LastEnqueuedOn { get; }

        /// <summary>
        /// Gets or sets the queues to process schedules for.
        /// </summary>
        QueueNameFilters QueueFilters { get; set; }

        /// <summary>
        /// Enqueues scheduled jobs that are due for execution.
        /// </summary>
        void EnqueueScheduledJobs();

        /// <summary>
        /// Refreshes this instance's schedules with the latest data from the repository.
        /// </summary>
        void RefreshSchedules();
    }
}
