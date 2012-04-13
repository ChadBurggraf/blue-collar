//-----------------------------------------------------------------------
// <copyright file="WorkingSignal.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;

    /// <summary>
    /// Defines the possible working signals.
    /// </summary>
    public enum WorkingSignal
    {
        /// <summary>
        /// Identifies no signal.
        /// </summary>
        None = 0,

        /// <summary>
        /// Identifies that the job is signalled to cancel.
        /// </summary>
        Cancel = 1
    }
}