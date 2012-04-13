//-----------------------------------------------------------------------
// <copyright file="FileSystemEventType.cs" company="Tasty Codes">
//     Copyright (c) 2012 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.IO
{
    using System;

    /// <summary>
    /// Defines the possible file system event types raised
    /// by a <see cref="FileSystemWatcher"/> object.
    /// </summary>
    internal enum FileSystemEventType
    {
        /// <summary>
        /// Identifies a changed event.
        /// </summary>
        Changed,

        /// <summary>
        /// Identifies a created event.
        /// </summary>
        Created,

        /// <summary>
        /// Identifies a deleted event.
        /// </summary>
        Deleted,

        /// <summary>
        /// Identifies a disposed event.
        /// </summary>
        Disposed,

        /// <summary>
        /// Identifies an error event.
        /// </summary>
        Error,

        /// <summary>
        /// Identifies a renamed event.
        /// </summary>
        Renamed
    }
}
