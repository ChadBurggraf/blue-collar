//-----------------------------------------------------------------------
// <copyright file="ScheduledJobRecordList.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;

    /// <summary>
    /// Wraps <see cref="RecordList{T}"/> with schedule information.
    /// </summary>
    public sealed class ScheduledJobRecordList : RecordList<ScheduledJobRecord>
    {
        /// <summary>
        /// Gets or sets the schedule's ID.
        /// </summary>
        public long? Id { get; set; }

        /// <summary>
        /// Gets or sets the schedule's name.
        /// </summary>
        public string Name { get; set; }
    }
}
