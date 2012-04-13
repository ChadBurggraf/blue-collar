//-----------------------------------------------------------------------
// <copyright file="DashboardStatsHandler.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Dashboard
{
    using System;
    using System.Web;

    /// <summary>
    /// Implements the dashboard stats handler.
    /// </summary>
    public sealed class DashboardStatsHandler : JsonHandler
    {
        /// <summary>
        /// Initializes a new instance of the DashboardStatsHandler class.
        /// </summary>
        /// <param name="repositoryFactory">The repository factory to use.</param>
        public DashboardStatsHandler(IRepositoryFactory repositoryFactory)
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
            DateTime now = DateTime.UtcNow.FloorWithSeconds();
            DateTime distant = now.AddDays(-14);
            DateTime recent = now.AddDays(-1);

            return Json(Repository.GetStatistics(ApplicationName, recent, distant, now, null));
        }
    }
}
