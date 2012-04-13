//-----------------------------------------------------------------------
// <copyright file="ScheduleRepeatType.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;

    /// <summary>
    /// Defines the possible schedule repeat types.
    /// </summary>
    public enum ScheduleRepeatType
    {
        /// <summary>
        /// Identifies no repeat.
        /// </summary>
        None = 0,

        /// <summary>
        /// Identifies a repeat by seconds.
        /// </summary>
        Seconds = 1,

        /// <summary>
        /// Identifies a repeat by minutes.
        /// </summary>
        Minutes = 2,

        /// <summary>
        /// Identifies hourly repeat.
        /// </summary>
        Hours = 3,

        /// <summary>
        /// Identifies daily repeat.
        /// </summary>
        Days = 4,

        /// <summary>
        /// Identifies weekly repeat.
        /// </summary>
        Weeks = 5
    }
}