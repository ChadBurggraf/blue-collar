//-----------------------------------------------------------------------
// <copyright file="IDashboardHandler.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Dashboard
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Web;

    /// <summary>
    /// Interface definition for dashboard <see cref="IHttpHandler"/> implementors.
    /// </summary>
    public interface IDashboardHandler : IHttpHandler, IDisposable
    {
        /// <summary>
        /// Gets or sets the name of the application the request is being processed for.
        /// </summary>
        string ApplicationName { get; set; }

        /// <summary>
        /// Gets or sets the URL of the root handler.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "The easiest format to handle this property in.")]
        string HandlerUrl { get; set; }

        /// <summary>
        /// Gets or sets the handler-relative URL of the request being processed.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "The easiest format to handle this property in.")]
        string HandlerRelativeRequestUrl { get; set; }

        /// <summary>
        /// Gets a collection of URL parameters that matched
        /// the route that created this instance.
        /// </summary>
        IList<string> RouteParameters { get; }

        /// <summary>
        /// Gets or sets the <see cref="QueryString"/> parsed from the current request.
        /// </summary>
        QueryString QueryString { get; set; }

        /// <summary>
        /// Gets or sets the HTTP verb of the request being processed.
        /// </summary>
        string Verb { get; set; }
    }
}
