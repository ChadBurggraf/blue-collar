//-----------------------------------------------------------------------
// <copyright file="ResponseCacheModes.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Dashboard
{
    using System;

    /// <summary>
    /// Defines the possible response cache modes.
    /// </summary>
    [Flags]
    public enum ResponseCacheModes
    {
        /// <summary>
        /// Identifies that a response cannot be cached.
        /// </summary>
        None = 0,

        /// <summary>
        /// Identifies that a response can be cached on the server.
        /// </summary>
        Server = 1,

        /// <summary>
        /// Identifies that a response can be cached on the client.
        /// </summary>
        Client = 2
    }
}
