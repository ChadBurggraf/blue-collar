//-----------------------------------------------------------------------
// <copyright file="SaveScheduleHandler.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Dashboard
{
    using System;
    using System.Collections.Generic;
    using System.Web;

    /// <summary>
    /// Implements the save schedule handler.
    /// </summary>
    public sealed class SaveScheduleHandler : JsonHandler<ScheduleRecord>
    {
        /// <summary>
        /// Initializes a new instance of the SaveScheduleHandler class.
        /// </summary>
        /// <param name="repositoryFactory">The repository factory to use.</param>
        public SaveScheduleHandler(IRepositoryFactory repositoryFactory)
            : base(repositoryFactory)
        {
        }

        /// <summary>
        /// Gets a value indicating whether the model loaded by <see cref="JsonHandler{T}.GetModel(HttpRequestBase)"/>
        /// is valid.
        /// </summary>
        /// <param name="model">The model to check the validity of.</param>
        /// <param name="results">The results of the validation.</param>
        /// <returns>True if the model passed validation, false otherwise.</returns>
        protected override bool IsValid(ScheduleRecord model, out IEnumerable<ValidationResult> results)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model", "model cannot be null.");
            }

            List<ValidationResult> r = new List<ValidationResult>();
            DateTime minDate = new DateTime(1900, 1, 1);
            DateTime maxDate = new DateTime(2100, 1, 1);

            if (string.IsNullOrEmpty(model.Name))
            {
                r.Add(new ValidationResult() { ErrorMessage = "Name is required.", MemberName = "Name" });
            }

            if (!string.IsNullOrEmpty(model.Name) && model.Name.Length > 24)
            {
                r.Add(new ValidationResult() { ErrorMessage = "Name cannot be longer than 24 characters.", MemberName = "Name" });
            }

            if (!string.IsNullOrEmpty(model.QueueName) && model.QueueName.Length > 24) 
            {
                r.Add(new ValidationResult() { ErrorMessage = "Name cannot be longer than 24 characters.", MemberName = "QueueName" });
            }

            if (model.StartOn < minDate || model.StartOn > maxDate)
            {
                r.Add(new ValidationResult() { ErrorMessage = "Start on must be between 1900-01-01 and 2100-01-01.", MemberName = "StartOn" });
            }

            if (model.EndOn < minDate || model.EndOn > maxDate)
            {
                r.Add(new ValidationResult() { ErrorMessage = "End on must be between 1900-01-01 and 2100-01-01.", MemberName = "EndOn" });
            }

            if (model.EndOn <= model.StartOn)
            {
                r.Add(new ValidationResult() { ErrorMessage = "End on must be after start on when specified.", MemberName = "EndOn" });
            }

            if (model.RepeatType != ScheduleRepeatType.None && model.RepeatValue < 1)
            {
                r.Add(new ValidationResult() { ErrorMessage = "Repeat value must be greater than 0 when a repeat type other than 'None' is specified.", MemberName = "RepeatType" });
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
        protected override object PerformRequest(HttpContextBase context, ScheduleRecord model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model", "model cannot be null.");
            }

            model.ApplicationName = ApplicationName;
            bool acquired = false;

            try
            {
                if ("PUT".Equals(this.Verb, StringComparison.OrdinalIgnoreCase))
                {
                    model.Id = Helper.RouteIntValue(0);

                    if (model.Id != null && model.Id > 0)
                    {
                        if (acquired = AcquireScheduleLock(model.Id.Value))
                        {
                            model = Repository.UpdateSchedule(model);
                        }
                        else
                        {
                            InternalServerError();
                        }
                    }
                    else
                    {
                        BadRequest();
                    }
                }
                else
                {
                    model = Repository.CreateSchedule(model);
                }

                if (acquired || (acquired = AcquireScheduleLock(model.Id.Value)))
                {
                    Repository.SignalWorkers(ApplicationName, WorkerSignal.RefreshSchedules);
                }
                else
                {
                    InternalServerError();
                }
            }
            finally
            {
                if (acquired && model.Id != null && model.Id > 0)
                {
                    Repository.ReleaseScheduleLock(model.Id.Value);
                }
            }

            return new { Id = model.Id };
        }
    }
}
