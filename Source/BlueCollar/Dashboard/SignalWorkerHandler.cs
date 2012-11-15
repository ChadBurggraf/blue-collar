//-----------------------------------------------------------------------
// <copyright file="SignalWorkerHandler.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Dashboard
{
    using System;
    using System.Collections.Generic;
    using System.Web;

    /// <summary>
    /// Implements the signal worker handler.
    /// </summary>
    public class SignalWorkerHandler : JsonHandler<WorkerSignalRecord>
    {
        private long? id;

        /// <summary>
        /// Initializes a new instance of the SignalWorkerHandler class.
        /// </summary>
        /// <param name="repositoryFactory">The repository factory to use.</param>
        public SignalWorkerHandler(IRepositoryFactory repositoryFactory)
            : base(repositoryFactory)
        {
        }

        /// <summary>
        /// Gets the ID of the worker to signal.
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
        /// Gets a value indicating whether the model loaded by <see cref="JsonHandler{T}.GetModel(HttpRequestBase)"/>
        /// is valid.
        /// </summary>
        /// <param name="model">The model to check the validity of.</param>
        /// <param name="results">The results of the validation.</param>
        /// <returns>True if the model passed validation, false otherwise.</returns>
        protected override bool IsValid(WorkerSignalRecord model, out IEnumerable<ValidationResult> results)
        {
            results = null;
            return true;
        }

        /// <summary>
        /// Performs the concrete request operation and returns the output
        /// as a byte array.
        /// </summary>
        /// <param name="context">The HTTP context to perform the request for.</param>
        /// <param name="model">The model passed in the request's content body.</param>
        /// <returns>The result of the request.</returns>
        protected override object PerformRequest(HttpContextBase context, WorkerSignalRecord model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model", "model cannot be null.");
            }

            if (this.Id > 0)
            {
                if (model.Signal == WorkerSignal.Start || model.Signal == WorkerSignal.Stop)
                {
                    bool acquired = false;

                    try
                    {
                        if (acquired = AcquireWorkerLock(this.Id))
                        {
                            WorkerRecord worker = Repository.GetWorker(this.Id);

                            if (worker != null && worker.ApplicationName == ApplicationName)
                            {
                                worker.Signal = model.Signal;
                                worker.UpdatedOn = DateTime.UtcNow;
                                Repository.UpdateWorker(worker);
                            }
                            else
                            {
                                NotFound();
                            }
                        }
                        else
                        {
                            InternalServerError();
                        }
                    }
                    finally
                    {
                        if (acquired)
                        {
                            Repository.ReleaseWorkerLock(this.Id);
                        }
                    }
                }
                else
                {
                    BadRequest();
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
