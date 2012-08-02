//-----------------------------------------------------------------------
// <copyright file="DashboardElement.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Represents the dashboard configuration.
    /// </summary>
    public sealed class DashboardElement : ConfigurationElement
    {
        /// <summary>
        /// Gets a value indicating whether caching of applicable dashboard HTTP responses is enabled.
        /// </summary>
        [ConfigurationProperty("cachingEnabled", IsRequired = false, DefaultValue = true)]
        public bool CachingEnabled
        {
            get { return (bool)this["cachingEnabled"]; }
        }

        /// <summary>
        /// Gets the URL the dashboard handler is configured at.
        /// </summary>
        [ConfigurationProperty("handlerUrl", IsRequired = false, DefaultValue = "~/collar")]
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Configuration property.")]
        public string HandlerUrl
        {
            get { return (string)this["handlerUrl"]; }
        }

        /// <summary>
        /// Gets the confiured dashboard enabled mode.
        /// </summary>
        [ConfigurationProperty("mode", IsRequired = false, DefaultValue = DashboardEnabledMode.LocalOnly)]
        public DashboardEnabledMode Mode
        {
            get { return (DashboardEnabledMode)this["mode"]; }
        }
    }
}
