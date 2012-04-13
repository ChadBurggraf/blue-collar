//-----------------------------------------------------------------------
// <copyright file="JobsPerHourByDayRecord.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;
    using System.Globalization;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents a count of jobs per hour in a given day.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public sealed class JobsPerHourByDayRecord
    {
        /// <summary>
        /// Gets the date.
        /// </summary>
        /// <remarks>
        /// This is a hack because Dapper and/or System.Data.SQLite is not parsing dates
        /// correctly when the projection uses a SQLite date function.
        /// </remarks>
        [JsonProperty]
        public DateTime Date
        {
            get
            {
                return !string.IsNullOrEmpty(this.Day)
                    ? DateTime.Parse(this.Day, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).NormalizeToUtc()
                    : DateTime.MinValue.NormalizeToUtc();
            }
        }

        /// <summary>
        /// Gets or sets the day.
        /// </summary>
        public string Day { get; set; }

        /// <summary>
        /// Gets or sets the average jobs per hour.
        /// </summary>
        [JsonProperty]
        public long JobsPerHour { get; set; }

        /// <summary>
        /// Gets or sets the queue name this record represents.
        /// </summary>
        [JsonProperty]
        public string QueueName { get; set; }
    }
}
