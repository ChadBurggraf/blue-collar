//-----------------------------------------------------------------------
// <copyright file="BootstrapsTests.cs" company="Tasty Codes">
//     Copyright (c) 2012 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Test
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Security;
    using System.Security.Permissions;
    using System.Threading;
    using BlueCollar.Console;
    using BlueCollar.Examples;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Bootstraps tests.
    /// </summary>
    [TestClass]
    [DeploymentItem(@"x86\SQLite.Interop.dll", "x86")]
    [DeploymentItem(@"x64\SQLite.Interop.dll", "x64")]
    [SecurityCritical]
    [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
    [SuppressMessage("Microsoft.Security", "CA2135:SecurityRuleSetLevel2MethodsShouldNotBeProtectedWithLinkDemands", Justification = "Appears to be an FxCop bug.")]
    public sealed class BootstrapsTests
    {
        /// <summary>
        /// Basic application assemblies not locked tests.
        /// </summary>
        [TestMethod]
        public void BootstrapsBasicApplicationAssembliesNotLocked()
        {
            string path = ApplicationUtils.CreateValidExampleApplication();
            AssembliesNotLocked(path, Path.Combine(path, "BlueCollar.dll"));
        }

        /// <summary>
        /// Basic application file change tests.
        /// </summary>
        [TestMethod]
        public void BootstrapsBasicApplicationFileChange()
        {
            string path = ApplicationUtils.CreateValidExampleApplication();
            string filePath = Path.Combine(path, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ".dll");
            ManualResetEvent handle = new ManualResetEvent(false);

            try
            {
                using (Bootstraps bootstraps = new Bootstraps(path, null, 500))
                {
                    bootstraps.ApplicationFilesChanged += (sender, e) =>
                    {
                        Assert.AreEqual(filePath, e.FullPath);
                        handle.Set();
                    };

                    Assert.AreEqual(BootstrapsPullupResultType.Success, bootstraps.PullUp().ResultType);

                    using (File.Create(filePath)) 
                    {
                    }

                    WaitHandle.WaitAll(new WaitHandle[] { handle });
                }
            }
            finally
            {
                handle.Close();
            }
        }

        /// <summary>
        /// Pull up web application and execute job tests.
        /// </summary>
        [TestMethod]
        public void BootstrapsPullUpAndExecuteJob()
        {
            string path = ApplicationUtils.CreateValidExampleApplication();

            using (Bootstraps bootstraps = new Bootstraps(path, null, 500))
            {
                Assert.AreEqual(BootstrapsPullupResultType.Success, bootstraps.PullUp(true).ResultType);

                CreateFileJob job = new CreateFileJob()
                {
                    Path = Path.Combine(path, Path.GetRandomFileName())
                };

                Assert.IsFalse(File.Exists(job.Path));

                // The default configuration specifies a SQLite repository pointing
                // to BlueCollar.sqlite in the application's root directory.
                using (IRepository repository = new SQLiteRepository(string.Format(CultureInfo.InvariantCulture, "data source={0};journal mode=Off;synchronous=Off;version=3", Path.Combine(path, "BlueCollar.sqlite"))))
                {
                    job.Enqueue("Default", null, repository);
                }

                // Default worker heartbeat is 5 seconds.
                Thread.Sleep(6000);
                Assert.IsTrue(File.Exists(job.Path));
            }
        }

        /// <summary>
        /// Pull up basic application tests.
        /// </summary>
        [TestMethod]
        public void BootstrapsPullUpBasicApplication()
        {
            string path = ApplicationUtils.CreateValidExampleApplication();

            using (Bootstraps bootstraps = new Bootstraps(path, null, 500))
            {
                Assert.AreEqual(BootstrapsPullupResultType.Success, bootstraps.PullUp().ResultType);
                Assert.IsTrue(bootstraps.IsLoaded);
            }
        }

        /// <summary>
        /// Pull up basic web application tests.
        /// </summary>
        [TestMethod]
        public void BootstrapsPullUpBasicWebApplication()
        {
            string path = ApplicationUtils.CreateValidExampleWebApplication();

            using (Bootstraps bootstraps = new Bootstraps(path, null, 500))
            {
                Assert.AreEqual(BootstrapsPullupResultType.Success, bootstraps.PullUp().ResultType);
                Assert.IsTrue(bootstraps.IsLoaded);
            }
        }

        /// <summary>
        /// Pull up fail tests.
        /// </summary>
        [TestMethod]
        public void BootstrapsPullUpFail()
        {
            string path = ApplicationUtils.CreateValidExampleApplication();
            File.Delete(Path.Combine(path, "BlueCollar.dll"));

            using (Bootstraps bootstraps = new Bootstraps(path, null, 500))
            {
                BootstrapsPullupResult result = bootstraps.PullUp();
                Assert.AreEqual(BootstrapsPullupResultType.Exception, result.ResultType);
                Assert.IsNotNull(result.Exception);
            }
        }

        /// <summary>
        /// Web application app code file change tests.
        /// </summary>
        [TestMethod]
        public void BootstrapsWebApplicationAppCodeFileChange()
        {
            string path = ApplicationUtils.CreateValidExampleWebApplication();
            string appCodePath = Path.Combine(path, "App_Code");
            Directory.CreateDirectory(appCodePath);
            string filePath = Path.Combine(appCodePath, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ".cs");
            ManualResetEvent handle = new ManualResetEvent(false);

            try
            {
                using (Bootstraps bootstraps = new Bootstraps(path, null, 500))
                {
                    bootstraps.ApplicationFilesChanged += (sender, e) =>
                    {
                        Assert.AreEqual(filePath, e.FullPath);
                        handle.Set();
                    };

                    Assert.AreEqual(BootstrapsPullupResultType.Success, bootstraps.PullUp().ResultType);

                    using (File.Create(filePath))
                    {
                    }

                    WaitHandle.WaitAll(new WaitHandle[] { handle });
                }
            }
            finally
            {
                handle.Close();
            }
        }

        /// <summary>
        /// Web application assemblies not locked tests.
        /// </summary>
        [TestMethod]
        public void BootstrapsWebApplicationAssembliesNotLocked()
        {
            string path = ApplicationUtils.CreateValidExampleWebApplication();
            AssembliesNotLocked(path, Path.Combine(Path.Combine(path, "bin"), "BlueCollar.dll"));
        }

        /// <summary>
        /// Web application bin file change tests.
        /// </summary>
        [TestMethod]
        public void BootstrapsWebApplicationBinFileChange()
        {
            string path = ApplicationUtils.CreateValidExampleWebApplication();
            string binPath = Path.Combine(path, "bin");
            string filePath = Path.Combine(binPath, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ".dll");
            ManualResetEvent handle = new ManualResetEvent(false);

            try
            {
                using (Bootstraps bootstraps = new Bootstraps(path, null, 500))
                {
                    bootstraps.ApplicationFilesChanged += (sender, e) =>
                    {
                        Assert.AreEqual(filePath, e.FullPath);
                        handle.Set();
                    };

                    Assert.AreEqual(BootstrapsPullupResultType.Success, bootstraps.PullUp().ResultType);

                    using (File.Create(filePath))
                    {
                    }

                    WaitHandle.WaitAll(new WaitHandle[] { handle });
                }
            }
            finally
            {
                handle.Close();
            }
        }

        /// <summary>
        /// Web application config file change tests.
        /// </summary>
        [TestMethod]
        public void BootstrapsWebApplicationConfigFileChange()
        {
            string path = ApplicationUtils.CreateValidExampleWebApplication();
            ManualResetEvent handle = new ManualResetEvent(false);

            try
            {
                using (Bootstraps bootstraps = new Bootstraps(path, null, 500))
                {
                    bootstraps.ApplicationFilesChanged += (sender, e) =>
                    {
                        Assert.AreEqual(Path.Combine(path, "Web.config"), e.FullPath);
                        handle.Set();
                    };

                    Assert.AreEqual(BootstrapsPullupResultType.Success, bootstraps.PullUp().ResultType);

                    using (File.Create(Path.Combine(path, "Web.config")))
                    {
                    }

                    WaitHandle.WaitAll(new WaitHandle[] { handle });
                }
            }
            finally
            {
                handle.Close();
            }
        }

        /// <summary>
        /// Executes tests that ensure assemblies are not locked when the remote app domain
        /// is loaded by <see cref="Bootstraps"/>.
        /// </summary>
        /// <param name="path">The path of the application to test.</param>
        /// <param name="pathToDelete">The path of the assembly file to delete.</param>
        private static void AssembliesNotLocked(string path, string pathToDelete)
        {
            using (Bootstraps bootstraps = new Bootstraps(path, null, 500))
            {
                Assert.AreEqual(BootstrapsPullupResultType.Success, bootstraps.PullUp().ResultType);
                Assert.IsTrue(File.Exists(pathToDelete));

                ProcessStartInfo info = new ProcessStartInfo()
                {
                    Arguments = string.Format(CultureInfo.InvariantCulture, @"/C del ""{0}""", pathToDelete),
                    CreateNoWindow = true,
                    FileName = "cmd.exe",
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                };

                using (Process process = new Process())
                {
                    process.StartInfo = info;
                    Assert.IsTrue(process.Start());

                    process.WaitForExit();
                    Assert.AreEqual(0, process.ExitCode);
                    Assert.AreEqual(string.Empty, process.StandardError.ReadToEnd());
                    Assert.IsFalse(File.Exists(pathToDelete));
                }
            }
        }
    }
}
