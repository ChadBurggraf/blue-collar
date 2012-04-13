//-----------------------------------------------------------------------
// <copyright file="JsonHandler{T}.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Dashboard
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Web;
    using Newtonsoft.Json;

    /// <summary>
    /// Generic implementation of <see cref="JsonHandler"/> for requests
    /// expecting a JSON object in the body.
    /// </summary>
    /// <typeparam name="T">The model type expected in the request body.</typeparam>
    public abstract class JsonHandler<T> : JsonHandler
    {
        /// <summary>
        /// Initializes a new instance of the JsonHandler class.
        /// </summary>
        /// <param name="repositoryFactory">The repository factory to use.</param>
        protected JsonHandler(IRepositoryFactory repositoryFactory)
            : base(repositoryFactory)
        {
        }

        /// <summary>
        /// Gets a value indicating whether the model loaded by <see cref="GetModel(HttpRequestBase)"/>
        /// is valid.
        /// </summary>
        /// <param name="model">The model to check the validity of.</param>
        /// <param name="errors">The error collection to add errors to.</param>
        /// <returns>True if the model is valid, false otherwise.</returns>
        protected virtual bool IsValid(T model, IDictionary<string, string> errors)
        {
            if (errors == null)
            {
                throw new ArgumentNullException("errors", "errors cannot be null.");
            }

            bool success;

            if (model != null)
            {
                IEnumerable<ValidationResult> results;
                success = this.IsValid(model, out results);

                if (!success)
                {
                    foreach (ValidationResult result in results)
                    {
                        if (errors.ContainsKey(result.MemberName))
                        {
                            errors[result.MemberName] += " " + result.ErrorMessage;
                        }
                        else
                        {
                            errors[result.MemberName] = result.ErrorMessage;
                        }
                    }
                }
            }
            else
            {
                errors["Model"] = "No data was submitted.";
                success = false;
            }

            return success;
        }

        /// <summary>
        /// Gets a value indicating whether the model loaded by <see cref="GetModel(HttpRequestBase)"/>
        /// is valid.
        /// </summary>
        /// <param name="model">The model to check the validity of.</param>
        /// <param name="results">The results of the validation.</param>
        /// <returns>True if the model passed validation, false otherwise.</returns>
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Justification = "This is the easiest design to work with right now.")]
        protected abstract bool IsValid(T model, out IEnumerable<ValidationResult> results);

        /// <summary>
        /// Gets the model passed into the given request's content body as a JSON string.
        /// </summary>
        /// <param name="request">The request to get the model for.</param>
        /// <returns>The request's de-serialized model.</returns>
        protected virtual T GetModel(HttpRequestBase request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request", "request cannot be null.");
            }

            T model;

            if (request.ContentLength > 0)
            {
                JsonSerializer serializer = new JsonSerializer();
                StreamReader sr = null;

                try
                {
                    sr = new StreamReader(request.InputStream);

                    using (JsonReader jr = new JsonTextReader(sr))
                    {
                        sr = null;
                        model = serializer.Deserialize<T>(jr);
                    }
                }
                finally
                {
                    if (sr != null)
                    {
                        sr.Dispose();
                    }
                }
            }
            else
            {
                model = default(T);
            }

            return model;
        }

        /// <summary>
        /// Performs the concrete request operation and returns the output
        /// as a byte array.
        /// </summary>
        /// <param name="context">The HTTP context to perform the request for.</param>
        /// <returns>The response to write.</returns>
        protected override byte[] PerformRequest(HttpContextBase context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context", "context cannot be null.");
            }

            T model = this.GetModel(context.Request);
            Dictionary<string, string> errors = new Dictionary<string, string>();
            byte[] output;

            if (this.IsValid(model, errors))
            {
                output = Json(this.PerformRequest(context, model));
            }
            else
            {
                context.Response.StatusCode = 400;
                output = Json(errors);
            }

            return output;
        }

        /// <summary>
        /// Performs the concrete request operation and returns the output
        /// as a byte array.
        /// </summary>
        /// <param name="context">The HTTP context to perform the request for.</param>
        /// <param name="model">The model passed in the request's content body.</param>
        /// <returns>The result of the request.</returns>
        protected abstract object PerformRequest(HttpContextBase context, T model);
    }
}
