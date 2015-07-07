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
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Web;

    /// <summary>
    /// Provides proxy access to a <see cref="Machine"/> instance across application domain boundaries.
    /// </summary>
    public sealed class MachineProxy : MarshalByRefObject, IDisposable
    {
        private List<HttpApplication> httpApplications = new List<HttpApplication>();
        private Machine machine;
        private bool disposed;
        
        /// <summary>
        /// Initializes a new instance of the MachineProxy class.
        /// </summary>
        /// <param name="logger">The logger to use when logging messages.</param>
        /// <param name="binPath">The path to use when probing for application assemblies.</param>
        public MachineProxy(ILogger logger, string binPath)
            : this(logger, binPath, BlueCollarSection.Section.Machine.ServiceExecutionEnabled)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MachineProxy class.
        /// </summary>
        /// <param name="logger">The logger to use when logging messages.</param>
        /// <param name="binPath">The path to use when probing for application assemblies.</param>
        /// <param name="enabled">A value indicating whether the machine is enabled.</param>
        public MachineProxy(ILogger logger, string binPath, bool enabled)
        {
            if (enabled)
            {
                this.SetupandInvokeEntryPoints(logger, !string.IsNullOrEmpty(binPath) ? binPath : AppDomain.CurrentDomain.BaseDirectory);
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
        /// Disposes all http applications that were created as entry points.
        /// </summary>
        private void DisposeHttpApplications()
        {
            foreach (HttpApplication app in this.httpApplications)
            {
                try
                {
                    MethodInfo method = HttpApplicationProbe.FindExitPoint(app.GetType());

                    if (method != null)
                    {
                        this.InvokeEventHandler(app, method);
                    }
                }
                finally
                {
                    app.Dispose();
                }
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

        /// <summary>
        /// Sets up and invokes entry points on <see cref="HttpApplication"/> implementors found
        /// by probing the given bin path.
        /// </summary>
        /// <param name="logger">The logger to use when probing.</param>
        /// <param name="binPath">The bin path to probe.</param>
        private void SetupandInvokeEntryPoints(ILogger logger, string binPath)
        {
            if (Directory.Exists(binPath))
            {
                HttpApplicationProbe probe = new HttpApplicationProbe(logger, binPath);
                IEnumerable<Type> types = HttpApplicationProbe.FindApplicationTypes(probe.FindApplicationAssemblies());

                foreach (Type type in types)
                {
                    HttpApplication app = (HttpApplication)Activator.CreateInstance(type);

                    try
                    {
                        MethodInfo entryPoint = HttpApplicationProbe.FindEntryPoint(type);

                        if (entryPoint != null)
                        {
                            this.InvokeEventHandler(app, entryPoint);
                        }

                        this.httpApplications.Add(app);
                        app = null;
                    }
                    finally
                    {
                        if (app != null)
                        {
                            app.Dispose();
                        }
                    }
                }
            }
        }
    }
}
