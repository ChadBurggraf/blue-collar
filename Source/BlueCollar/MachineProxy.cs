//-----------------------------------------------------------------------
// <copyright file="MachineProxy.cs" company="Tasty Codes">
//     Copyright (c) 2012 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Web;

    /// <summary>
    /// Provides proxy access to a <see cref="Machine"/> instance across application domain boundaries.
    /// </summary>
    public sealed class MachineProxy : MarshalByRefObject, IDisposable
    {
        private static string[] assemblyBlackList = new string[] { "/^system/", "/^microsoft" };

        private Machine machine;
        private bool disposed;
        private List<object> webApplications = new List<object>();

        /// <summary>
        /// Initializes a new instance of the MachineProxy class.
        /// </summary>
        /// <param name="logger">The logger to use when logging messages.</param>
        public MachineProxy(ILogger logger)
            : this(logger, BlueCollarSection.Section.Machine.ServiceExecutionEnabled)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MachineProxy class.
        /// </summary>
        /// <param name="logger">The logger to use when logging messages.</param>
        /// <param name="enabled">A value indicating whether the machine is enabled.</param>
        public MachineProxy(ILogger logger, bool enabled)
        {
            if (enabled)
            {
                AppDomain.CurrentDomain.AssemblyLoad += this.AssemblyLoadEventHandler;

                this.machine = new Machine(logger);
            }
        }

        /// <summary>
        /// Finalizes an instance of the MachineProxy class.
        /// </summary>
        ~MachineProxy()
        {
            this.Dispose(true, false);
        }

        /// <summary>
        /// Disposes of resources used by this instance.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA1816:CallGCSuppressFinalizeCorrectly", Justification = "GC.SuppressFinalize(object) is called indirectly.")]
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Disposes of resources used by this instance.
        /// </summary>
        /// <param name="force">A value indicating whether to force workers to stop.</param>
        [SuppressMessage("Microsoft.Usage", "CA1816:CallGCSuppressFinalizeCorrectly", Justification = "This method is called by Dispose().")]
        public void Dispose(bool force)
        {
            this.Dispose(force, true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes of resources used by this instance.
        /// </summary>
        /// <param name="force">A value indicating whether to force workers to stop.</param>
        /// <param name="disposing">A value indicating whether to dispose of managed resources.</param>
        private void Dispose(bool force, bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (this.machine != null)
                    {
                        this.DisposeWebApplications();
                        this.machine.Dispose(force);

                        this.machine = null;
                    }
                }

                this.disposed = true;
            }
        }

        /// <summary>
        /// On assembly load, probe the assembly for a valid blue collar entry point and invoke it.
        /// </summary>
        /// <param name="sender">The event publisher.</param>
        /// <param name="args">The event.</param>
        private void AssemblyLoadEventHandler(object sender, AssemblyLoadEventArgs args)
        {
            Assembly assembly = args.LoadedAssembly;
            string assemblyName = assembly.GetName().Name;

            if (MachineProxy.assemblyBlackList.Any(b => Regex.Match(assemblyName, b).Length != 0))
            {
                return;
            }

            Type type = typeof(HttpApplication);
            string[] methods = { "Application_Start", "Application_OnStart" };

            IEnumerable<Type> types = assembly.GetTypes().Where(t => type.IsAssignableFrom(t) && t != type && t.IsAbstract == false);
            IEnumerable<object> apps = types.Select(t => Activator.CreateInstance(t));

            foreach (object app in apps)
            {
                MethodInfo method = app.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                    .Where(m => methods.Select(em => em.ToUpperInvariant()).Contains(m.Name.ToUpperInvariant()))
                    .FirstOrDefault();

                if (method != null)
                {
                    this.webApplications.Add(app);

                    method.Invoke(app, new object[] { this, EventArgs.Empty });
                }
            }
        }

        /// <summary>
        /// Disposes all web applications that were created as entry points.
        /// </summary>
        private void DisposeWebApplications()
        {
            string[] methods = { "Application_End", "Application_OnEnd" };

            foreach (object app in this.webApplications)
            {
                MethodInfo method = app.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                    .Where(m => methods.Select(em => em.ToUpperInvariant()).Contains(m.Name.ToUpperInvariant()))
                    .FirstOrDefault();

                if (method != null)
                {
                    method.Invoke(app, new object[] { this, EventArgs.Empty });
                }

                (app as HttpApplication).Dispose();
            }
        }
    }
}
