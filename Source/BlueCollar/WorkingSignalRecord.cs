//-----------------------------------------------------------------------
// <copyright file="WorkingSignalRecord.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;

    /// <summary>
    /// Represents a working signal submitted by the dashboard.
    /// </summary>
    public sealed class WorkingSignalRecord
    {
        /// <summary>
        /// Gets or sets the signal.
        /// </summary>
        public WorkingSignal Signal { get; set; }
    }
}
