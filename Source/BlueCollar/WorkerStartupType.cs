//-----------------------------------------------------------------------
// <copyright file="WorkerStartupType.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;

    /// <summary>
    /// Defines the possible worker startup types.
    /// </summary>
    public enum WorkerStartupType
    {
        /// <summary>
        /// Identifies automatic startup.
        /// </summary>
        Automatic = 0,

        /// <summary>
        /// Identifies manual startup.
        /// </summary>
        Manual = 1
    }
}
