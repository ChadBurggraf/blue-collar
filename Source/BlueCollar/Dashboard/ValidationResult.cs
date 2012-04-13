//-----------------------------------------------------------------------
// <copyright file="ValidationResult.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Dashboard
{
    using System;

    /// <summary>
    /// Represents the results of a validation test.
    /// </summary>
    public sealed class ValidationResult
    {
        /// <summary>
        /// Gets or sets the error message to display.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the name of the member this result is for.
        /// </summary>
        public string MemberName { get; set; }
    }
}
