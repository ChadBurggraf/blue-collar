//-----------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;

    /// <summary>
    /// Provides extensions to built-in types.
    /// </summary>
    internal static class Extensions
    {
        /// <summary>
        /// Adds a range of items to the list.
        /// </summary>
        /// <typeparam name="T">The type of items in the list.</typeparam>
        /// <param name="list">The list to add to.</param>
        /// <param name="collection">The collection of items to add.</param>
        public static void AddRange<T>(this IList<T> list, IEnumerable<T> collection)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list", "list cannot be null.");
            }

            if (collection != null)
            {
                foreach (T item in collection)
                {
                    list.Add(item);
                }
            }
        }

        /// <summary>
        /// Converts a string value into an enum.
        /// </summary>
        /// <typeparam name="T">The type of the enum to convert.</typeparam>
        /// <param name="value">The string value to convert.</param>
        /// <returns>The converted enum value.</returns>
        public static T AsEnum<T>(this string value) where T : struct
        {
            if (!string.IsNullOrEmpty(value))
            {
                return (T)Enum.Parse(typeof(T), value, true);
            }

            return default(T);
        }

        /// <summary>
        /// Compares two <see cref="IDictionary{TKey, TValue}"/> instances for sequence equality.
        /// </summary>
        /// <typeparam name="TKey">The key type of the dictionaries to compare.</typeparam>
        /// <typeparam name="TValue">The value type of the dictionaries to compare.</typeparam>
        /// <param name="left">The left dictionary to compare.</param>
        /// <param name="right">The right dictionary to compare.</param>
        /// <returns>True if the dictionaries are sequence equal, false otherwise.</returns>
        public static bool DictionarySequenceEqual<TKey, TValue>(this IDictionary<TKey, TValue> left, IDictionary<TKey, TValue> right)
        {
            left = left ?? new Dictionary<TKey, TValue>();
            right = right ?? new Dictionary<TKey, TValue>();

            if (left.Keys.Count == right.Keys.Count)
            {
                foreach (TKey key in left.Keys)
                {
                    if (right.ContainsKey(key))
                    {
                        TValue leftValue = left[key], rightValue = right[key];

                        if ((leftValue != null && !leftValue.Equals(rightValue))
                            || (rightValue != null && !rightValue.Equals(leftValue)))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the floor value of the given DateTime with seconds precision.
        /// </summary>
        /// <param name="value">The value to get the floor value of.</param>
        /// <returns>The floor value.</returns>
        public static DateTime FloorWithSeconds(this DateTime value)
        {
            return ((DateTime?)value).FloorWithSeconds().Value;
        }

        /// <summary>
        /// Gets the floor value of the given DateTime with seconds precision.
        /// </summary>
        /// <param name="value">The value to get the floor value of.</param>
        /// <returns>The floor value.</returns>
        public static DateTime? FloorWithSeconds(this DateTime? value)
        {
            if (value != null)
            {
                return new DateTime(value.Value.Ticks - (value.Value.Ticks % TimeSpan.TicksPerSecond), value.Value.Kind);
            }

            return value;
        }

        /// <summary>
        /// Invokes an action with a timeout, throwing a <see cref="TimeoutException"/>
        /// if the action takes longer than the duration specified.
        /// </summary>
        /// <param name="action">The action to invoke.</param>
        /// <param name="milliseconds">The maximum number of milliseconds to allow the action to take.</param>
        public static void InvokeWithTimeout(this Action action, int milliseconds)
        {
            action.InvokeWithTimeout(new TimeSpan(0, 0, 0, 0, milliseconds));
        }

        /// <summary>
        /// Invokes an action with a timeout, throwing a <see cref="TimeoutException"/>
        /// if the action takes longer than the duration specified.
        /// </summary>
        /// <param name="action">The action to invoke.</param>
        /// <param name="duration">The maximum duration to allow the action to take.</param>
        public static void InvokeWithTimeout(this Action action, TimeSpan duration)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action", "action cannot be null.");
            }

            if (duration.Ticks == 0)
            {
                throw new ArgumentException("duration must represent a time span greater than 0.", "duration");
            }

            object sync = new object();
            bool complete = false;

            WaitCallback callback =
                target =>
                {
                    Thread thread = target as Thread;

                    lock (sync)
                    {
                        if (!complete)
                        {
                            Monitor.Wait(sync, duration);
                        }
                    }

                    if (!complete)
                    {
                        thread.Abort();
                    }
                };

            try
            {
                ThreadPool.QueueUserWorkItem(callback, Thread.CurrentThread);
                action();
            }
            catch (ThreadAbortException)
            {
                Thread.ResetAbort();
                throw new TimeoutException();
            }
            finally
            {
                lock (sync)
                {
                    complete = true;
                    Monitor.Pulse(sync);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether type is a nullable type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>True if the type is a nullable type, false otherwise.</returns>
        public static bool IsNullable(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type", "type cannot be null.");
            }

            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary>
        /// Normalizes the given <see cref="DateTime"/> to UTC.
        /// </summary>
        /// <param name="value">The <see cref="DateTime"/> to normalize.</param>
        /// <returns>The normalized <see cref="DateTime"/>.</returns>
        public static DateTime NormalizeToUtc(this DateTime value)
        {
            return NormalizeToUtc(value as DateTime?).Value;
        }

        /// <summary>
        /// Normalizes the given <see cref="DateTime"/> to UTC.
        /// </summary>
        /// <param name="value">The <see cref="DateTime"/> to normalize.</param>
        /// <returns>The normalized <see cref="DateTime"/>.</returns>
        public static DateTime? NormalizeToUtc(this DateTime? value)
        {
            if (value != null)
            {
                switch (value.Value.Kind)
                {
                    case DateTimeKind.Local:
                        value = value.Value.ToUniversalTime();
                        break;
                    case DateTimeKind.Unspecified:
                        value = new DateTime(value.Value.Ticks, DateTimeKind.Utc);
                        break;
                    case DateTimeKind.Utc:
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            return value;
        }

        /// <summary>
        /// Reads the contents of the embedded resource with the given name as a string.
        /// </summary>
        /// <param name="resourceName">The name of the resource to read.</param>
        /// <returns>The contents of the resource.</returns>
        public static string ReadResourceContents(string resourceName)
        {
            Stream stream = null;

            try
            {
                stream = typeof(Extensions).Assembly.GetManifestResourceStream(string.Concat("BlueCollar.", resourceName));

                using (StreamReader reader = new StreamReader(stream))
                {
                    stream = null;
                    return reader.ReadToEnd();
                }
            }
            finally
            {
                if (stream != null)
                {
                    stream.Dispose();
                }
            }
        }

        /// <summary>
        /// Converts the camelCase or PascalCase string to a lower-case string with a separator 
        /// (e.g., camel_case or pascal-case, depending on the separator).
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <param name="separator">The separator to use.</param>
        /// <returns>The converted string.</returns>
        public static string ToLowercaseWithSeparator(this string value, char separator)
        {
            value = (value ?? string.Empty).Trim();

            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            StringBuilder sb = new StringBuilder();
            int i = 0;
            int wordLetterNumber = 0;
            bool prevUpper = false;

            while (i < value.Length)
            {
                if (char.IsLetterOrDigit(value, i))
                {
                    wordLetterNumber++;
                }
                else
                {
                    wordLetterNumber = 0;
                }

                if (char.IsUpper(value, i))
                {
                    if (wordLetterNumber > 1 && !prevUpper)
                    {
                        sb.Append(separator);
                    }

                    sb.Append(char.ToLowerInvariant(value[i]));
                    prevUpper = true;
                }
                else
                {
                    sb.Append(value[i]);
                    prevUpper = false;
                }

                i++;
            }

            return sb.ToString();
        }
    }
}
