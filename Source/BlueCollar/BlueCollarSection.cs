//-----------------------------------------------------------------------
// <copyright file="BlueCollarSection.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Implements the Blue Collar configuration section.
    /// </summary>
    public sealed class BlueCollarSection : ConfigurationSection
    {
        private static readonly BlueCollarSection section = (BlueCollarSection)(ConfigurationManager.GetSection("blueCollar") ?? new BlueCollarSection());

        /// <summary>
        /// Gets the current configuration.
        /// </summary>
        public static BlueCollarSection Section
        {
            get { return section; }
        }

        /// <summary>
        /// Gets the name of the application the configuration represents.
        /// </summary>
        [ConfigurationProperty("applicationName", IsRequired = false, DefaultValue = "Default")]
        public string ApplicationName
        {
            get { return (string)this["applicationName"]; }
        }

        /// <summary>
        /// Gets the dashboard configuration.
        /// </summary>
        [ConfigurationProperty("dashboard", IsRequired = false)]
        public DashboardElement Dashboard
        {
            get { return (DashboardElement)(this["dashboard"] ?? (this["dashboard"] = new DashboardElement())); }
        }

        /// <summary>
        /// Gets the machine configuration.
        /// </summary>
        [ConfigurationProperty("machine", IsRequired = false)]
        public MachineElement Machine
        {
            get { return (MachineElement)(this["machine"] ?? (this["machine"] = new MachineElement())); }
        }

        /// <summary>
        /// Gets the repository configuration.
        /// </summary>
        [ConfigurationProperty("repository", IsRequired = false)]
        public RepositoryElement Repository
        {
            get { return (RepositoryElement)(this["repository"] ?? (this["repository"] = new RepositoryElement())); }
        }

        /// <summary>
        /// Gets the default worker heartbeat, in seconds.
        /// </summary>
        [ConfigurationProperty("workerHeartbeat", IsRequired = false, DefaultValue = 5)]
        public int WorkerHeartbeat
        {
            get { return (int)this["workerHeartbeat"]; }
        }

        /// <summary>
        /// Resolves the given path relative to this instance's configuration file.
        /// </summary>
        /// <param name="path">The path to resolve.</param>
        /// <returns>The resolved path.</returns>
        public string ResolvePath(string path)
        {
            const string DataDirectory = "|DataDirectory|";
            path = (path ?? string.Empty).Trim();

            if (!string.IsNullOrEmpty(path))
            {
                int dataDirectoryIndex = path.IndexOf(DataDirectory, StringComparison.OrdinalIgnoreCase);

                if (dataDirectoryIndex > -1)
                {
                    path = path.Substring(dataDirectoryIndex + DataDirectory.Length);
                    path = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data"), path);
                }
                else
                {
                    Regex exp = new Regex(string.Concat("[", Regex.Escape(new string(Path.GetInvalidPathChars())), "]"));

                    if (!exp.IsMatch(path) && !Path.IsPathRooted(path))
                    {
                        string dir = ElementInformation.Source;
                        dir = !string.IsNullOrEmpty(dir) ? Path.GetDirectoryName(dir) : null;

                        if (!string.IsNullOrEmpty(dir))
                        {
                            path = Path.Combine(dir, path);
                        }
                        else
                        {
                            path = Path.GetFullPath(path);
                        }
                    }
                }
            }
            else
            {
                path = AppDomain.CurrentDomain.BaseDirectory;
            }

            return path;
        }
    }
}
