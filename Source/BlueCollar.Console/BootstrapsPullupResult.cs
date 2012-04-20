//-----------------------------------------------------------------------
// <copyright file="BootstrapsPullupResult.cs" company="Tasty Codes">
//     Copyright (c) 2012 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Console
{
    using System;

    /// <summary>
    /// Represents the result of a <see cref="Bootstraps.Pullup()"/> operation.
    /// </summary>
    public sealed class BootstrapsPullupResult
    {
        /// <summary>
        /// Initializes a new instance of the BootstrapsPullupResult class.
        /// </summary>
        /// <param name="resultType">The result type.</param>
        public BootstrapsPullupResult(BootstrapsPullupResultType resultType)
            : this(resultType, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the BootstrapsPullupResult class.
        /// </summary>
        /// <param name="resultType">The result type.</param>
        /// <param name="ex">The exception, if applicable.</param>
        public BootstrapsPullupResult(BootstrapsPullupResultType resultType, Exception ex)
        {
            this.ResultType = resultType;
            this.Exception = ex;
        }

        /// <summary>
        /// Gets the exception, if applicable.
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// Gets the result type.
        /// </summary>
        public BootstrapsPullupResultType ResultType { get; private set; }
    }
}
