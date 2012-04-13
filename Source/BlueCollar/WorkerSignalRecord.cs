//-----------------------------------------------------------------------
// <copyright file="WorkerSignalRecord.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;

    /// <summary>
    /// Represents a worker signal submitted by the dashboard.
    /// </summary>
    public sealed class WorkerSignalRecord
    {
        /// <summary>
        /// Gets or sets the signal.
        /// </summary>
        public WorkerSignal Signal { get; set; }
    }
}
