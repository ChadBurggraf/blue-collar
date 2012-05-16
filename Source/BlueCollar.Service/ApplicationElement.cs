//-----------------------------------------------------------------------
// <copyright file="ApplicationElement.cs" company="Tasty Codes">
//     Copyright (c) 2012 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Service
{
    using System;
    using System.Configuration;

    /// <summary>
    /// Implements the application configuration element.
    /// </summary>
    public sealed class ApplicationElement : ConfigurationElement
    {
        /// <summary>
        /// Gets or sets the path of the application configuration file, if applicable.
        /// </summary>
        [ConfigurationProperty("applicationConfigPath", IsRequired = false)]
        public string ApplicationConfigPath
        {
            get { return (string)this["applicationConfigPath"]; }
            set { this["applicationConfigPath"] = value; }
        }

        /// <summary>
        /// Gets or sets the path of the application on disk.
        /// </summary>
        [ConfigurationProperty("applicationPath", IsRequired = true, IsKey = true)]
        public string ApplicationPath
        {
            get { return (string)this["applicationPath"]; }
            set { this["applicationPath"] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to force the application to run as
        /// 32-bit, even if running on a 64-bit platform.
        /// </summary>
        [ConfigurationProperty("force32Bit", IsRequired = false, DefaultValue = false)]
        public bool Force32Bit
        {
            get { return (bool)this["force32Bit"]; }
            set { this["force32Bit"] = value; }
        }

        /// <summary>
        /// Gets or sets the framework version to use when launching the application.
        /// Possible values: 4.0, 3.5
        /// </summary>
        [ConfigurationProperty("framework", IsRequired = false, DefaultValue = "4.0")]
        public string Framework
        {
            get { return (string)this["framework"]; }
            set { this["framework"] = value; }
        }

        /// <summary>
        /// Gets a value indicating whether this element is read-only.
        /// </summary>
        /// <returns>True if the element is read-only, false otherwise.</returns>
        public override bool IsReadOnly()
        {
#if DEBUG
            return false;
#else
            return true;
#endif
        }
    }
}
