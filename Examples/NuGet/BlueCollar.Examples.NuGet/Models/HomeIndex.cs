namespace BlueCollar.Examples.NuGet.Models
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Web.Mvc;
    using BlueCollar.Examples;

    /// <summary>
    /// Provides model data for Home/Index.
    /// </summary>
    public sealed class HomeIndex
    {
        private IList<LogEntry> entries;
        private string logPath;

        /// <summary>
        /// Gets the parsed list of log entries.
        /// </summary>
        public IList<LogEntry> Entries
        {
            get { return this.entries ?? (this.entries = new List<LogEntry>()); }
        }

        /// <summary>
        /// Gets or sets the path of the log file to read.
        /// </summary>
        public string LogPath
        {
            get
            {
                if (string.IsNullOrEmpty(this.logPath))
                {
                    this.logPath = Path.Combine("App_Data", "BlueCollar.log");
                }

                return this.logPath;
            }

            set
            {
                this.logPath = value;
            }
        }

        /// <summary>
        /// Gets the CSS class to use for the given log level text.
        /// </summary>
        /// <param name="level">The log level text to get the CSS class for.</param>
        /// <returns>A CSS class.</returns>
        public static string CssClassForLogLevel(string level)
        {
            level = (level ?? string.Empty).Trim().ToUpperInvariant();

            switch (level)
            {
                case "INFO":
                    return "label label-info";
                case "WARN":
                    return "label label-warning";
                case "ERROR":
                    return "label label-important";
                default:
                    return "label";
            }
        }

        /// <summary>
        /// Fills the model with data.
        /// </summary>
        /// <param name="modelState">The <see cref="ModelStateDictionary"/> to add errors to.</param>
        /// <param name="isPostBack">A value indicating whether the fill is taking place during a post-back.</param>
        /// <returns>True if the fill succeeded, false otherwise.</returns>
        public bool Fill(ModelStateDictionary modelState, bool isPostBack)
        {
            bool success = true;

            if (File.Exists(this.LogPath))
            {
                IList<string> tail = Tail.Read(this.LogPath, 100).ToList();
                IList<string> multiLine = new List<string>();
                IList<string> result = new List<string>();

                foreach (string line in tail)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        if (Regex.IsMatch(line, @"^\d{4}-\d{2}-\d{2}\s\d{2}:\d{2}:\d{2}\.[^\s]+"))
                        {
                            if (multiLine.Count > 0)
                            {
                                multiLine.Add(line.Trim());
                                result.Add(string.Join("\n", multiLine.Reverse().ToArray()));
                                multiLine.Clear();
                            }
                            else
                            {
                                result.Add(line.Trim());
                            }
                        }
                        else
                        {
                            multiLine.Add(line.Trim());
                        }
                    }
                }

                this.entries = result
                    .Select(l => LogEntry.Parse(l))
                    .ToList();
            }
            else
            {
                modelState.AddModelError(string.Empty, string.Format(CultureInfo.InvariantCulture, "There was no log file found at '{0}'.", this.LogPath));
                modelState.AddModelError(string.Empty, "Note that no log file will be created if running from the console application (Collar.exe), or the service application. Logs are maintained in those situations under the appropriate Console.exe directory instead.");
                success = false;
            }

            return success;
        }
    }
}