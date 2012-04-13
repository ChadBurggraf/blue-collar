//-----------------------------------------------------------------------
// <copyright file="DashboardFactoryTests.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Test
{
    using System;
    using BlueCollar.Dashboard;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Dashboard factory tests.
    /// </summary>
    [TestClass]
    public sealed class DashboardFactoryTests
    {
        /// <summary>
        /// Resolve handler URL tests.
        /// </summary>
        [TestMethod]
        public void DashboardHandlerFactoryResolveHandlerUrl()
        {
            // Config-relative, standard config location.
            string url = DashboardHandlerFactory.ResolveHandlerUrl("collar", @"C:\Path\To\Application", "/bluecollar", @"C:\Path\To\Application\Web.config");
            Assert.AreEqual("/bluecollar/collar", url);

            // Config-relative, subdirectory config location.
            url = DashboardHandlerFactory.ResolveHandlerUrl("collar", @"C:\Path\To\Application", "/bluecollar", @"C:\Path\To\Application\Admin\Web.config");
            Assert.AreEqual("/bluecollar/Admin/collar", url);

            // Config-relative, root application location.
            url = DashboardHandlerFactory.ResolveHandlerUrl("collar", @"C:\Path\To\Application", "/", @"C:\Path\To\Application\Web.config");
            Assert.AreEqual("/collar", url);

            // Root-relative, non-root application location.
            url = DashboardHandlerFactory.ResolveHandlerUrl("/collar", @"C:\Path\To\Application", "/bluecollar", @"C:\Path\To\Application\Web.config");
            Assert.AreEqual("/collar", url);

            // App-relative, subdirectory config location.
            url = DashboardHandlerFactory.ResolveHandlerUrl("~/collar", @"C:\Path\To\Application", "/bluecollar", @"C:\Path\To\Application\Admin\Web.config");
            Assert.AreEqual("/bluecollar/collar", url);

            // App-relative, non-root application location.
            url = DashboardHandlerFactory.ResolveHandlerUrl("~/", @"C:\Path\To\Application", "/bluecollar", @"C:\Path\To\Application\Web.config");
            Assert.AreEqual("/bluecollar", url);

            // App-relative, root application location.
            url = DashboardHandlerFactory.ResolveHandlerUrl("~/", @"C:\Path\To\Application", "/", @"C:\Path\To\Application\Web.config");
            Assert.AreEqual("/", url);
        }
    }
}
