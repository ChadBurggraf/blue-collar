//-----------------------------------------------------------------------
// <copyright file="RepositoryFactoryTests.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Test
{
    using System;
    using System.Configuration;
    using BlueCollar;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Repository factory tests.
    /// </summary>
    [TestClass]
    [DeploymentItem(@"x86\SQLite.Interop.dll", "x86")]
    [DeploymentItem(@"x64\SQLite.Interop.dll", "x64")]
    public sealed class RepositoryFactoryTests
    {
        /// <summary>
        /// Configuration create with defaults tests.
        /// </summary>
        [TestMethod]
        public void RepositoryFactoryConfigurationCreateWithDefaults()
        {
            RepositoryElement defaultConfig = new RepositoryElement();
            IRepositoryFactory factory = new ConfigurationRepositoryFactory(defaultConfig.RepositoryType, defaultConfig.ConnectionStringName);
            IRepository repository = factory.Create();
            Assert.IsNotNull(repository);
            Assert.IsInstanceOfType(repository, typeof(SQLiteRepository));
        }

        /// <summary>
        /// Configuration create with specified tests.
        /// </summary>
        [TestMethod]
        public void RepositoryFactoryConfigurationCreateWithSpecified()
        {
            IRepositoryFactory factory = new ConfigurationRepositoryFactory("BlueCollar.Test.TestNoOpRepository, BlueCollar.Test", "TestNoOpRepository");
            IRepository repository = factory.Create();
            Assert.IsNotNull(repository);
            Assert.IsInstanceOfType(repository, typeof(TestNoOpRepository));
            Assert.AreEqual(ConfigurationManager.ConnectionStrings["TestNoOpRepository"].ConnectionString, ((TestNoOpRepository)repository).ConnectionString);
        }

        /// <summary>
        /// Configuration fail create with missing connection string tests.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void RepositoryFactoryConfigurationFailCreateWithMissingConnectionString()
        {
            IRepositoryFactory factory = new ConfigurationRepositoryFactory("BlueCollar.Test.TestNoOpRepository, BlueCollar.Test", "NotAConnectionStringName");
            factory.Create();
        }
        
        /// <summary>
        /// Configuration fail create not an IRepository tests.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void RepositoryFactoryConfigurationFailCreateNotAnIRepository()
        {
            IRepositoryFactory factory = new ConfigurationRepositoryFactory("BlueCollar.Test.RepositoryFactoryTests, BlueCollar.Test", null);
            factory.Create();
        }
    }
}
