//-----------------------------------------------------------------------
// <copyright file="ScheduledJob.cs" company="Tasty Codes">
//     Copyright (c) 2012 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;
    using System.Collections.Specialized;

    /// <summary>
    /// Extends <see cref="Job"/> as a base <see cref="IScheduledJob"/> implementation.
    /// </summary>
    public abstract class ScheduledJob : Job, IScheduledJob
    {
        private NameValueCollection properties = new NameValueCollection();

        /// <summary>
        /// Gets a dictionary to which properties defined in the schedule
        /// are added for reference during execution.
        /// </summary>
        public NameValueCollection Properties
        {
            get { return this.properties; }
        }
    }
}
