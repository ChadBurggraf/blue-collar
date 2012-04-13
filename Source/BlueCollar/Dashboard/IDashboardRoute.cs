//-----------------------------------------------------------------------
// <copyright file="IDashboardRoute.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Dashboard
{
    using System;

    /// <summary>
    /// Defines the interface for dashboard routes.
    /// </summary>
    internal interface IDashboardRoute
    {
        /// <summary>
        /// Creates an <see cref="IDashboardHandler"/> for the given HTTP verb and URL path.
        /// </summary>
        /// <param name="verb">The HTTP verb to create the handler for.</param>
        /// <param name="path">The URL path to create the handler for.</param>
        /// <param name="repositoryFactory">The repository factory to use when creating the route.</param>
        /// <returns>The created handler.</returns>
        IDashboardHandler CreateHandler(string verb, string path, IRepositoryFactory repositoryFactory);

        /// <summary>
        /// Gets a value indicating whether the given HTTP verb and URL path
        /// matches this route.
        /// </summary>
        /// <param name="verb">The HTTP verb to check.</param>
        /// <param name="path">The URL path to check.</param>
        /// <returns>True if the this route matches, false otherwise.</returns>
        bool IsMatch(string verb, string path);
    }
}
