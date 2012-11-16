//-----------------------------------------------------------------------
// <copyright file="ApplicationCoordinatorTests.cs" company="Tasty Codes">
//     Copyright (c) 2012 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Test
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Security;
    using System.Security.Permissions;
    using BlueCollar.Service;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NLog;

    /// <summary>
    /// Application coordinator tests.
    /// </summary>
    [TestClass]
    [SecurityCritical]
    [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
    [SuppressMessage("Microsoft.Security", "CA2135:SecurityRuleSetLevel2MethodsShouldNotBeProtectedWithLinkDemandsFxCopRule", Justification = "Conflicting recommendations.")]
    public sealed class ApplicationCoordinatorTests
    {
        private static readonly Logger Logger = LogManager.CreateNullLogger();

        /// <summary>
        /// Gets the framework version to use for application processes.
        /// </summary>
        public static string Framework
        {
            get
            {
#if NET35
                return "3.5";
#else
                return "4.0";
#endif
            }
        }

        /// <summary>
        /// Refresh add tests.
        /// </summary>
        [TestMethod]
        public void ApplicationCoordinatorRefreshAdd()
        {
            ApplicationElement element1 = new ApplicationElement() { ApplicationPath = ApplicationUtils.CreateValidExampleApplication(), Framework = Framework };
            ApplicationElement element2 = new ApplicationElement() { ApplicationPath = ApplicationUtils.CreateValidExampleApplication(), Framework = Framework };
            ApplicationElement element3 = new ApplicationElement() { ApplicationPath = ApplicationUtils.CreateValidExampleApplication(), Framework = Framework };

            using (ApplicationCoordinator coordinator = new ApplicationCoordinator(Logger, Path.GetFullPath("Collar.exe")))
            {
                coordinator.StartAndRefresh(new ApplicationElement[] { element1, element2 });
                Assert.IsTrue(coordinator.IsRunning);

                var paths = coordinator.GetCoordinatedApplicationPaths();
                Assert.AreEqual(2, coordinator.Count);
                Assert.IsTrue(paths.Contains(element1.ApplicationPath));
                Assert.IsTrue(paths.Contains(element2.ApplicationPath));

                coordinator.StartAndRefresh(new ApplicationElement[] { element1, element2, element3 });
                Assert.IsTrue(coordinator.IsRunning);

                paths = coordinator.GetCoordinatedApplicationPaths();
                Assert.AreEqual(3, coordinator.Count);
                Assert.IsTrue(paths.Contains(element1.ApplicationPath));
                Assert.IsTrue(paths.Contains(element2.ApplicationPath));
                Assert.IsTrue(paths.Contains(element3.ApplicationPath));
            }
        }

        /// <summary>
        /// Refresh remove tests.
        /// </summary>
        [TestMethod]
        public void ApplicationCoordinatorRefreshRemove()
        {
            ApplicationElement element1 = new ApplicationElement() { ApplicationPath = ApplicationUtils.CreateValidExampleApplication(), Framework = Framework };
            ApplicationElement element2 = new ApplicationElement() { ApplicationPath = ApplicationUtils.CreateValidExampleApplication(), Framework = Framework };
            ApplicationElement element3 = new ApplicationElement() { ApplicationPath = ApplicationUtils.CreateValidExampleApplication(), Framework = Framework };

            using (ApplicationCoordinator coordinator = new ApplicationCoordinator(Logger, Path.GetFullPath("Collar.exe")))
            {
                coordinator.StartAndRefresh(new ApplicationElement[] { element1, element2, element3 });
                Assert.IsTrue(coordinator.IsRunning);

                var paths = coordinator.GetCoordinatedApplicationPaths();
                Assert.AreEqual(3, coordinator.Count);
                Assert.IsTrue(paths.Contains(element1.ApplicationPath));
                Assert.IsTrue(paths.Contains(element2.ApplicationPath));
                Assert.IsTrue(paths.Contains(element3.ApplicationPath));

                coordinator.StartAndRefresh(new ApplicationElement[] { element1, element3 });
                Assert.IsTrue(coordinator.IsRunning);
                
                paths = coordinator.GetCoordinatedApplicationPaths();
                Assert.AreEqual(2, coordinator.Count);
                Assert.IsTrue(paths.Contains(element1.ApplicationPath));
                Assert.IsTrue(paths.Contains(element3.ApplicationPath));
            }
        }

        /// <summary>
        /// Start tests.
        /// </summary>
        [TestMethod]
        public void ApplicationCoordinatorStart()
        {
            ApplicationElement element1 = new ApplicationElement() { ApplicationPath = ApplicationUtils.CreateValidExampleApplication(), Framework = Framework };
            ApplicationElement element2 = new ApplicationElement() { ApplicationPath = ApplicationUtils.CreateValidExampleApplication(), Framework = Framework };

            using (ApplicationCoordinator coordinator = new ApplicationCoordinator(Logger, Path.GetFullPath("Collar.exe")))
            {
                coordinator.StartAndRefresh(new ApplicationElement[] { element1, element2 });
                Assert.IsTrue(coordinator.IsRunning);

                var paths = coordinator.GetCoordinatedApplicationPaths();
                Assert.AreEqual(2, coordinator.Count);
                Assert.IsTrue(paths.Contains(element1.ApplicationPath));
                Assert.IsTrue(paths.Contains(element2.ApplicationPath));
            }
        }
    }
}
