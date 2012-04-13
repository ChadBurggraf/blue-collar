//-----------------------------------------------------------------------
// <copyright file="DashboardRoute{T}.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Dashboard
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Generic <see cref="IDashboardRoute"/> implementation.
    /// </summary>
    /// <typeparam name="T">The <see cref="IDashboardHandler"/> implementation type the route routes to.</typeparam>
    internal class DashboardRoute<T> : IDashboardRoute where T : IDashboardHandler
    {
        /// <summary>
        /// Initializes a new instance of the DashboardRoute class.
        /// </summary>
        /// <param name="pattern">The regular expression to use when matching routes.</param>
        /// <param name="verbs">The HTTP verbs to match.</param>
        public DashboardRoute(string pattern, IEnumerable<string> verbs)
        {
            if (string.IsNullOrEmpty(pattern))
            {
                throw new ArgumentNullException("pattern", "pattern must contain a value.");
            }

            this.Expression = new Regex(pattern, RegexOptions.IgnoreCase);
            this.Verbs = (verbs ?? new string[0]).ToArray();
        }

        /// <summary>
        /// Gets the expression this route uses when matching.
        /// </summary>
        public Regex Expression { get; private set; }

        /// <summary>
        /// Gets the collection of HTTP verbs this route uses when matching.
        /// </summary>
        public IEnumerable<string> Verbs { get; private set; }

        /// <summary>
        /// Creates an <see cref="IDashboardHandler"/> for the given HTTP verb and URL path.
        /// </summary>
        /// <param name="verb">The HTTP verb to create the handler for.</param>
        /// <param name="path">The URL path to create the handler for.</param>
        /// <param name="repositoryFactory">The repository factory to use when creating the route.</param>
        /// <returns>The created handler.</returns>
        public IDashboardHandler CreateHandler(string verb, string path, IRepositoryFactory repositoryFactory)
        {
            Match match = this.Expression.Match(path);

            IDashboardHandler handler = (IDashboardHandler)typeof(T)
                .GetConstructor(new Type[] { typeof(IRepositoryFactory) })
                .Invoke(new object[] { repositoryFactory });

            for (int i = 1; i < match.Groups.Count; i++)
            {
                handler.RouteParameters.Add(match.Groups[i].Value);
            }

            return handler;
        }

        /// <summary>
        /// Gets a value indicating whether the given HTTP verb and URL path
        /// matches this route.
        /// </summary>
        /// <param name="verb">The HTTP verb to check.</param>
        /// <param name="path">The URL path to check.</param>
        /// <returns>True if the this route matches, false otherwise.</returns>
        public virtual bool IsMatch(string verb, string path)
        {
            if (this.Verbs.Count() == 0 || this.Verbs.Contains(verb, StringComparer.OrdinalIgnoreCase))
            {
                return this.Expression.IsMatch(path);
            }

            return false;
        }
    }
}
