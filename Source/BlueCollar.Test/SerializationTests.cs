//-----------------------------------------------------------------------
// <copyright file="SerializationTests.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Test
{
    using System;
    using System.Collections.Specialized;
    using System.Runtime.Serialization;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Serialization tests.
    /// </summary>
    [TestClass]
    public sealed class SerializationTests
    {
        /// <summary>
        /// De-serialize tests.
        /// </summary>
        [TestMethod]
        public void SerializationDeserialize()
        {
            string typeName = JobSerializer.GetTypeName(typeof(TestSerializationJob));
            string data = @"{""A"":""1854ef1b-3937-476a-8b32-56436a7b6feb"",""B"":""Hello, world!"",""C"":""1982-05-28T07:00:00Z""}";
            TestSerializationJob job = JobSerializer.Deserialize(typeName, data) as TestSerializationJob;
            Assert.IsNotNull(job);
            Assert.AreEqual("1854ef1b-3937-476a-8b32-56436a7b6feb", job.A);
            Assert.AreEqual("Hello, world!", job.B);
            Assert.AreEqual(new DateTime(1982, 5, 28).ToUniversalTime(), job.C);

            Assert.IsNotNull(JobSerializer.Deserialize(typeName, null));
        }

        /// <summary>
        /// De-serialize scheduled job tests.
        /// </summary>
        [TestMethod]
        public void SerializationDeserializeScheduledJob()
        {
            string typeName = JobSerializer.GetTypeName(typeof(TestSerializationScheduledJob));
            string data = @"{""A"":""1854ef1b-3937-476a-8b32-56436a7b6feb"",""B"":""Hello, world!"",""C"":""5/28/1982 7:00:00 AM""}";
            TestSerializationScheduledJob job = JobSerializer.Deserialize(typeName, data) as TestSerializationScheduledJob;
            Assert.IsNotNull(job);
            Assert.AreEqual("1854ef1b-3937-476a-8b32-56436a7b6feb", job.Properties["A"]);
            Assert.AreEqual("Hello, world!", job.Properties["B"]);
            Assert.AreEqual("5/28/1982 7:00:00 AM", job.Properties["C"]);

            Assert.IsNotNull(JobSerializer.Deserialize(typeName, null));
        }

        /// <summary>
        /// Serialize tests.
        /// </summary>
        [TestMethod]
        public void SerializationSerialize()
        {
            var job = new TestSerializationJob()
            {
                A = new Guid("1854ef1b-3937-476a-8b32-56436a7b6feb").ToString(),
                B = "Hello, world!",
                C = new DateTime(1982, 5, 28).ToUniversalTime()
            };

            string data = JobSerializer.Serialize(job);
            Assert.AreEqual(@"{""A"":""1854ef1b-3937-476a-8b32-56436a7b6feb"",""B"":""Hello, world!"",""C"":""1982-05-28T07:00:00Z""}", data);
        }

        /// <summary>
        /// Serialize scheduled job tests.
        /// </summary>
        [TestMethod]
        public void SerializationSerializeScheduledJob()
        {
            TestSerializationScheduledJob job = new TestSerializationScheduledJob();
            job.Properties["A"] = new Guid("1854ef1b-3937-476a-8b32-56436a7b6feb").ToString();
            job.Properties["B"] = "Hello, world!";
            job.Properties["C"] = new DateTime(1982, 5, 28).ToUniversalTime().ToString();

            string data = JobSerializer.Serialize(job);
            Assert.AreEqual(@"{""A"":""1854ef1b-3937-476a-8b32-56436a7b6feb"",""B"":""Hello, world!"",""C"":""5/28/1982 7:00:00 AM""}", data);
        }

        #region TestSerializationJob Class

        /// <summary>
        /// Test serialization job.
        /// </summary>
        [DataContract]
        private class TestSerializationJob : IJob
        {
            /// <summary>
            /// Gets or sets A.
            /// </summary>
            [DataMember]
            public string A { get; set; }

            /// <summary>
            /// Gets or sets B.
            /// </summary>
            [DataMember]
            public string B { get; set; }

            /// <summary>
            /// Gets or sets C.
            /// </summary>
            [DataMember]
            public DateTime C { get; set; }

            /// <summary>
            /// Gets the display name of the job.
            /// </summary>
            public string Name
            {
                get { return "Test Serialize"; }
            }

            /// <summary>
            /// Gets the maximum number of retries for the job.
            /// Use 0 for infinite, -1 for none.
            /// </summary>
            public int Retries
            {
                get { return 0; }
            }

            /// <summary>
            /// Gets the maximum timeout, in milliseconds, the job is allowed to run in.
            /// Use 0 for infinite.
            /// </summary>
            public int Timeout
            {
                get { return 0; }
            }

            /// <summary>
            /// Executes the job.
            /// </summary>
            public void Execute()
            {
            }
        }

        #endregion

        #region TestSerializationScheduledJob Class

        /// <summary>
        /// Test serialization scheduled job.
        /// </summary>
        [DataContract]
        private class TestSerializationScheduledJob : TestSerializationJob, IScheduledJob
        {
            /// <summary>
            /// Initializes a new instance of the TestSerializationScheduledJob class.
            /// </summary>
            public TestSerializationScheduledJob()
            {
                this.Properties = new NameValueCollection();
            }

            /// <summary>
            /// Gets a dictionary to which properties defined in the schedule
            /// are added for reference during execution.
            /// </summary>
            public NameValueCollection Properties { get; private set; }
        }

        #endregion
    }
}
