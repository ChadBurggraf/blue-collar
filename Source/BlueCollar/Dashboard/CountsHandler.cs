//-----------------------------------------------------------------------
// <copyright file="CountsHandler.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Dashboard
{
    using System;
    using System.Web;

    /// <summary>
    /// Implements the counts handler.
    /// </summary>
    public sealed class CountsHandler : JsonHandler
    {
        /// <summary>
        /// Initializes a new instance of the CountsHandler class.
        /// </summary>
        /// <param name="repositoryFactory">The repository factory to use.</param>
        public CountsHandler(IRepositoryFactory repositoryFactory)
            : base(repositoryFactory)
        {
        }

        /// <summary>
        /// Performs the concrete request operation and returns the output
        /// as a byte array.
        /// </summary>
        /// <param name="context">The HTTP context to perform the request for.</param>
        /// <returns>The response to write.</returns>
        protected override byte[] PerformRequest(HttpContextBase context)
        {
            return Json(Repository.GetCounts(ApplicationName, null));
        }
    }
}
