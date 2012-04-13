//-----------------------------------------------------------------------
// <copyright file="BlueCollarServiceSection.cs" company="Tasty Codes">
//     Copyright (c) 2012 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Service
{
    using System;
    using System.Configuration;

    /// <summary>
    /// Implements the Blue Collar Service configuration section.
    /// </summary>
    public sealed class BlueCollarServiceSection : ConfigurationSection
    {
        private static readonly object locker = new object();
        private static BlueCollarServiceSection section;

        /// <summary>
        /// Gets the current configuration.
        /// </summary>
        public static BlueCollarServiceSection Section
        {
            get
            {
                lock (locker)
                {
                    return section ?? (section = (BlueCollarServiceSection)(ConfigurationManager.GetSection("blueCollarService") ?? new BlueCollarServiceSection()));
                }
            }
        }

        /// <summary>
        /// Gets the application configuration element collection.
        /// </summary>
        [ConfigurationProperty("applications", IsRequired = false, IsDefaultCollection = true)]
        public ApplicationElementCollection Applications
        {
            get { return (ApplicationElementCollection)(this["applications"] ?? (this["applications"] = new ApplicationElementCollection())); }
        }

        /// <summary>
        /// Refreshes the currently loaded configuration settings to the latest values from the configuration file.
        /// </summary>
        public static void Refresh()
        {
            lock (locker)
            {
                ConfigurationManager.RefreshSection("blueCollarService");
                section = null;
            }
        }
    }
}
