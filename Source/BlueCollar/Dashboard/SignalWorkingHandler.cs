//-----------------------------------------------------------------------
// <copyright file="SignalWorkingHandler.cs" company="Tasty Codes">
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
    /// Implements the signal working handler.
    /// </summary>
    public class SignalWorkingHandler : JsonHandler<WorkingSignalRecord>
    {
        private long? id;

        /// <summary>
        /// Initializes a new instance of the SignalWorkingHandler class.
        /// </summary>
        /// <param name="repositoryFactory">The repository factory to use.</param>
        public SignalWorkingHandler(IRepositoryFactory repositoryFactory)
            : base(repositoryFactory)
        {
        }

        /// <summary>
        /// Gets the ID of the working job to signal.
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
        protected override bool IsValid(WorkingSignalRecord model, out IEnumerable<ValidationResult> results)
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
        protected override object PerformRequest(HttpContextBase context, WorkingSignalRecord model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model", "model cannot be null.");
            }

            if (this.Id > 0)
            {
                if (model.Signal == WorkingSignal.Cancel)
                {
                    using (IDbTransaction transaction = Repository.BeginTransaction())
                    {
                        try
                        {
                            WorkingRecord working = Repository.GetWorking(this.Id, transaction);

                            if (working != null && working.ApplicationName == ApplicationName)
                            {
                                working.Signal = model.Signal;
                                Repository.UpdateWorking(working, transaction);
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
            }
            else
            {
                BadRequest();
            }

            return null;
        }
    }
}