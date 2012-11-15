//-----------------------------------------------------------------------
// <copyright file="MachineTests.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Test
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    /// <summary>
    /// Machine tests.
    /// </summary>
    [TestClass]
    public sealed class MachineTests
    {
        /// <summary>
        /// Machine address tests.
        /// </summary>
        [TestMethod]
        public void MachineAddress()
        {
            Assert.IsNotNull(Machine.Address);
        }

        /// <summary>
        /// Ensure default worker tests.
        /// </summary>
        [TestMethod]
        public void MachineEnsureDefaultWorker()
        {
            SignalsRecord signals = new SignalsRecord() { QueueNames = "*", WorkerSignal = WorkerSignal.None, WorkingSignal = WorkingSignal.None };

            var repository = new Mock<IRepository>();
            repository.Setup(r => r.GetWorkingSignals(It.IsAny<long>(), It.IsAny<long?>())).Returns(signals);

            var factory = new Mock<IRepositoryFactory>();
            factory.Setup(f => f.Create()).Returns(repository.Object);

            var logger = new Mock<ILogger>();

            using (Machine machine = new Machine(logger.Object, factory.Object, BlueCollarSection.Section.ApplicationName, Machine.Address, Machine.Name, 1, 1, false, true))
            {
                Thread.Sleep(1500);
            }

            repository.Verify(r => r.CreateWorker(It.Is<WorkerRecord>(w => w.ApplicationName == BlueCollarSection.Section.ApplicationName && w.Name == "Default" && w.MachineAddress == Machine.Address && w.MachineName == Machine.Name)));
        }

        /// <summary>
        /// Get public address tests.
        /// </summary>
        [TestMethod]
        public void MachineGetPublicAddress()
        {
            Assert.IsNotNull(Machine.GetPublicMachineAddress());
        }

        /// <summary>
        /// Get private address tests.
        /// </summary>
        [TestMethod]
        public void MachineGetPrivateAddress()
        {
            Assert.IsNotNull(Machine.GetPrivateMachineAddress());
        }

        /// <summary>
        /// Machine name tests.
        /// </summary>
        [TestMethod]
        public void MachineName()
        {
            Assert.IsNotNull(Machine.Name);
            Assert.IsTrue(0 < Machine.Name.Length);
        }
    }
}
