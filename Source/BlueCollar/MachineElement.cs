//-----------------------------------------------------------------------
// <copyright file="MachineElement.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;
    using System.Configuration;

    /// <summary>
    /// Represents the machine configuration.
    /// </summary>
    public sealed class MachineElement : ConfigurationElement
    {
        /// <summary>
        /// Gets a value indicating whether to ensure the existence of a default worker
        /// when the machine boots up for this application.
        /// </summary>
        [ConfigurationProperty("ensureDefaultWorker", IsRequired = false, DefaultValue = true)]
        public bool EnsureDefaultWorker
        {
            get { return (bool)this["ensureDefaultWorker"]; }
        }

        /// <summary>
        /// Gets the heartbeat, in seconds, used to poll for machine changes.
        /// </summary>
        [ConfigurationProperty("heartbeat", IsRequired = false, DefaultValue = 60)]
        public int Heartbeat
        {
            get { return (int)this["heartbeat"]; }
        }

        /// <summary>
        /// Gets a value indicating whether service execution is enabled
        /// for this application.
        /// </summary>
        [ConfigurationProperty("serviceExecutionEnabled", IsRequired = false, DefaultValue = false)]
        public bool ServiceExecutionEnabled
        {
            get { return (bool)this["serviceExecutionEnabled"]; }
        }
    }
}
