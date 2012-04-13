//-----------------------------------------------------------------------
// <copyright file="ScheduledJobListHandler.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Dashboard
{
    using System;
    using System.Web;

    /// <summary>
    /// Implements the scheduled job list handler.
    /// </summary>
    public sealed class ScheduledJobListHandler : JsonHandler
    {
        private long? id;

        /// <summary>
        /// Initializes a new instance of the ScheduledJobListHandler class.
        /// </summary>
        /// <param name="repositoryFactory">The repository factory to use.</param>
        public ScheduledJobListHandler(IRepositoryFactory repositoryFactory)
            : base(repositoryFactory)
        {
        }

        /// <summary>
        /// Gets the ID of the resource requesting to be deleted.
        /// </summary>
        public long Id
        {
            get
            {
                if (this.id == null)
                {
                    this.id = Helper.RouteIntValue(0) ?? 0;
                }

                return this.id.Value;
            }
        }

        /// <summary>
        /// Performs the concrete request operation and returns the output
        /// as a byte array.
        /// </summary>
        /// <param name="context">The HTTP context to perform the request for.</param>
        /// <returns>The response to write.</returns>
        protected override byte[] PerformRequest(HttpContextBase context)
        {
            if (this.Id > 0)
            {
                ScheduledJobRecordList result = Repository.GetScheduledJobList(
                    ApplicationName,
                    this.Id,
                    QueryString["q"],
                    Helper.PageSize,
                    Helper.PagingOffset(Helper.QueryIntValue("p")),
                    null);

                if (result != null && result.Id != null && result.Id > 0)
                {
                    return Json(result);
                }
                else
                {
                    NotFound();
                }
            }
            else
            {
                BadRequest();
            }

            return null;
        }
    }
}
