//-----------------------------------------------------------------------
// <copyright file="WorkerSignal.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;

    /// <summary>
    /// Defines the possible worker signals.
    /// </summary>
    public enum WorkerSignal
    {
        /// <summary>
        /// Identifies no signal.
        /// </summary>
        None = 0,

        /// <summary>
        /// Identifies that that worker is signalled to start.
        /// </summary>
        Start = 1,

        /// <summary>
        /// Identifies that the worker is signalled to stop.
        /// </summary>
        Stop = 2,

        /// <summary>
        /// Identifies that the worker is signalled to refresh the schedules.
        /// </summary>
        RefreshSchedules = 3
    }
}