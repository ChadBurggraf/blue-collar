//-----------------------------------------------------------------------
// <copyright file="SaveScheduledJobHandler.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Dashboard
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Web;

    /// <summary>
    /// Implements the save scheduled job handler.
    /// </summary>
    public sealed class SaveScheduledJobHandler : JsonHandler<ScheduledJobRecord>
    {
        private long? scheduleId;

        /// <summary>
        /// Initializes a new instance of the SaveScheduledJobHandler class.
        /// </summary>
        /// <param name="repositoryFactory">The repository factory to use.</param>
        public SaveScheduledJobHandler(IRepositoryFactory repositoryFactory)
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
        protected override bool IsValid(ScheduledJobRecord model, out IEnumerable<ValidationResult> results)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model", "model cannot be null.");
            }

            List<ValidationResult> r = new List<ValidationResult>();
            bool validateJobType = true;

            if (string.IsNullOrEmpty(model.JobType))
            {
                r.Add(new ValidationResult() { ErrorMessage = "Job type is required.", MemberName = "JobType" });
                validateJobType = false;
            }

            if (!string.IsNullOrEmpty(model.JobType) && model.JobType.Length > 256)
            {
                r.Add(new ValidationResult() { ErrorMessage = "Job type cannot be longer than 256 characters.", MemberName = "JobType" });
                validateJobType = false;
            }

            if (validateJobType)
            {
                IJob job = null;
                string error = null;

                if (!ValidateJobType(model.JobType, null, out job, out error))
                {
                    r.Add(new ValidationResult() { ErrorMessage = error, MemberName = "JobType" });
                }
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
        protected override object PerformRequest(HttpContextBase context, ScheduledJobRecord model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model", "model cannot be null.");
            }

            if (this.ScheduleId > 0)
            {
                model.ScheduleId = this.ScheduleId;

                using (IDbTransaction transaction = Repository.BeginTransaction())
                {
                    try
                    {
                        ScheduleRecord schedule = Repository.GetSchedule(this.ScheduleId, transaction);

                        if (schedule != null)
                        {
                            if ("PUT".Equals(this.Verb, StringComparison.OrdinalIgnoreCase))
                            {
                                model.Id = Helper.RouteIntValue(1);

                                if (model.Id != null && model.Id > 0)
                                {
                                    model = Repository.UpdateScheduledJob(model, transaction);
                                }
                                else
                                {
                                    BadRequest();
                                }
                            }
                            else
                            {
                                model = Repository.CreateScheduledJob(model, transaction);
                            }

                            Repository.SignalWorkers(schedule.ApplicationName, WorkerSignal.RefreshSchedules, transaction);
                        }
                        else
                        {
                            NotFound();
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
            else
            {
                BadRequest();
            }

            return new { Id = model.Id };
        }
    }
}
