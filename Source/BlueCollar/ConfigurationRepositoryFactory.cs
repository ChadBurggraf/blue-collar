//-----------------------------------------------------------------------
// <copyright file="ConfigurationRepositoryFactory.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// Implements <see cref="IRepositoryFactory"/> to use the Blue Collar configuration section.
    /// </summary>
    public class ConfigurationRepositoryFactory : IRepositoryFactory
    {
        private bool initialized;
        private string defaultDataDirectory, repositoryTypeName, connectionStringName, connectionString;
        private Type repositoryType;
        private ConstructorInfo repositoryConstructor;

        /// <summary>
        /// Initializes a new instance of the ConfigurationRepositoryFactory class.
        /// </summary>
        public ConfigurationRepositoryFactory()
            : this(BlueCollarSection.Section.Repository.RepositoryType, BlueCollarSection.Section.Repository.ConnectionStringName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ConfigurationRepositoryFactory class.
        /// </summary>
        /// <param name="repositoryType">The type name of the repository use use from the configuration.</param>
        /// <param name="connectionStringName">The name of the connection string to use when initializing the repository from the configuration.</param>
        public ConfigurationRepositoryFactory(string repositoryType, string connectionStringName)
        {
            this.repositoryTypeName = repositoryType;
            this.connectionStringName = connectionStringName;
        }

        /// <summary>
        /// Gets the default directory to use for database files.
        /// </summary>
        protected virtual string DefaultDataDirectory
        {
            get
            {
                if (this.defaultDataDirectory == null)
                {
                    string dir = AppDomain.CurrentDomain.BaseDirectory;
                    string appData = Path.Combine(dir, "App_Data");

                    if (Directory.Exists(appData))
                    {
                        this.defaultDataDirectory = appData;
                    }
                    else
                    {
                        this.defaultDataDirectory = dir;
                    }
                }

                return this.defaultDataDirectory;
            }
        }

        /// <summary>
        /// Creates a new <see cref="IRepository"/> instance.
        /// </summary>
        /// <returns>An <see cref="IRepository"/> instance.</returns>
        [SuppressMessage("Microsoft.Naming", "CA2204:LiteralsShouldBeSpelledCorrectly", Justification = "The spelling is correct.")]
        public virtual IRepository Create()
        {
            this.EnsureInitialized();

            try
            {
                if (!string.IsNullOrEmpty(this.connectionString))
                {
                    return (IRepository)this.repositoryConstructor.Invoke(new object[] { this.connectionString });
                }
                else
                {
                    return (IRepository)this.repositoryConstructor.Invoke(null);
                }
            }
            catch (TargetInvocationException ex)
            {
                if (this.repositoryTypeName.StartsWith("BlueCollar.SQLiteRepository", StringComparison.Ordinal)
                    && ex.InnerException != null)
                {
                    if (typeof(FileLoadException) == ex.InnerException.GetType()
                        && ex.InnerException.Message.Contains("Mixed mode assembly"))
                    {
                        throw new InvalidOperationException("The System.Data.SQLite.dll being referenced is built against an un-supported version of the .NET runtime. Please reference a System.Data.SQLite.dll built against the runtime (.NET 3.5/4.0) and architecture (x86/x64) that matches your application.", ex);
                    }
                    else if (typeof(FileNotFoundException) == ex.InnerException.GetType())
                    {
                        throw new InvalidOperationException("You are trying to use BlueCollar.SQLiteRepository for storage, but System.Data.SQLite.dll could not be found. Please add a reference to the System.Data.SQLite.dll mixed mode assembly built against the runtime (.NET 3.5/4.0) and architecture (x86/x64) that matches your application.", ex);
                    }
                }

                throw;
            }
        }

        /// <summary>
        /// Resolves the given path to a fully qualified path.
        /// </summary>
        /// <param name="path">The path to resolve.</param>
        /// <returns>A fully qualified path.</returns>
        protected virtual string ResolvePath(string path)
        {
            return BlueCollarSection.Section.ResolvePath(path);
        }

        /// <summary>
        /// Ensures that this instance has been initializes, performing initialization if necessary.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "connectionStrings", Justification = "connectionStrings is the correct spelling.")]
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "appSettings", Justification = "appSettings is the correct spelling.")] 
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "IRepository", Justification = "IRepository is the correct spelling.")]
        private void EnsureInitialized()
        {
            lock (this)
            {
                if (!this.initialized)
                {
                    this.repositoryTypeName = !string.IsNullOrEmpty(this.repositoryTypeName) ? this.repositoryTypeName : "BlueCollar.SQLiteRepository, BlueCollar";

                    if (!string.IsNullOrEmpty(this.connectionStringName))
                    {
                        ConnectionStringSettings css = ConfigurationManager.ConnectionStrings[this.connectionStringName];

                        if (css != null && !string.IsNullOrEmpty(css.ConnectionString))
                        {
                            this.connectionString = css.ConnectionString;
                        }
                        else
                        {
                            this.connectionString = ConfigurationManager.AppSettings[this.connectionStringName];
                        }

                        if (string.IsNullOrEmpty(this.connectionString))
                        {
                            throw new ConfigurationErrorsException(
                                string.Format(CultureInfo.InvariantCulture, "Failed to find a connection string named '{0}' in either <connectionStrings/> or <appSettings/>.", this.connectionStringName),
                                BlueCollarSection.Section.ElementInformation.Source,
                                BlueCollarSection.Section.ElementInformation.LineNumber);
                        }
                    }
                    else if (this.repositoryTypeName.StartsWith("BlueCollar.SQLiteRepository", StringComparison.Ordinal))
                    {
                        string path = this.ResolvePath(Path.Combine(this.DefaultDataDirectory, "BlueCollar.sqlite"));
                        this.connectionString = string.Concat("data source=", path, ";journal mode=Off;synchronous=Off;version=3");
                    }

                    Exception typeException = null;

                    try
                    {
                        this.repositoryType = Type.GetType(this.repositoryTypeName, true, true);
                    }
                    catch (TargetInvocationException ex)
                    {
                        typeException = ex;
                    }
                    catch (TypeLoadException ex)
                    {
                        typeException = ex;
                    }
                    catch (ArgumentException ex)
                    {
                        typeException = ex;
                    }
                    catch (FileNotFoundException ex)
                    {
                        typeException = ex;
                    }
                    catch (FileLoadException ex)
                    {
                        typeException = ex;
                    }
                    catch (BadImageFormatException ex)
                    {
                        typeException = ex;
                    }

                    if (typeException != null)
                    {
                        throw new ConfigurationErrorsException(
                            string.Format(CultureInfo.InvariantCulture, "Failed to load type '{0}' configured as the IRepository type to use.", this.repositoryTypeName),
                            typeException,
                            BlueCollarSection.Section.ElementInformation.Source,
                            BlueCollarSection.Section.Repository.ElementInformation.LineNumber);
                    }

                    if (!typeof(IRepository).IsAssignableFrom(this.repositoryType))
                    {
                        throw new ConfigurationErrorsException(
                            string.Format(CultureInfo.InvariantCulture, "Type '{0}', configured as the IRepository type to use, does not implement IRepository.", this.repositoryTypeName),
                            BlueCollarSection.Section.ElementInformation.Source,
                            BlueCollarSection.Section.Repository.ElementInformation.LineNumber);
                    }

                    if (string.IsNullOrEmpty(this.connectionString))
                    {
                        this.repositoryConstructor = this.repositoryType.GetConstructor(BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, Type.EmptyTypes, null);

                        if (this.repositoryConstructor == null)
                        {
                            throw new ConfigurationErrorsException(
                                string.Format(CultureInfo.InvariantCulture, "Failed to find an empty constructor for IRepository type '{0}'.", this.repositoryTypeName),
                                BlueCollarSection.Section.ElementInformation.Source,
                                BlueCollarSection.Section.Repository.ElementInformation.LineNumber);
                        }
                    }
                    else
                    {
                        this.repositoryConstructor = this.repositoryType.GetConstructor(BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, new Type[] { typeof(string) }, null);

                        if (this.repositoryConstructor == null)
                        {
                            throw new ConfigurationErrorsException(
                                string.Format(CultureInfo.InvariantCulture, "Failed to find a constructor that takes a single string argument (the connection string) for IRepository type '{0}'.", this.repositoryTypeName),
                                BlueCollarSection.Section.ElementInformation.Source,
                                BlueCollarSection.Section.Repository.ElementInformation.LineNumber);
                        }
                    }

                    this.initialized = true;
                }
            }
        }
    }
}
