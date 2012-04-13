//-----------------------------------------------------------------------
// <copyright file="TestScheduledJob.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Test
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Test scheduled job.
    /// </summary>
    public class TestScheduledJob : TestJob, IScheduledJob
    {
        /// <summary>
        /// Initializes a new instance of the TestScheduledJob class.
        /// </summary>
        public TestScheduledJob()
        {
            this.Properties = new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets a dictionary to which properties defined in the schedule
        /// are added for reference during execution.
        /// </summary>
        public IDictionary<string, string> Properties { get; private set; }
    }
}
