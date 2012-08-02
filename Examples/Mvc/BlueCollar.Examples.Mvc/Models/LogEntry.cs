namespace BlueCollar.Examples.Mvc.Models
{
    using System;
    using System.Globalization;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Represents an entry in a Blue Collar log file.
    /// </summary>
    public sealed class LogEntry
    {
        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the level.
        /// </summary>
        public string Level { get; set; }

        /// <summary>
        /// Gets or sets the message;
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Parses a line into a <see cref="LogEntry"/>.
        /// </summary>
        /// <param name="line">The line to parse.</param>
        /// <returns>The parsed <see cref="LogEntry"/>.</returns>
        public static LogEntry Parse(string line)
        {
            if (string.IsNullOrEmpty(line))
            {
                throw new ArgumentNullException("line", "line must contain a value.");
            }

            Match match = Regex.Match(line, @"^(\d{4}-\d{2}-\d{2}\s\d{2}:\d{2}:\d{2}\.[^\s]+)\s+-\s+([^\s]+)\s+-\s+(.*)$", RegexOptions.Singleline);

            if (match.Success)
            {
                return new LogEntry()
                {
                    Date = DateTime.Parse(match.Groups[1].Value.Trim()),
                    Level = match.Groups[2].Value.Trim(),
                    Message = match.Groups[3].Value.Trim()
                };
            }
            else
            {
                throw new FormatException(string.Format(CultureInfo.InvariantCulture, "The line '{0}' is not in the correct format to be parsed into a LogEntry.", line));
            }
        }
    }
}