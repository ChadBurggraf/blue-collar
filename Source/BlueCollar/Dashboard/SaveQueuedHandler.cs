//-----------------------------------------------------------------------
// <copyright file="SaveQueuedHandler.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Dashboard
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Web;

    /// <summary>
    /// Implements the save queued handler.
    /// </summary>
    public sealed class SaveQueuedHandler : JsonHandler<EnqueueJobRecord>
    {
        /// <summary>
        /// Initializes a new instance of the SaveQueuedHandler class.
        /// </summary>
        /// <param name="repositoryFactory">The repository factory to use.</param>
        public SaveQueuedHandler(IRepositoryFactory repositoryFactory)
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
        protected override bool IsValid(EnqueueJobRecord model, out IEnumerable<ValidationResult> results)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model", "model cannot be null.");
            }

            List<ValidationResult> r = new List<ValidationResult>();

            if (string.IsNullOrEmpty(model.JobType))
            {
                r.Add(new ValidationResult() { ErrorMessage = "Job type is required.", MemberName = "JobType" });
            }

            if (!string.IsNullOrEmpty(model.JobType) && model.JobType.Length > 256)
            {
                r.Add(new ValidationResult() { ErrorMessage = "Job type cannot be longer than 256 characters.", MemberName = "JobType" });
            }

            if (!string.IsNullOrEmpty(model.QueueName) && model.QueueName.Length > 24)
            {
                r.Add(new ValidationResult() { ErrorMessage = "Queue cannot be longer than 24 characters.", MemberName = "QueueName" });
            }

            model.Data = (model.Data ?? string.Empty).Trim();

            if (string.IsNullOrEmpty(model.Data))
            {
                model.Data = "{}";
            }

            IJob job = null;
            string error = null;

            try
            {
                job = JobSerializer.Deserialize(model.JobType, model.Data);
            }
            catch (ArgumentException)
            {
                error = "Job type contains invalid type syntax or does not implement IJob.";
            }
            catch (TargetInvocationException)
            {
                error = "Job type's class initializer threw an exception.";
            }
            catch (TypeLoadException)
            {
                error = "Failed to load job type.";
            }
            catch (FileNotFoundException)
            {
                error = "Job type or one of its dependencies was not found.";
            }
            catch (FileLoadException)
            {
                error = "Job type or one of its dependencies could not be loaded.";
            }
            catch (BadImageFormatException)
            {
                error = "Job type's assembly that could not be loaded into the current runtime.";
            }

            if (job != null)
            {
                model.JobName = job.Name;
            }
            else
            {
                r.Add(new ValidationResult() { ErrorMessage = error, MemberName = "JobType" });
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
        protected override object PerformRequest(HttpContextBase context, EnqueueJobRecord model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model", "model cannot be null.");
            }

            QueueRecord record = Repository.CreateQueued(
                new QueueRecord()
                {
                    ApplicationName = ApplicationName,
                    Data = model.Data,
                    JobName = model.JobName,
                    JobType = model.JobType,
                    QueuedOn = DateTime.UtcNow,
                    QueueName = model.QueueName,
                    TryNumber = 1
                }, 
                null);

            return new { Id = record.Id };
        }
    }
}
