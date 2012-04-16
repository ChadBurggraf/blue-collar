//-----------------------------------------------------------------------
// <copyright file="ScheduledJob.cs" company="Tasty Codes">
//     Copyright (c) 2012 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Extends <see cref="Job"/> as a base <see cref="IScheduledJob"/> implementation.
    /// </summary>
    public abstract class ScheduledJob : Job, IScheduledJob
    {
        private Dictionary<string, string> properties = new Dictionary<string, string>();

        /// <summary>
        /// Gets a dictionary to which properties defined in the schedule
        /// are added for reference during execution.
        /// </summary>
        public IDictionary<string, string> Properties
        {
            get { return this.properties; }
        }
    }
}
