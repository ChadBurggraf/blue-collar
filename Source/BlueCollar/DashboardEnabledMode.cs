//-----------------------------------------------------------------------
// <copyright file="DashboardEnabledMode.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;

    /// <summary>
    /// Defines the possible dashboard enabled modes.
    /// </summary>
    public enum DashboardEnabledMode
    {
        /// <summary>
        /// Identifies that the dashboard is enabled for local requests only.
        /// </summary>
        LocalOnly = 0,

        /// <summary>
        /// Identifies that the dashboard is not enabled.
        /// </summary>
        Off = 1,

        /// <summary>
        /// Identifies that the dashboard is completely enabled.
        /// </summary>
        On = 2
    }
}
