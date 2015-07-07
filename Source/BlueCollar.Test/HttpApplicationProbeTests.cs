//-----------------------------------------------------------------------
// <copyright file="HttpApplicationProbeTests.cs" company="Tasty Codes">
//     Copyright (c) 2015 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using BlueCollar.Examples;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NLog;

    /// <summary>
    /// HTTP application probe tests.
    /// </summary>
    [TestClass]
    public sealed class HttpApplicationProbeTests
    {
        private static readonly ILogger Logger = new NLogger();

        /// <summary>
        /// Gets or sets the current test context.
        /// </summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// Find application assemblies tests.
        /// </summary>
        [TestMethod]
        public void HttpApplicationProbeFindApplicationAssemblies()
        {
            IEnumerable<Assembly> assemblies = this.FindAssemblies();
            Assert.AreEqual(1, assemblies.Count(a => a.FullName.StartsWith("BlueCollar.Examples", StringComparison.Ordinal)));
        }

        /// <summary>
        /// Find application types tests.
        /// </summary>
        [TestMethod]
        public void HttpApplicationProbeFindApplicationTypes()
        {
            IEnumerable<Type> types = this.FindTypes();
            Assert.AreEqual(1, types.Count(t => typeof(HttpApplicationEntryPoint).Equals(t)));
        }

        /// <summary>
        /// Find entry point tests.
        /// </summary>
        [TestMethod]
        public void HttpApplicationProbeFindEntryPoint()
        {
            Type type = this.FindTypes().First();
            MethodInfo info = HttpApplicationProbe.FindEntryPoint(type);
            Assert.IsNotNull(info);
        }

        /// <summary>
        /// Find exit point tests.
        /// </summary>
        [TestMethod]
        public void HttpApplicationProbeFindExitPoint()
        {
            Type type = this.FindTypes().First();
            MethodInfo info = HttpApplicationProbe.FindExitPoint(type);
            Assert.IsNotNull(info);
        }

        /// <summary>
        /// Finds <see cref="HttpApplication"/> assemblies in the current test context.
        /// </summary>
        /// <returns>A collection of <see cref="HttpApplication"/> assemblies.</returns>
        private IEnumerable<Assembly> FindAssemblies()
        {
            HttpApplicationProbe probe = new HttpApplicationProbe(HttpApplicationProbeTests.Logger, this.TestContext.TestDeploymentDir);
            IEnumerable<Assembly> assemblies = probe.FindApplicationAssemblies();
            Assert.IsNotNull(assemblies);
            Assert.IsTrue(assemblies.Any());
            return assemblies;
        }

        /// <summary>
        /// Finds a collection of <see cref="HttpApplication"/> types in the current test context.
        /// </summary>
        /// <returns>A collection of <see cref="HttpApplication"/> types.</returns>
        private IEnumerable<Type> FindTypes()
        {
            return this.FindTypes(null);
        }

        /// <summary>
        /// Finds a collection of <see cref="HttpApplication"/> types in the give collection of assemblies.
        /// </summary>
        /// <param name="assemblies">A collection of assemblies, or null to probe the current test context.</param>
        /// <returns>A collection of <see cref="HttpApplication"/> types.</returns>
        private IEnumerable<Type> FindTypes(IEnumerable<Assembly> assemblies)
        {
            IEnumerable<Type> types = HttpApplicationProbe.FindApplicationTypes(assemblies ?? this.FindAssemblies());
            Assert.IsNotNull(types);
            Assert.IsTrue(types.Any());
            return types;
        }
    }
}
