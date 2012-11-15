//-----------------------------------------------------------------------
// <copyright file="HistoryDetailsHandler.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Dashboard
{
    using System;
    using System.Web;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Implements the history details handler.
    /// </summary>
    public sealed class HistoryDetailsHandler : JsonHandler
    {
        private long? id;

        /// <summary>
        /// Initializes a new instance of the HistoryDetailsHandler class.
        /// </summary>
        /// <param name="repositoryFactory">The repository factory to use.</param>
        public HistoryDetailsHandler(IRepositoryFactory repositoryFactory)
            : base(repositoryFactory)
        {
        }

        /// <summary>
        /// Gets the ID of the resource being requested.
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
                HistoryDetailsRecord record = Repository.GetHistoryDetails(this.Id);

                if (record != null)
                {
                    if (!string.IsNullOrEmpty(record.Data)) 
                    {
                        record.Data = JObject.Parse(record.Data).ToString(Formatting.Indented);
                    }

                    return Json(record);
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
