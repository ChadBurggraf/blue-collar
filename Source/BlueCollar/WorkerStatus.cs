//-----------------------------------------------------------------------
// <copyright file="WorkerStatus.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;

    /// <summary>
    /// Defines the possible worker status states.
    /// </summary>
    public enum WorkerStatus
    {
        /// <summary>
        /// Identifies that the worker is stopped.
        /// </summary>
        Stopped = 0,

        /// <summary>
        /// Identifies that the worker is working.
        /// </summary>
        Working = 1,

        /// <summary>
        /// Identifies that the working is stopping.
        /// </summary>
        Stopping = 2
    }
}