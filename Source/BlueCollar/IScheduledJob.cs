//-----------------------------------------------------------------------
// <copyright file="IScheduledJob.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;
    using System.Collections.Specialized;

    /// <summary>
    /// Optional interface definition for scheduled jobs.
    /// </summary>
    public interface IScheduledJob : IJob
    {
        /// <summary>
        /// Gets a dictionary to which properties defined in the schedule
        /// are added for reference during execution.
        /// </summary>
        NameValueCollection Properties { get; }
    }
}
