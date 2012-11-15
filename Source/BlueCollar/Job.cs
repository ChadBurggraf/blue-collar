//-----------------------------------------------------------------------
// <copyright file="Job.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;
    using Newtonsoft.Json;

    /// <summary>
    /// Base <see cref="IJob"/> implementation.
    /// </summary>
    public abstract class Job : IJob
    {
        /// <summary>
        /// Gets the display name of the job.
        /// </summary>
        [JsonIgnore]
        public abstract string Name { get; }

        /// <summary>
        /// Gets the maximum number of retries for the job.
        /// Use 0 for infinite, -1 for none. Defaults to 3.
        /// </summary>
        [JsonIgnore]
        public virtual int Retries
        {
            get { return 3; }
        }

        /// <summary>
        /// Gets the maximum timeout, in milliseconds, the job is allowed to run in.
        /// Use 0 for infinite; defaults to 60,000 (1 minute).
        /// </summary>
        [JsonIgnore]
        public virtual int Timeout
        {
            get { return 60000; }
        }

        /// <summary>
        /// Enqueues the job using the currently configured application name on the default queue.
        /// </summary>
        public void Enqueue()
        {
            this.Enqueue(null);
        }

        /// <summary>
        /// Enqueues this job using the currently configured application name and the given queue name.
        /// </summary>
        /// <param name="queueName">The name of the queue to enqueue the job on, or null for the default queue.</param>
        public void Enqueue(string queueName)
        {
            this.Enqueue(BlueCollarSection.Section.ApplicationName, queueName);
        }

        /// <summary>
        /// Enqueues a job for the given application name and queue name.
        /// </summary>
        /// <param name="applicationName">The name of the application to enqueue the job for.</param>
        /// <param name="queueName">The name of the queue to enqueue the job on, or null for the default queue.</param>
        public void Enqueue(string applicationName, string queueName)
        {
            using (IRepository repository = new ConfigurationRepositoryFactory().Create())
            {
                this.Enqueue(applicationName, queueName, repository);
            }
        }

        /// <summary>
        /// Enqueues a job for the given application name and queue name.
        /// </summary>
        /// <param name="applicationName">The name of the application to enqueue the job for.</param>
        /// <param name="queueName">The name of the queue to enqueue the job on, or null for the default queue.</param>
        /// <param name="repository">The repository to use when enqueueing the job record.</param>
        public virtual void Enqueue(string applicationName, string queueName, IRepository repository)
        {
            if (string.IsNullOrEmpty(applicationName))
            {
                throw new ArgumentNullException("applicationName", "applicationName must contain a value.");
            }

            if (string.IsNullOrEmpty(queueName))
            {
                queueName = "*";
            }

            if (repository == null)
            {
                throw new ArgumentNullException("repository", "repository cannot be null.");
            }

            QueueRecord record = new QueueRecord()
            {
                ApplicationName = applicationName,
                Data = JobSerializer.Serialize(this),
                JobName = this.Name,
                JobType = JobSerializer.GetTypeName(this),
                QueuedOn = DateTime.UtcNow,
                QueueName = queueName,
                TryNumber = 1
            };

            repository.CreateQueued(record);
        }

        /// <summary>
        /// Executes the job.
        /// </summary>
        public abstract void Execute();
    }
}
