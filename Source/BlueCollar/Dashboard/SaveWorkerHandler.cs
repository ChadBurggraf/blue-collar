//-----------------------------------------------------------------------
// <copyright file="SaveWorkerHandler.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Dashboard
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// Implements the save worker handler.
    /// </summary>
    public sealed class SaveWorkerHandler : JsonHandler<WorkerRecord>
    {
        /// <summary>
        /// Initializes a new instance of the SaveWorkerHandler class.
        /// </summary>
        /// <param name="repositoryFactory">The repository factory to use.</param>
        public SaveWorkerHandler(IRepositoryFactory repositoryFactory)
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
        protected override bool IsValid(WorkerRecord model, out IEnumerable<ValidationResult> results)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model", "model cannot be null.");
            }

            List<ValidationResult> r = new List<ValidationResult>();

            if (!string.IsNullOrEmpty(model.MachineAddress) && model.MachineAddress.Length > 64)
            {
                r.Add(new ValidationResult() { ErrorMessage = "Machine address cannot be longer than 64 characters.", MemberName = "MachineAddress" });
            }

            if (!string.IsNullOrEmpty(model.MachineName) && model.MachineName.Length > 128)
            {
                r.Add(new ValidationResult() { ErrorMessage = "Machine name cannot be longer than 128 characters.", MemberName = "MachineName" });
            }

            if (string.IsNullOrEmpty(model.MachineAddress) && string.IsNullOrEmpty(model.MachineName))
            {
                r.Add(new ValidationResult() { ErrorMessage = "Machine name, machine address, or both must be specified.", MemberName = "Machine" });
            }

            if (string.IsNullOrEmpty(model.Name))
            {
                r.Add(new ValidationResult() { ErrorMessage = "Name is required.", MemberName = "Name" });
            }

            if (!string.IsNullOrEmpty(model.Name) && model.Name.Length > 64)
            {
                r.Add(new ValidationResult() { ErrorMessage = "Name cannot be longer than 64 characters.", MemberName = "Name" });
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
        protected override object PerformRequest(HttpContextBase context, WorkerRecord model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model", "model cannot be null.");
            }

            model.ApplicationName = ApplicationName;
            model.QueueNames = QueueNameFilters.Parse(model.QueueNames).ToString();
            model.UpdatedOn = DateTime.UtcNow;

            bool acquired = false;

            try
            {
                if ("PUT".Equals(this.Verb, StringComparison.OrdinalIgnoreCase))
                {
                    model.Id = Helper.RouteIntValue(0);

                    if (model.Id != null && model.Id > 0)
                    {
                        if (acquired = AcquireWorkerLock(model.Id.Value))
                        {
                            WorkerRecord existing = Repository.GetWorker(model.Id.Value);
                            model.Signal = existing.Signal;
                            model.Status = existing.Status;

                            model = Repository.UpdateWorker(model);
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
                    model = Repository.CreateWorker(model);
                }
            }
            finally
            {
                if (acquired && model.Id != null && model.Id > 0)
                {
                    Repository.ReleaseWorkerLock(model.Id.Value);
                }
            }

            return new { Id = model.Id };
        }
    }
}
