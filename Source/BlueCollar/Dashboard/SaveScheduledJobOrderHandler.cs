//-----------------------------------------------------------------------
// <copyright file="SaveScheduledJobOrderHandler.cs" company="Tasty Codes">
//     Copyright (c) 2012 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Dashboard
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// Implements the save scheduled job order handler.
    /// </summary>
    public sealed class SaveScheduledJobOrderHandler : JsonHandler<ScheduledJobOrderList>
    {
        private long? scheduleId;

        /// <summary>
        /// Initializes a new instance of the SaveScheduledJobOrderHandler class.
        /// </summary>
        /// <param name="repositoryFactory">The repository factory to use.</param>
        public SaveScheduledJobOrderHandler(IRepositoryFactory repositoryFactory)
            : base(repositoryFactory)
        {
        }

        /// <summary>
        /// Gets the requesting schedule ID.
        /// </summary>
        public long ScheduleId
        {
            get
            {
                if (this.scheduleId == null)
                {
                    this.scheduleId = Helper.RouteIntValue(0);

                    if (this.scheduleId == null)
                    {
                        this.scheduleId = 0;
                    }
                }

                return this.scheduleId.Value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the model loaded by <see cref="JsonHandler{T}.GetModel(HttpRequestBase)"/>
        /// is valid.
        /// </summary>
        /// <param name="model">The model to check the validity of.</param>
        /// <param name="results">The results of the validation.</param>
        /// <returns>True if the model passed validation, false otherwise.</returns>
        protected override bool IsValid(ScheduledJobOrderList model, out IEnumerable<ValidationResult> results)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model", "model cannot be null.");
            }

            List<ValidationResult> r = new List<ValidationResult>();

            if (model.Numbers.Any(m => m.Id <= 0 || m.Number <= 0))
            {
                r.Add(new ValidationResult() { ErrorMessage = "All requested order numbers and job IDs must be greater than 0.", MemberName = "Numbers" });
            }

            results = r;
            return r.Count == 0;
        }

        /// <summary>
        /// Performs the concrete request operation and returns the output
        /// as a byte array.
        /// </summary>
        /// <param name="context">The HTTP context to perform the request for.</param>
        /// <param name="model">The model passed in the request's content body.</param>
        /// <returns>The result of the request.</returns>
        protected override object PerformRequest(HttpContextBase context, ScheduledJobOrderList model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model", "model cannot be null.");
            }

            if (this.ScheduleId > 0)
            {
                if (model.Numbers.Count > 0)
                {
                    using (IDbTransaction transaction = Repository.BeginTransaction())
                    {
                        try
                        {
                            foreach (ScheduledJobOrderRecord record in model.Numbers)
                            {
                                record.ScheduleId = this.ScheduleId;
                                Repository.UpdateScheduledJobOrder(record, transaction);
                            }

                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
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
