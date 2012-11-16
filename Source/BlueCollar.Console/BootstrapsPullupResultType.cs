//-----------------------------------------------------------------------
// <copyright file="BootstrapsPullupResultType.cs" company="Tasty Codes">
//     Copyright (c) 2012 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Console
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Defines the possible <see cref="Bootstraps.PullUp()"/> result types.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Pullup", Justification = "Reviewed.")]
    public enum BootstrapsPullupResultType
    {
        /// <summary>
        /// Identifies success.
        /// </summary>
        Success,

        /// <summary>
        /// Identifies that the application directory was not found.
        /// </summary>
        ApplicationDirectoryNotFound,

        /// <summary>
        /// Identifies that the configuration file was not found.
        /// </summary>
        ConfigurationFileNotFound,

        /// <summary>
        /// Identifies that an exception was thrown during pullup.
        /// </summary>
        Exception
    }
}
