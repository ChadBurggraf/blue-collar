//-----------------------------------------------------------------------
// <copyright file="JobsPerWorkerRecord.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;

    /// <summary>
    /// Represents a count of jobs per worker.
    /// </summary>
    public class JobsPerWorkerRecord
    {
        /// <summary>
        /// Gets or sets job count.
        /// </summary>
        public long Count { get; set; }

        /// <summary>
        /// Gets or sets the address of the machine.
        /// </summary>
        public string MachineAddress { get; set; }

        /// <summary>
        /// Gets or sets the name of the machine.
        /// </summary>
        public string MachineName { get; set; }

        /// <summary>
        /// Gets or sets the name of the worker.
        /// </summary>
        public string Name { get; set; }
    }
}
