//-----------------------------------------------------------------------
// <copyright file="SleepJob.cs" company="Tasty Codes">
//     Copyright (c) 2012 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Examples
{
    using System;
    using System.Globalization;
    using System.Threading;

    /// <summary>
    /// Sleep job.
    /// </summary>
    public sealed class SleepJob : ScheduledJob
    {
        private int? duration;

        /// <summary>
        /// Gets or sets the duration, in milliseconds, to sleep for.
        /// </summary>
        public int Duration
        {
            get
            {
                if (this.duration == null)
                {
                    if (!string.IsNullOrEmpty(Properties["Duration"]))
                    {
                        this.duration = Convert.ToInt32(Properties["Duration"], CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        this.duration = 0;
                    }
                }

                return this.duration.Value;
            }

            set
            {
                this.duration = value;
            }
        }

        /// <summary>
        /// Gets the display name of the job.
        /// </summary>
        public override string Name
        {
            get { return "Sleep"; }
        }

        /// <summary>
        /// Executes the job.
        /// </summary>
        public override void Execute()
        {
            if (this.Duration > 0)
            {
                Thread.Sleep(this.Duration);
            }
        }
    }
}
