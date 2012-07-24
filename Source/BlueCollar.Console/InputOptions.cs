//-----------------------------------------------------------------------
// <copyright file="InputOptions.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Console
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using NDesk.Options;

    /// <summary>
    /// Represents a set of options passed as input to a BlueCollar.Console process.
    /// </summary>
    [CLSCompliant(false)]
    public sealed class InputOptions
    {
        private static readonly Regex PathQuotesExp = new Regex(@"^[""']?([^""']*)[""']?$", RegexOptions.Compiled);

        /// <summary>
        /// Initializes a new instance of the InputOptions class.
        /// </summary>
        public InputOptions()
        {
            this.Threshold = 500;
        }

        /// <summary>
        /// Gets the path to the application directory.
        /// </summary>
        public string ApplicationPath { get; private set; }

        /// <summary>
        /// Gets the path to the configuration file for the application, if applicable.
        /// </summary>
        public string ConfigPath { get; private set; }

        /// <summary>
        /// Gets a value indicating whether to display the help message.
        /// </summary>
        public bool Help { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is valid.
        /// </summary>
        public bool IsValid { get; private set; }

        /// <summary>
        /// Gets the <see cref="OptionSet"/> used to create this instance.
        /// </summary>
        public OptionSet OptionSet { get; private set; }

        /// <summary>
        /// Gets the ID of this process' parent process, if it was passed upon launch.
        /// </summary>
        public int ParentProcessId { get; private set; }

        /// <summary>
        /// Gets the error message that was set during creation, if applicable.
        /// </summary>
        public string ParseErrorMessage { get; private set; }

        /// <summary>
        /// Gets the parse exception that occurred during creation, if applicable.
        /// </summary>
        public OptionException ParseException { get; private set; }

        /// <summary>
        /// Gets the threshold, in milliseconds, to compress file system events into.
        /// </summary>
        public int Threshold { get; private set; }

        /// <summary>
        /// Gets a value indicating whether to write logging output to the console.
        /// </summary>
        public bool Verbose { get; private set; }

        /// <summary>
        /// Creates a new <see cref="InputOptions"/> instance by parsing the given collection of arguments.
        /// </summary>
        /// <param name="args">The collection of arguments to parse.</param>
        /// <returns>The created <see cref="InputOptions"/>.</returns>
        public static InputOptions Create(IEnumerable<string> args)
        {
            string app = null, config = null, thresh = null, pid = null;
            bool verbose = false, help = false;

            OptionSet options = new OptionSet()
            {
                { "app=", "(required) the path to the directory of the application to run jobs for.", v => app = v },
                { "config=", "the path to the configuration file to use, if not the default for the application.", v => config = v },
                { "v|verbose", "write logging information to standard out.", v => verbose = v != null },
                { "thresh=", "the threshold, in milliseconds, to compress file system events into.", v => thresh = v },
                { "h|?|help", "display usage help.", v => help = v != null }
            };

            InputOptions result = new InputOptions();
            result.OptionSet = options;

            try
            {
                options.Parse(args);
                result.ApplicationPath = PathQuotesExp.Replace(app ?? string.Empty, "$1");
                result.ConfigPath = PathQuotesExp.Replace(config ?? string.Empty, "$1");
                result.IsValid = true;
            }
            catch (OptionException ex)
            {
                result.IsValid = false;
                result.ParseException = ex;
            }

            if (result.IsValid)
            {
                result.Help = help;
                result.Verbose = verbose;

                if (!result.Help)
                {
                    if (string.IsNullOrEmpty(result.ApplicationPath))
                    {
                        result.ParseErrorMessage = "Application path is required.";
                        result.IsValid = false;
                    }

                    pid = Environment.GetEnvironmentVariable("COLLARSERVICEPID");

                    if (!string.IsNullOrEmpty(pid))
                    {
                        try
                        {
                            result.ParentProcessId = Convert.ToInt32(pid, CultureInfo.InvariantCulture);
                        }
                        catch (FormatException)
                        {
                        }
                        catch (OverflowException)
                        {
                        }
                    }

                    if (!string.IsNullOrEmpty(thresh))
                    {
                        const string ThresholdErrorMessage = "Threshold must be an integer value greater than or equal to 500.";

                        try
                        {
                            result.Threshold = Convert.ToInt32(thresh, CultureInfo.InvariantCulture);

                            if (result.Threshold < 500)
                            {
                                result.IsValid = false;
                                result.ParseErrorMessage = ThresholdErrorMessage;
                            }
                        }
                        catch (FormatException)
                        {
                            result.IsValid = false;
                            result.ParseErrorMessage = ThresholdErrorMessage;
                        }
                        catch (OverflowException)
                        {
                            result.IsValid = false;
                            result.ParseErrorMessage = ThresholdErrorMessage;
                        }
                    }
                }
            }

            return result;
        }
    }
}
