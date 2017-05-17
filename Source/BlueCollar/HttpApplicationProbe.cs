//-----------------------------------------------------------------------
// <copyright file="HttpApplicationProbe.cs" company="Tasty Codes">
//     Copyright (c) 2015 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Security;
    using System.Web;

    /// <summary>
    /// Provides probing functions for <see cref="HttpApplication"/> entry/exit
    /// points in a bin path. Assumes the bin path is in the current app domain's
    /// probing path.
    /// </summary>
    internal sealed class HttpApplicationProbe
    {
        private static readonly string[] EntryPoints = new[] { "Application_Start", "Application_OnStart" };
        private static readonly string[] ExitPoints = new[] { "Application_End", "Application_OnEnd" };

        /// <summary>
        /// Initializes a new instance of the HttpApplicationProbe class.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="binPath">The bin path to probe.</param>
        public HttpApplicationProbe(ILogger logger, string binPath)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger", "logger cannot be null.");
            }

            binPath = (binPath ?? string.Empty).Trim();

            if (string.IsNullOrEmpty(binPath))
            {
                binPath = ".";
            }

            this.Logger = logger;
            this.BinPath = Path.GetFullPath(binPath);
        }

        /// <summary>
        /// Gets the bin path being probed.
        /// </summary>
        public string BinPath { get; private set; }

        /// <summary>
        /// Gets the logger being used.
        /// </summary>
        public ILogger Logger { get; private set; }

        /// <summary>
        /// Finds all types in the given assembly collection that implement <see cref="HttpApplication"/>
        /// and contain an entry or exit point (or both).
        /// </summary>
        /// <param name="assemblies">The assemblies to find application types for.</param>
        /// <returns>A collection of <see cref="HttpApplication"/> types.</returns>
        public static IEnumerable<Type> FindApplicationTypes(IEnumerable<Assembly> assemblies)
        {
            List<Type> result = new List<Type>();

            foreach (Assembly assembly in assemblies ?? new Assembly[0])
            {
                foreach (Type type in assembly.GetTypes().Where(t => HttpApplicationProbe.IsHttpApplication(t)))
                {
                    if (HttpApplicationProbe.FindEntryPoint(type) != null
                        || HttpApplicationProbe.FindExitPoint(type) != null)
                    {
                        result.Add(type);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Finds a method acting as an <see cref="HttpApplication"/> entry point
        /// for the given type.
        /// </summary>
        /// <param name="type">The type to find the entry point for.</param>
        /// <returns>An entry point method, or null if none was found.</returns>
        public static MethodInfo FindEntryPoint(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type", "type cannot be null.");
            }

            return HttpApplicationProbe.FindEventHandler(type, HttpApplicationProbe.EntryPoints);
        }

        /// <summary>
        /// Finds a method acting as an <see cref="HttpApplication"/> exit point
        /// for the given type.
        /// </summary>
        /// <param name="type">The type to find the exit point for.</param>
        /// <returns>An exit point method, or null if none was found.</returns>
        public static MethodInfo FindExitPoint(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type", "type cannot be null.");
            }

            return HttpApplicationProbe.FindEventHandler(type, HttpApplicationProbe.ExitPoints);
        }

        /// <summary>
        /// Finds <see cref="Assembly"/> instances in the current bin path
        /// that contain types that implement <see cref="HttpApplication"/>.
        /// </summary>
        /// <returns>A collection of <see cref="Assembly"/> instances.</returns>
        public IEnumerable<Assembly> FindApplicationAssemblies()
        {
            List<Assembly> result = new List<Assembly>();
            string[] assemblyPaths = null;

            try
            {
                assemblyPaths = Directory.GetFiles(this.BinPath, "*.dll");
            }
            catch (IOException ex)
            {
                this.Logger.Error(ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                this.Logger.Error(ex);
            }

            if (assemblyPaths != null)
            {
                foreach (string assemblyPath in assemblyPaths)
                {
                    AssemblyName assemblyName = null;
                    Assembly assembly = null;
                    
                    try
                    {
                        assemblyName = AssemblyName.GetAssemblyName(assemblyPath);
                    }
                    catch (Exception ex)
                    {
                        this.Logger.Error(ex);
                    }

                    if (assemblyName != null
                        && !assemblyName.FullName.StartsWith("System.", StringComparison.OrdinalIgnoreCase)
                        && !assemblyName.FullName.StartsWith("Microsoft.", StringComparison.OrdinalIgnoreCase))
                    {
                        try
                        {
                            assembly = Assembly.Load(assemblyName);
                        }
                        catch (ArgumentException ex)
                        {
                            this.Logger.Error(ex);
                        }
                        catch (BadImageFormatException ex)
                        {
                            this.Logger.Error(ex);
                        }
                        catch (IOException ex)
                        {
                            this.Logger.Error(ex);
                        }
                        catch (ReflectionTypeLoadException ex)
                        {
                            this.Logger.Error(ex);
                        }
                        catch (SecurityException ex)
                        {
                            this.Logger.Error(ex);
                        }
                    }

                    try
                    {
                        if (assembly != null && assembly.GetTypes().Any(t => HttpApplicationProbe.IsHttpApplication(t)))
                        {
                            result.Add(assembly);
                        }
                    }
                    catch (ReflectionTypeLoadException ex)
                    {
                        try
                        {
                            this.Logger.Error(ex);
                        }
                        catch
                        {
                            // This can fail for various reasons...
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Finds a method that looks like an event handler on the given
        /// type using the given name collection as a whitelist.
        /// </summary>
        /// <param name="type">The type to find the event handler on.</param>
        /// <param name="names">A collection of method names to whitelist against.</param>
        /// <returns>An event handler method, or null if none was found.</returns>
        private static MethodInfo FindEventHandler(Type type, string[] names)
        {
            MethodInfo result = null;

            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => names.Any(n => n.Equals(m.Name, StringComparison.OrdinalIgnoreCase)))
                .Select(m => new { Args = m.GetParameters(), Method = m })
                .Where(obj => obj.Args.Length == 0 || obj.Args[0].ParameterType == typeof(object))
                .OrderByDescending(obj => obj.Args.Length);

            foreach (var method in methods)
            {
                if ((method.Args.Length == 2
                    && method.Args[1].ParameterType == typeof(EventArgs))
                    || method.Args.Length == 1
                    || method.Args.Length == 0)
                {
                    result = method.Method;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Gets a value indicating whether the given type implements <see cref="HttpApplication"/>.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>True if the type implements <see cref="HttpApplication"/>, false otherwise.</returns>
        private static bool IsHttpApplication(Type type)
        {
            return typeof(HttpApplication).IsAssignableFrom(type) && type != typeof(HttpApplication) && !type.IsAbstract;
        }
    }
}
