//-----------------------------------------------------------------------
// <copyright file="ApplicationProcessTests.cs" company="Tasty Codes">
//     Copyright (c) 2012 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Test
{
    using System;
    using System.IO;
    using System.Threading;
    using BlueCollar.Service;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NLog;

    /// <summary>
    /// Application process tests.
    /// </summary>
    [TestClass]
    public sealed class ApplicationProcessTests
    {
        private static readonly Logger logger = LogManager.CreateNullLogger();

        /// <summary>
        /// Base path tests.
        /// </summary>
        [TestMethod]
        public void ApplicationProcessBasePath()
        {
            using (ApplicationProcess process = new ApplicationProcess(logger, @"C:\Path"))
            {
                Assert.AreEqual(Path.GetFullPath(Path.GetDirectoryName(GetType().Assembly.Location)), process.BasePath);
            }
        }

        /// <summary>
        /// Fail path invalid tests.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ApplicationProcessFailConfigPathInvalid()
        {
            using (ApplicationProcess process = new ApplicationProcess(logger, @"C:\Path"))
            {
                process.ConfigPath = @"C:\Path\Invalid>";
            }
        }

        /// <summary>
        /// Fail framework version invalid tests.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ApplicationProcessFailFrameworkVersionInvalid()
        {
            using (ApplicationProcess process = new ApplicationProcess(logger, @"C:\Path"))
            {
                process.FrameworkVersion = "1.0";
            }
        }

        /// <summary>
        /// Fail path empty tests.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ApplicationProcessFailPathEmpty()
        {
            using (ApplicationProcess process = new ApplicationProcess(logger, null))
            {
            }
        }

        /// <summary>
        /// Fail path invalid tests.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ApplicationProcessFailPathInvalid()
        {
            using (ApplicationProcess process = new ApplicationProcess(logger, @"C:\Invalid\Path>"))
            {
            }
        }

        /// <summary>
        /// Fail threshold invalid tests.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ApplicationProcessFailThresholdInvalid()
        {
            using (ApplicationProcess process = new ApplicationProcess(logger, @"C:\Path"))
            {
                process.Threshold = 0;
            }
        }

        /// <summary>
        /// Start tests.
        /// </summary>
        [TestMethod]
        public void ApplicationProcessStart()
        {
            string path = ApplicationUtils.CreateValidExampleApplication();

            using (ApplicationProcess process = new ApplicationProcess(logger, path, Path.GetFullPath("Collar.exe")))
            {
                Assert.IsTrue(process.Start());
            }
        }

        /// <summary>
        /// Stop tests.
        /// </summary>
        [TestMethod]
        public void ApplicationProcessStop()
        {
            string path = ApplicationUtils.CreateValidExampleApplication();
            ManualResetEvent handle = new ManualResetEvent(false);

            try
            {
                using (ApplicationProcess process = new ApplicationProcess(logger, path, Path.GetFullPath("Collar.exe")))
                {
                    process.Exited += (object sender, EventArgs e) =>
                    {
                        handle.Set();
                    };

                    Assert.IsTrue(process.Start());
                    process.Stop(false);
                    WaitHandle.WaitAll(new WaitHandle[] { handle });
                }
            }
            finally
            {
                handle.Close();
            }
        }

        /// <summary>
        /// Stop force tests.
        /// </summary>
        [TestMethod]
        public void ApplicationProcessStopForce()
        {
            string path = ApplicationUtils.CreateValidExampleApplication();
            ManualResetEvent handle = new ManualResetEvent(false);

            try
            {
                using (ApplicationProcess process = new ApplicationProcess(logger, path, Path.GetFullPath("Collar.exe")))
                {
                    process.Exited += (object sender, EventArgs e) =>
                    {
                        Assert.Fail();
                        handle.Set();
                    };

                    process.KillTimeout += (object sender, EventArgs e) =>
                    {
                        Assert.Fail();
                        handle.Set();
                    };

                    Assert.IsTrue(process.Start());
                    process.Stop(true);
                    WaitHandle.WaitAll(new WaitHandle[] { handle }, 6000);
                }
            }
            finally
            {
                handle.Close();
            }
        }
    }
}
