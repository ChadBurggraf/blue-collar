//-----------------------------------------------------------------------
// <copyright file="HandlerOutput.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Dashboard
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Represents output returned by a dashboard handler.
    /// </summary>
    [Serializable]
    public sealed class HandlerOutput
    {
        /// <summary>
        /// Gets or sets the output data.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "DTO for raw output data.")]
        public byte[] Data { get; set; }

        /// <summary>
        /// Gets or sets the date the output data was last modified.
        /// </summary>
        public DateTime Modified { get; set; }
    }
}
