namespace BlueCollar.Examples.Webpages
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Web.UI;

    /// <summary>
    /// Default page.
    /// </summary>
    public partial class _Default : Page
    {
        private IList<LogEntry> entries;
        private IList<string> errors;
        private string logPath;

        /// <summary>
        /// Gets the parsed list of log entries.
        /// </summary>
        public IList<LogEntry> Entries
        {
            get { return this.entries ?? (this.entries = new List<LogEntry>()); }
        }

        /// <summary>
        /// Gets a collection of errors encountered when loading page data.
        /// </summary>
        public IList<string> Errors
        {
            get { return this.errors ?? (this.errors = new List<string>()); }
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
                    this.logPath = Server.MapPath("~/App_Data/BlueCollar.log");
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
        /// Raises the page's Load event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnLoad(EventArgs e)
        {
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
                this.Errors.Add(string.Format(CultureInfo.InvariantCulture, "There was no log file found at '{0}'.", this.LogPath));
                this.Errors.Add("Note that no log file will be created if running from the console application (Collar.exe), or the service application. Logs are maintained in those situations under the appropriate Console.exe directory instead.");
            }

            base.OnLoad(e);
        }
    }
}
