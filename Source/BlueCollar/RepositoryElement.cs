//-----------------------------------------------------------------------
// <copyright file="RepositoryElement.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;
    using System.Configuration;

    /// <summary>
    /// Implements the repository configuration element.
    /// </summary>
    public sealed class RepositoryElement : ConfigurationElement
    {
        /// <summary>
        /// Gets the name of the connection string to use when initializing the repository, if applicable.
        /// </summary>
        [ConfigurationProperty("connectionStringName", IsRequired = false)]
        public string ConnectionStringName
        {
            get { return (string)this["connectionStringName"]; }
        }

        /// <summary>
        /// Gets the type of repository to use.
        /// </summary>
        [ConfigurationProperty("type", IsRequired = false, DefaultValue = "BlueCollar.SQLiteRepository, BlueCollar.SQLiteRepository")]
        public string RepositoryType
        {
            get { return (string)this["type"]; }
        }
    }
}
