//-----------------------------------------------------------------------
// <copyright file="EventLoggerTests.cs" company="Tasty Codes">
//     Copyright (c) 2012 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Test
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Event logger tests.
    /// </summary>
    [TestClass]
    public sealed class EventLoggerTests
    {
        /// <summary>
        /// Log debug tests.
        /// </summary>
        [TestMethod]
        public void EventLoggerLogDebug()
        {
            ManualResetEvent handle = new ManualResetEvent(false);

            try
            {
                EventLogger logger = new EventLogger();

                logger.Log += (sender, args) =>
                {
                    Assert.AreEqual(EventLoggerEventType.Debug, args.EventType);
                    Assert.AreEqual("Debug", args.Message);
                    Assert.IsNull(args.Exception);
                    handle.Set();
                };

                logger.Debug("Debug");
                WaitHandle.WaitAll(new WaitHandle[] { handle });
            }
            finally
            {
                handle.Close();
            }
        }

        /// <summary>
        /// Log error exception tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Testing with Exception stub only.")]
        public void EventLoggerLogErrorException()
        {
            ManualResetEvent handle = new ManualResetEvent(false);

            try
            {
                EventLogger logger = new EventLogger();
                Exception ex = new Exception();

                logger.Log += (sender, args) =>
                {
                    Assert.AreEqual(EventLoggerEventType.Error, args.EventType);
                    Assert.AreEqual(string.Empty, args.Message);
                    Assert.AreEqual(ex, args.Exception);
                    handle.Set();
                };

                logger.Error(ex);
                WaitHandle.WaitAll(new WaitHandle[] { handle });
            }
            finally
            {
                handle.Close();
            }
        }

        /// <summary>
        /// Log error exception and message tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Testing with Exception stub only.")]
        public void EventLoggerLogErrorExceptionAndMessage()
        {
            ManualResetEvent handle = new ManualResetEvent(false);

            try
            {
                EventLogger logger = new EventLogger();
                Exception ex = new Exception();

                logger.Log += (sender, args) =>
                {
                    Assert.AreEqual(EventLoggerEventType.Error, args.EventType);
                    Assert.AreEqual("Error", args.Message);
                    Assert.AreEqual(ex, args.Exception);
                    handle.Set();
                };

                logger.Error(ex, "Error");
                WaitHandle.WaitAll(new WaitHandle[] { handle });
            }
            finally
            {
                handle.Close();
            }
        }

        /// <summary>
        /// Log error message tests.
        /// </summary>
        [TestMethod]
        public void EventLoggerLogErrorMessage()
        {
            ManualResetEvent handle = new ManualResetEvent(false);

            try
            {
                EventLogger logger = new EventLogger();

                logger.Log += (sender, args) =>
                {
                    Assert.AreEqual(EventLoggerEventType.Error, args.EventType);
                    Assert.AreEqual("Error", args.Message);
                    Assert.IsNull(args.Exception);
                    handle.Set();
                };

                logger.Error("Error");
                WaitHandle.WaitAll(new WaitHandle[] { handle });
            }
            finally
            {
                handle.Close();
            }
        }

        /// <summary>
        /// Log info tests.
        /// </summary>
        [TestMethod]
        public void EventLoggerLogInfo()
        {
            ManualResetEvent handle = new ManualResetEvent(false);

            try
            {
                EventLogger logger = new EventLogger();

                logger.Log += (sender, args) =>
                {
                    Assert.AreEqual(EventLoggerEventType.Info, args.EventType);
                    Assert.AreEqual("Info", args.Message);
                    Assert.IsNull(args.Exception);
                    handle.Set();
                };

                logger.Info("Info");
                WaitHandle.WaitAll(new WaitHandle[] { handle });
            }
            finally
            {
                handle.Close();
            }
        }

        /// <summary>
        /// Log warn tests.
        /// </summary>
        [TestMethod]
        public void EventLoggerLogWarn()
        {
            ManualResetEvent handle = new ManualResetEvent(false);

            try
            {
                EventLogger logger = new EventLogger();

                logger.Log += (sender, args) =>
                {
                    Assert.AreEqual(EventLoggerEventType.Warn, args.EventType);
                    Assert.AreEqual("Warn", args.Message);
                    Assert.IsNull(args.Exception);
                    handle.Set();
                };

                logger.Warn("Warn");
                WaitHandle.WaitAll(new WaitHandle[] { handle });
            }
            finally
            {
                handle.Close();
            }
        }
    }
}
