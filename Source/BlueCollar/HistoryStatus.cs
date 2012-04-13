//-----------------------------------------------------------------------
// <copyright file="HistoryStatus.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;

    /// <summary>
    /// Defines the possible history status states.
    /// </summary>
    public enum HistoryStatus
    {
        /// <summary>
        /// Identifies an unknown status.
        /// </summary>
        None = 0,

        /// <summary>
        /// Identifies that a job succeeded.
        /// </summary>
        Succeeded = 1,

        /// <summary>
        /// Identifies that a job failed due to an uncaught exception.
        /// </summary>
        Failed = 2,

        /// <summary>
        /// Identifies that a job was canceled.
        /// </summary>
        Canceled = 3,

        /// <summary>
        /// Identifies that a job timed out.
        /// </summary>
        TimedOut = 4,

        /// <summary>
        /// Identifies that a job was interrupted by the AppDomain shutting down.
        /// </summary>
        Interrupted = 5
    }
}