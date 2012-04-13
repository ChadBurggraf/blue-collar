//-----------------------------------------------------------------------
// <copyright file="HandlerHelper.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Dashboard
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;

    /// <summary>
    /// Provides a mixin of helpers for dashboard handlers.
    /// </summary>
    public sealed class HandlerHelper
    {
        /// <summary>
        /// Defines the default page size for lists.
        /// </summary>
        public const int DefaultPageSize = 25;

        /// <summary>
        /// Initializes a new instance of the HandlerHelper class.
        /// </summary>
        /// <param name="handler">The handler to initialize this instance with.</param>
        public HandlerHelper(IDashboardHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler", "handler cannot be null.");
            }

            this.Handler = handler;
        }

        /// <summary>
        /// Gets the handler this instance was initialized with.
        /// </summary>
        public IDashboardHandler Handler { get; private set; }

        /// <summary>
        /// Gets the page size to use for lists.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Leaving as instance for ease of consumption.")]
        public int PageSize
        {
            get { return DefaultPageSize; }
        }

        /// <summary>
        /// Gets the list paging offset to use for the given page number.
        /// </summary>
        /// <param name="pageNumber">The page number to get the list paging offset for.</param>
        /// <returns>A list paging offset.</returns>
        public int PagingOffset(int? pageNumber)
        {
            if (pageNumber == null || pageNumber < 1)
            {
                pageNumber = 1;
            }

            return (pageNumber.Value - 1) * this.PageSize;
        }

        /// <summary>
        /// Gets a DateTime value from the query string.
        /// </summary>
        /// <param name="key">The key to get the value from.</param>
        /// <returns>A DateTime value.</returns>
        public DateTime? QueryDateValue(string key)
        {
            DateTime? result = null;

            if (!string.IsNullOrEmpty(this.Handler.QueryString[key]))
            {
                try
                {
                    result = DateTime.Parse(this.Handler.QueryString[key], CultureInfo.InvariantCulture);
                }
                catch (FormatException)
                {
                }
            }

            return result;
        }

        /// <summary>
        /// Gets an integer value from the query string.
        /// </summary>
        /// <param name="key">The key to get the value from.</param>
        /// <returns>An integer value.</returns>
        public int? QueryIntValue(string key)
        {
            int? result = null;

            if (!string.IsNullOrEmpty(this.Handler.QueryString[key]))
            {
                try
                {
                    result = Convert.ToInt32(this.Handler.QueryString[key], CultureInfo.InvariantCulture);
                }
                catch (FormatException)
                {
                }
                catch (OverflowException)
                {
                }
            }

            return result;
        }

        /// <summary>
        /// Gets an integer value from the route parameters.
        /// </summary>
        /// <param name="index">The index of the parameter to get the value from.</param>
        /// <returns>An integer value.</returns>
        public long? RouteIntValue(int index)
        {
            long? result = null;

            if (this.Handler.RouteParameters != null && this.Handler.RouteParameters.Count > index)
            {
                try
                {
                    result = Convert.ToInt64(this.Handler.RouteParameters[index], CultureInfo.InvariantCulture);
                }
                catch (FormatException)
                {
                }
                catch (OverflowException)
                {
                }
            }

            return result;
        }
    }
}
