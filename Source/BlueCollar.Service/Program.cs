//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="Tasty Codes">
//     Copyright (c) 2012 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Service
{
    using System;
    using System.ServiceProcess;

    /// <summary>
    /// Program entry point.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Application entry point.
        /// </summary>
        /// <returns>The program's exit code.</returns>
        public static int Main()
        {
            ServiceBase.Run(
                new ServiceBase[]
                {
                    new Service()
                });

            return 0;
        }
    }
}