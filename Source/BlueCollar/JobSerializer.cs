//-----------------------------------------------------------------------
// <copyright file="JobSerializer.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Performs serialization and de-serialization services for jobs.
    /// </summary>
    public static class JobSerializer
    {
        private static readonly JsonConverter[] Converters = new JsonConverter[] 
        {
            new IsoDateTimeConverter() { DateTimeStyles = DateTimeStyles.RoundtripKind }
        };

        /// <summary>
        /// De-serializes an <see cref="IJob"/> instance from the given type name and serialized data.
        /// </summary>
        /// <param name="typeName">The name of the type to create the <see cref="IJob"/> instance from.</param>
        /// <param name="data">The serialized job data to de-serialize.</param>
        /// <returns>The de-serialized <see cref="IJob"/> instance.</returns>
        public static IJob Deserialize(string typeName, string data)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                throw new ArgumentNullException("typeName", "typeName must contain a value.");
            }

            Type jobType = Type.GetType(typeName, true, true);

            if (typeof(IJob).IsAssignableFrom(jobType))
            {
                IJob job = !string.IsNullOrEmpty(data)
                    ? JsonConvert.DeserializeObject(data, jobType, Converters) as IJob
                    : CreateInstance(jobType) as IJob;

                if (job == null)
                {
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Failed to de-serialize '{0}' into an IJob instance.", typeName), "typeName");
                }

                return job;
            }
            else
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Type '{0}' does not implement IJob.", typeName), "typeName");
            }
        }

        /// <summary>
        /// Gets the given value's type name as a string.
        /// </summary>
        /// <param name="value">The value to get the type name of.</param>
        /// <returns>The value's type name.</returns>
        public static string GetTypeName(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value", "value cannot be null.");
            }

            return GetTypeName(value.GetType());
        }

        /// <summary>
        /// Gets the given type's name as a string.
        /// </summary>
        /// <param name="type">The type to get the name of.</param>
        /// <returns>The type's name.</returns>
        public static string GetTypeName(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type", "type cannot be null.");
            }

            return string.Concat(type.FullName, ", ", type.Assembly.GetName().Name);
        }

        /// <summary>
        /// Serializes the given <see cref="IJob"/> for storage.
        /// </summary>
        /// <param name="job">The <see cref="IJob"/> to serialize.</param>
        /// <returns>The serialized job data.</returns>
        public static string Serialize(IJob job)
        {
            if (job != null)
            {
                return JsonConvert.SerializeObject(job, Converters);
            }

            return string.Empty;
        }

        /// <summary>
        /// Creates an <see cref="IJob"/> instance from the given type.
        /// </summary>
        /// <param name="type">The job type to create the instance of.</param>
        /// <returns>An <see cref="IJob"/> instance.</returns>
        private static object CreateInstance(Type type)
        {
            return Activator.CreateInstance(type);
        }
    }
}
