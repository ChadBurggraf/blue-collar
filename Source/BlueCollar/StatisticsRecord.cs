//-----------------------------------------------------------------------
// <copyright file="StatisticsRecord.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents a set of general system statistics.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public sealed class StatisticsRecord
    {
        [JsonProperty(PropertyName = "JobsPerHourByDay")]
        private IList<JobsPerHourByDayRecord> jobsPerHourByDay = new List<JobsPerHourByDayRecord>();
        [JsonProperty(PropertyName = "JobsPerWorker")]
        private IList<JobsPerWorkerRecord> jobsPerWorker = new List<JobsPerWorkerRecord>();

        /// <summary>
        /// Gets or sets the simple counts.
        /// </summary>
        [JsonProperty]
        public CountsRecord Counts { get; set; }

        /// <summary>
        /// Gets or sets a list of history status counts for the distant period.
        /// </summary>
        [JsonProperty]
        public HistoryStatusCountsRecord HistoryStatusDistant { get; set; }

        /// <summary>
        /// Gets or sets a set of history status counts for the recent period.
        /// </summary>
        [JsonProperty]
        public HistoryStatusCountsRecord HistoryStatusRecent { get; set; }

        /// <summary>
        /// Gets a list of jobs per hour by day averages.
        /// </summary>
        public IList<JobsPerHourByDayRecord> JobsPerHourByDay
        {
            get { return this.jobsPerHourByDay; }
        }

        /// <summary>
        /// Gets a list of jobs per worker counts.
        /// </summary>
        public IList<JobsPerWorkerRecord> JobsPerWorker
        {
            get { return this.jobsPerWorker; }
        }
    }
}
