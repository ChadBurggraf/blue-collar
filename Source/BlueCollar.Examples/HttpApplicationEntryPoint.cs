//-----------------------------------------------------------------------
// <copyright file="HttpApplicationEntryPoint.cs" company="Tasty Codes">
//     Copyright (c) 2012 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Examples
{
    using System;
    using System.IO;
    using System.Text;
    using System.Web;

    /// <summary>
    /// A HttpApplication entry point.
    /// </summary>
    public class HttpApplicationEntryPoint : HttpApplication
    {
        /// <summary>
        /// Create a beacon file when application starts.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">Event arguments.</param>
        protected void Application_Start(object sender, EventArgs e)
        {
            File.AppendAllText("HttpApplicationStart", "Hello, from HttpApplicationEntryPoint!", Encoding.UTF8);
        }

        /// <summary>
        /// Create a beacon file when the application ends.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">Event arguments.</param>
        protected void Application_End(object sender, EventArgs e)
        {
            File.AppendAllText("HttpApplicationEnd", "Goodbye, from HttpApplicationEntryPoint!", Encoding.UTF8);
        }
    }
}
