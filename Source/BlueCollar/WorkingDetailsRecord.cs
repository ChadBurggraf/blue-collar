//-----------------------------------------------------------------------
// <copyright file="WorkingDetailsRecord.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;

    /// <summary>
    /// Represents additional details not contained in <see cref="WorkingListRecord"/>
    /// about a queued job.
    /// </summary>
    public sealed class WorkingDetailsRecord : WorkingListRecord
    {
        /// <summary>
        /// Gets or sets the serialized job data.
        /// </summary>
        public string Data { get; set; }
    }
}
