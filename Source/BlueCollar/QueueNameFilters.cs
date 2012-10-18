//-----------------------------------------------------------------------
// <copyright file="QueueNameFilters.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Represents inclusion and exclusion filters by queue name.
    /// </summary>
    public sealed class QueueNameFilters : IEquatable<QueueNameFilters>
    {
        /// <summary>
        /// Initializes a new instance of the QueueNameFilters class.
        /// </summary>
        /// <param name="queueNames">The queue names string to initialize this instance with.</param>
        public QueueNameFilters(string queueNames)
        {
            IEnumerable<string> i, e;
            Parse(queueNames, out i, out e);
            this.Include = new ReadOnlyCollection<string>(new List<string>(i));
            this.Exclude = new ReadOnlyCollection<string>(new List<string>(e));
        }

        /// <summary>
        /// Initializes a new instance of the QueueNameFilters class.
        /// </summary>
        /// <param name="include">A collection of queue name includes.</param>
        /// <param name="exclude">A collection of queue name excludes.</param>
        public QueueNameFilters(IEnumerable<string> include, IEnumerable<string> exclude)
        {
            this.Include = new ReadOnlyCollection<string>(new List<string>(NormalizeCollection(include)));
            this.Exclude = new ReadOnlyCollection<string>(new List<string>(NormalizeCollection(exclude)));
        }

        /// <summary>
        /// Gets the queue name exclude list. Will be empty if there are no excludes.
        /// </summary>
        public ReadOnlyCollection<string> Exclude { get; private set; }

        /// <summary>
        /// Gets the queue name include list. Will be empty if there are no includes; will contain
        /// only "*" if all queue names are included and there are no excludes.
        /// </summary>
        public ReadOnlyCollection<string> Include { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance indicates
        /// that no queue name filtering should take place; either by being
        /// empty or by containing "*" in the include list.
        /// </summary>
        public bool IncludesAllQueues
        {
            get { return this.IsEmpty || this.Include.Any(i => i == "*"); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is empty, which indicates
        /// no queue name filtering should take place.
        /// </summary>
        public bool IsEmpty
        {
            get { return this.Exclude.Count == 0 && this.Include.Count == 0; }
        }

        /// <summary>
        /// Creates a new <see cref="QueueNameFilters"/> instance that indicates no queue name filtering.
        /// </summary>
        /// <returns>A new <see cref="QueueNameFilters"/> instance.</returns>
        public static QueueNameFilters Any()
        {
            return new QueueNameFilters(null);
        }

        /// <summary>
        /// Parses the given queue names string into a new <see cref="QueueNameFilters"/> instance.
        /// </summary>
        /// <param name="queueNames">The queue names string to parse.</param>
        /// <returns>A new <see cref="QueueNameFilters"/> instance.</returns>
        public static QueueNameFilters Parse(string queueNames)
        {
            IEnumerable<string> i, e;
            Parse(queueNames, out i, out e);
            return new QueueNameFilters(i, e);
        }

        /// <summary>
        /// Parses the given queue names string into include and exclude collections.
        /// </summary>
        /// <param name="queueNames">The queue names string to parse.</param>
        /// <param name="includes">A collection of queue names to include.</param>
        /// <param name="excludes">A collection of queue names to exclude.</param>
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Justification = "An overload is provided without output parameters.")]
        public static void Parse(string queueNames, out IEnumerable<string> includes, out IEnumerable<string> excludes)
        {
            List<string> i = new List<string>();
            List<string> e = new List<string>();
            bool istar = false, estar = false;

            foreach (string q in SplitQueueNames(queueNames))
            {
                if (q.StartsWith("not:", StringComparison.OrdinalIgnoreCase))
                {
                    string n = q.Substring(4);

                    if (!string.IsNullOrEmpty(n))
                    {
                        e.Add(n);
                        estar = estar || n == "*";
                    }
                }
                else
                {
                    i.Add(q);
                    istar = istar || q == "*";
                }
            }

            if (istar)
            {
                i.Clear();
            }

            if (estar)
            {
                e.Clear();
            }

            if (i.Count == 0 && e.Count == 0 && istar)
            {
                i.Add("*");
            }

            includes = NormalizeCollection(i).ToArray();
            excludes = NormalizeCollection(e).ToArray();
        }

        /// <summary>
        /// Splits the given queue names string into a collection of include+exclude tokens.
        /// </summary>
        /// <param name="queueNames">The queue names string to split.</param>
        /// <returns>A collection of include + exclude tokens.</returns>
        public static IEnumerable<string> SplitQueueNames(string queueNames)
        {
            queueNames = (queueNames ?? string.Empty).Trim();

            if (!string.IsNullOrEmpty(queueNames))
            {
                string[] queues = (from qn in Regex.Split(queueNames, @"[^a-zA-Z0-9_\:*-]+")
                                   let tqn = qn.Trim()
                                   where !string.IsNullOrEmpty(tqn)
                                   select tqn).ToArray();

                return queues;
            }

            return new string[] { "*" };
        }

        /// <summary>
        /// Determines whether the given object is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to compare equality with.</param>
        /// <returns>True if the given object is equal to this instance, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as QueueNameFilters);
        }

        /// <summary>
        /// Gets a value indicating whether the given object is equal to this instance.
        /// </summary>
        /// <param name="other">The object to compare equality with.</param>
        /// <returns>True if the given object is equal to this instance, false otherwise.</returns>
        public bool Equals(QueueNameFilters other)
        {
            return other != null
                && other.Include.SequenceEqual(this.Include)
                && other.Exclude.SequenceEqual(this.Exclude);
        }

        /// <summary>
        /// Serves as the hash function for a particular type.
        /// </summary>
        /// <returns>A hash code.</returns>
        public override int GetHashCode()
        {
            return this.Include.GetHashCode() ^ this.Exclude.GetHashCode();
        }

        /// <summary>
        /// Gets a value indicating whether the given queue name is included by this instance.
        /// </summary>
        /// <param name="queueName">The queue name to check.</param>
        /// <returns>True if the queue name is included, false otherwise.</returns>
        public bool Includes(string queueName)
        {
            if (!this.IncludesAllQueues)
            {
                if (!string.IsNullOrEmpty(queueName))
                {
                    return this.Include.Contains(queueName, StringComparer.Ordinal)
                        && !this.Exclude.Contains(queueName, StringComparer.Ordinal);
                }
            }
            else
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            if (this.IncludesAllQueues)
            {
                return "*";
            }

            return string.Join("\n", this.Include.Concat(this.Exclude.Select(e => "not:" + e)).ToArray());
        }

        /// <summary>
        /// Normalizes the given collection of queue names into a reliable order and removes any duplicates.
        /// </summary>
        /// <param name="queueNames">The queue name collection to normalize.</param>
        /// <returns>The normalized collection.</returns>
        private static IEnumerable<string> NormalizeCollection(IEnumerable<string> queueNames)
        {
            return (queueNames ?? new string[0]).OrderBy(s => s, StringComparer.Ordinal).Distinct();
        }
    }
}
