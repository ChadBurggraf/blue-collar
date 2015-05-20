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
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Web;

    /// <summary>
    /// Provides proxy access to a <see cref="Machine"/> instance across application domain boundaries.
    /// </summary>
    public sealed class MachineProxy : MarshalByRefObject, IDisposable
    {
        private static readonly string[] AssemblyBlacklist = new string[] { "System.", "Microsoft." };
        private static readonly string[] HttpApplicationEntryPoints = new[] { "Application_Start", "Application_OnStart" };
        private static readonly string[] HttpApplicationExitPoints = new[] { "Application_End", "Application_OnEnd" };
        private List<HttpApplication> httpApplications = new List<HttpApplication>();
        private Machine machine;
        private bool disposed;
        
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
        /// Finds a method on the specified type that looks like an event handler.
        /// I.e., signatures include Method(), Method(object), or Method(object, EventArgs).
        /// </summary>
        /// <param name="type">The type to find the method in.</param>
        /// <param name="names">The names of methods to look for.</param>
        /// <returns>The first, most specific, method found that looks like an event handler.</returns>
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
        /// Disposes of resources used by this instance.
        /// </summary>
        /// <param name="force">A value indicating whether to force workers to stop.</param>
        /// <param name="disposing">A value indicating whether to dispose of managed resources.</param>
        private void Dispose(bool force, bool disposing)
        {
            if (!this.disposed)
            {
                AppDomain.CurrentDomain.AssemblyLoad -= this.AssemblyLoadEventHandler;

                if (disposing)
                {
                    this.DisposeHttpApplications();

                    Machine m = this.machine;
                    this.machine = null;

                    if (m != null)
                    {
                        m.Dispose(force);
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

            if (!MachineProxy.AssemblyBlacklist.Any(n => assemblyName.StartsWith(n, StringComparison.OrdinalIgnoreCase)))
            {
                IEnumerable<Type> types = assembly.GetTypes().Where(t => typeof(HttpApplication).IsAssignableFrom(t) && t != typeof(HttpApplication) && !t.IsAbstract);

                foreach (Type type in types)
                {
                    MethodInfo method = MachineProxy.FindEventHandler(type, MachineProxy.HttpApplicationEntryPoints);

                    if (method != null)
                    {
                        HttpApplication app = (HttpApplication)Activator.CreateInstance(type);
                        this.httpApplications.Add(app);
                        this.InvokeEventHandler(app, method);
                    }
                }
            }
        }

        /// <summary>
        /// Disposes all http applications that were created as entry points.
        /// </summary>
        private void DisposeHttpApplications()
        {
            foreach (HttpApplication app in this.httpApplications)
            {
                MethodInfo method = MachineProxy.FindEventHandler(app.GetType(), MachineProxy.HttpApplicationExitPoints);

                if (method != null)
                {
                    this.InvokeEventHandler(app, method);
                }

                app.Dispose();
            }

            this.httpApplications.Clear();
        }

        /// <summary>
        /// Invokes the event handler identified by the given method on the given instance.
        /// </summary>
        /// <param name="instance">The instance to invoke the event handler on.</param>
        /// <param name="method">The method identifying the event handler to invoke.</param>
        private void InvokeEventHandler(object instance, MethodInfo method)
        {
            ParameterInfo[] args = method.GetParameters();

            switch (args.Length)
            {
                case 2:
                    method.Invoke(instance, new object[] { this, new EventArgs() });
                    break;
                case 1:
                    method.Invoke(instance, new object[] { this });
                    break;
                case 0:
                    method.Invoke(instance, null);
                    break;
                default:
                    throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "The specified method was expected to have 0, 1, or 2 parameters: {0}", method.ToString()));
            }
        }
    }
}
