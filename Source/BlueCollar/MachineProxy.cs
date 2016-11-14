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
        private readonly ILogger logger;
        private HttpApplication httpApplication;
        private Machine machine;
        private bool disposed;
        
        /// <summary>
        /// Initializes a new instance of the MachineProxy class.
        /// </summary>
        /// <param name="logger">The logger to use when logging messages.</param>
        /// <param name="binPath">The path to use when probing for application assemblies.</param>
        public MachineProxy(ILogger logger, string binPath)
            : this(logger, binPath, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MachineProxy class.
        /// </summary>
        /// <param name="logger">The logger to use when logging messages.</param>
        /// <param name="binPath">The path to use when probing for application assemblies.</param>
        /// <param name="force">A value indicating whether to force the machine, even if <see cref="MachineElement.ServiceExecutionEnabled"/> is false.</param>
        public MachineProxy(ILogger logger, string binPath, bool force)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger", "logger cannot be null.");
            }

            this.logger = logger;

            if (force || BlueCollarSection.Section.Machine.ServiceExecutionEnabled)
            {
                this.SetupandInvokeEntryPoint(logger, !string.IsNullOrEmpty(binPath) ? binPath : AppDomain.CurrentDomain.BaseDirectory);
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
                    this.DisposeHttpApplication();

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
        /// Disposes of the HTTP applications that was created as an entry point.
        /// </summary>
        private void DisposeHttpApplication()
        {
            HttpApplication httpApplication = this.httpApplication;
            this.httpApplication = null;

            if (httpApplication != null)
            {
                try
                {
                    MethodInfo method = HttpApplicationProbe.FindExitPoint(httpApplication.GetType());

                    if (method != null)
                    {
                        this.logger.Info("Invoking exit point for HTTP application {0}.", httpApplication.GetType().FullName);
                        this.InvokeEventHandler(httpApplication, method);
                    }
                    else
                    {
                        this.logger.Info("No exit point found for HTTP application {0}.", httpApplication.GetType().FullName);
                    }
                }
                finally
                {
                    httpApplication.Dispose();
                }
            }
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
        /// Sets up and invokes an entry point on an <see cref="HttpApplication"/> implementer found
        /// by probing the given bin path.
        /// </summary>
        /// <param name="logger">The logger to use when probing.</param>
        /// <param name="binPath">The bin path to probe.</param>
        private void SetupandInvokeEntryPoint(ILogger logger, string binPath)
        {
            if (Directory.Exists(binPath))
            {
                HttpApplicationProbe probe = new HttpApplicationProbe(logger, binPath);
                Type type = HttpApplicationProbe.FindApplicationTypes(probe.FindApplicationAssemblies()).FirstOrDefault();

                if (type != null)
                {
                    logger.Info("Found an HTTP application requiring startup: {0}.", type.FullName);
                    HttpApplication httpApplication = null;

                    try
                    {
                        httpApplication = (HttpApplication)Activator.CreateInstance(type);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex, "Failed to create an instance of the HTTP application {0}.", type.FullName);
                    }

                    if (httpApplication != null)
                    {
                        try
                        {
                            MethodInfo entryPoint = HttpApplicationProbe.FindEntryPoint(type);

                            if (entryPoint != null)
                            {
                                logger.Info("Invoking entry point for HTTP application {0}.", type.FullName);
                                this.InvokeEventHandler(httpApplication, entryPoint);
                            }
                            else
                            {
                                logger.Info("No entry point found for HTTP application {0}.", type.FullName);
                            }

                            this.httpApplication = httpApplication;
                            httpApplication = null;
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex, "Failed to invoke the entry point for HTTP application {0}.", type.FullName);

                            if (ex.InnerException != null)
                            {
                                logger.Error(ex.InnerException);
                            }
                        }
                        finally
                        {
                            if (httpApplication != null)
                            {
                                httpApplication.Dispose();
                            }
                        }
                    }
                }
            }
        }
    }
}
