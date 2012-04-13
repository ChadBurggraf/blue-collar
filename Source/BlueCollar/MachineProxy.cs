//-----------------------------------------------------------------------
// <copyright file="MachineProxy.cs" company="Tasty Codes">
//     Copyright (c) 2012 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Provides proxy access to a <see cref="Machine"/> instance across application domain boundaries.
    /// </summary>
    public sealed class MachineProxy : MarshalByRefObject, IDisposable
    {
        private Machine machine;
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the MachineProxy class.
        /// </summary>
        /// <param name="logger">The logger to use when logging messages.</param>
        public MachineProxy(ILogger logger)
        {
            this.machine = new Machine(logger);
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
                        this.machine.Dispose(force);
                        this.machine = null;
                    }
                }

                this.disposed = true;
            }
        }
    }
}
