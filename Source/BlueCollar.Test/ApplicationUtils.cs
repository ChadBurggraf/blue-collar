//-----------------------------------------------------------------------
// <copyright file="ApplicationUtils.cs" company="Tasty Codes">
//     Copyright (c) 2012 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Test
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Provides utilities and helpers for application testing.
    /// </summary>
    internal static class ApplicationUtils
    {
        /// <summary>
        /// Copies the embedded resource with the given name to the given path on disk.
        /// </summary>
        /// <param name="name">The name of the resource file top copy.</param>
        /// <param name="path">The path to copy the file to.</param>
        public static void CopyEmbeddedResourceToPath(string name, string path)
        {
            using (Stream inputStream = typeof(ApplicationUtils).Assembly.GetManifestResourceStream("BlueCollar.Test." + name))
            {
                using (Stream outputStream = File.Create(path))
                {
                    byte[] buffer = new byte[4096];
                    int count = 0;

                    while (0 < (count = inputStream.Read(buffer, 0, buffer.Length)))
                    {
                        outputStream.Write(buffer, 0, count);
                    }
                }
            }
        }

        /// <summary>
        /// Creates a valid example application structure in a random destination directory
        /// and returns the path to the directory.
        /// </summary>
        /// <returns>The path to the created valid example application directory.</returns>
        public static string CreateValidExampleApplication()
        {
            string path = Path.GetFullPath(Path.GetRandomFileName().Replace(".", string.Empty));

            IList<string> assemblies = GetAssemblyPathsForBasicApplication();
            assemblies.Add(Path.GetFullPath("BlueCollar.Examples.dll"));

            Directory.CreateDirectory(path);

            foreach (string assembly in assemblies)
            {
                File.Copy(assembly, Path.Combine(path, Path.GetFileName(assembly)));
            }

            return path;
        }

        /// <summary>
        /// Creates a valid example web application structure in a random directory
        /// and returns the path to the directory.
        /// </summary>
        /// <returns>The path to the created valid example web application directory.</returns>
        public static string CreateValidExampleWebApplication()
        {
            string path = Path.GetFullPath(Path.GetRandomFileName().Replace(".", string.Empty));
            string binPath = Path.Combine(path, "bin");

            IList<string> assemblies = GetAssemblyPathsForBasicApplication();
            assemblies.Add(Path.GetFullPath("BlueCollar.Examples.dll"));

            Directory.CreateDirectory(path);
            Directory.CreateDirectory(binPath);
            Directory.CreateDirectory(Path.Combine(path, "App_Data"));

            foreach (string assembly in assemblies)
            {
                File.Copy(assembly, Path.Combine(binPath, Path.GetFileName(assembly)));
            }

            CopyEmbeddedResourceToPath("Web.config", Path.Combine(path, "Web.config"));

            return path;
        }

        /// <summary>
        /// Gets a list of fully-qualified paths for the minimum required input assemblies
        /// for a valid Blue Collar application.
        /// </summary>
        /// <returns>A list of fully-qualified paths.</returns>
        public static IList<string> GetAssemblyPathsForBasicApplication()
        {
            return new List<string>(
                new string[] 
                {
                    Path.GetFullPath("BlueCollar.dll"),
                    Path.GetFullPath("BlueCollar.SQLiteRepository.dll"),
                    Path.GetFullPath("Dapper.dll"),
                    Path.GetFullPath("Newtonsoft.Json.dll"),
                    Path.GetFullPath("NLog.dll")
                });
        }
    }
}
