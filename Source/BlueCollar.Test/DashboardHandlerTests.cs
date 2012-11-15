//-----------------------------------------------------------------------
// <copyright file="DashboardHandlerTests.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Test
{
    using System;
    using System.Collections.Specialized;
    using System.Data;
    using System.IO;
    using System.Text;
    using System.Web;
    using System.Web.Caching;
    using BlueCollar;
    using BlueCollar.Dashboard;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Newtonsoft.Json;

    /// <summary>
    /// Dashboard handler tests.
    /// </summary>
    [TestClass]
    public sealed class DashboardHandlerTests
    {
        /// <summary>
        /// Counts get tests.
        /// </summary>
        [TestMethod]
        public void DashboardHandlerCountsGet()
        {
            var record = new CountsRecord()
            {
                HistoryCount = 1,
                QueueCount = 2,
                ScheduleCount = 3,
                WorkerCount = 4,
                WorkingCount = 5
            };

            var transaction = new Mock<IDbTransaction>();

            var repository = new Mock<IRepository>();
            repository.Setup(r => r.GetCounts(It.IsAny<string>(), It.IsAny<IDbTransaction>())).Returns(record);

            var factory = new Mock<IRepositoryFactory>();
            factory.Setup(f => f.Create()).Returns(repository.Object);

            CountsHandler handler = null;
            string output;

            try
            {
                handler = new CountsHandler(factory.Object);
                handler.ApplicationName = BlueCollarSection.Section.ApplicationName;
                handler.HandlerRelativeRequestUrl = "~/counts";
                handler.QueryString = new QueryString();

                using (MemoryStream inputStream = new MemoryStream())
                {
                    using (MemoryStream outputStream = new MemoryStream())
                    {
                        var context = MockHttpContext("GET", "/counts", inputStream, outputStream);
                        handler.ProcessRequest(context.Object);

                        outputStream.Position = 0;
                        output = Encoding.UTF8.GetString(outputStream.ToArray());
                    }
                }
            }
            finally
            {
                handler.Dispose();
            }

            Assert.IsNotNull(output);
            Assert.IsTrue(0 < output.Length);

            var outputRecord = JsonConvert.DeserializeObject<CountsRecord>(output);
            Assert.IsNotNull(outputRecord);
            Assert.AreEqual(record.HistoryCount, outputRecord.HistoryCount);
            Assert.AreEqual(record.QueueCount, outputRecord.QueueCount);
            Assert.AreEqual(record.ScheduleCount, outputRecord.ScheduleCount);
            Assert.AreEqual(record.WorkerCount, outputRecord.WorkerCount);
            Assert.AreEqual(record.WorkingCount, outputRecord.WorkingCount);
        }

        /// <summary>
        /// Schedule save tests.
        /// </summary>
        [TestMethod]
        public void DashboardHandlerScheduleSave()
        {
            var record = new ScheduleRecord()
            {
                ApplicationName = BlueCollarSection.Section.ApplicationName,
                Id = 0,
                Name = "Nightly",
                QueueName = "schedules",
                RepeatType = ScheduleRepeatType.Days,
                RepeatValue = 1,
                StartOn = new DateTime(2011, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            };

            var transaction = new Mock<IDbTransaction>();

            var repository = new Mock<IRepository>();
            repository.Setup(r => r.AcquireScheduleLock(It.IsAny<long>(), It.IsAny<DateTime>(), It.IsAny<IDbTransaction>())).Returns(true);
            repository.Setup(r => r.UpdateSchedule(It.IsAny<ScheduleRecord>(), It.IsAny<IDbTransaction>())).Returns(record);
            repository
                .Setup(r => r.CreateSchedule(It.IsAny<ScheduleRecord>(), It.IsAny<IDbTransaction>()))
                .Returns(record)
                .Callback(() => 
                { 
                    record.Id = new Random().Next(1, 1000); 
                });

            var factory = new Mock<IRepositoryFactory>();
            factory.Setup(f => f.Create()).Returns(repository.Object);

            using (SaveScheduleHandler handler = new SaveScheduleHandler(factory.Object))
            {
                handler.ApplicationName = BlueCollarSection.Section.ApplicationName;
                handler.HandlerRelativeRequestUrl = "~/schedules";
                handler.QueryString = new QueryString();

                string output;

                using (MemoryStream inputStream = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(record))))
                {
                    using (MemoryStream outputStream = new MemoryStream())
                    {
                        var context = MockHttpContext("POST", "/schedules", inputStream, outputStream);
                        handler.ProcessRequest(context.Object);

                        outputStream.Position = 0;
                        output = Encoding.UTF8.GetString(outputStream.ToArray());
                    }
                }

                Assert.IsNotNull(output);
                Assert.IsTrue(0 < output.Length);

                var outputRecord = JsonConvert.DeserializeAnonymousType(output, new { Id = 0 });
                Assert.IsNotNull(outputRecord);
                Assert.AreEqual(record.Id, outputRecord.Id);

                using (MemoryStream inputStream = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(record))))
                {
                    using (MemoryStream outputStream = new MemoryStream())
                    {
                        var context = MockHttpContext("PUT", "/schedules", inputStream, outputStream);
                        handler.ProcessRequest(context.Object);

                        outputStream.Position = 0;
                        output = Encoding.UTF8.GetString(outputStream.ToArray());
                    }
                }

                Assert.IsNotNull(output);
                Assert.IsTrue(0 < output.Length);

                outputRecord = JsonConvert.DeserializeAnonymousType(output, new { Id = 0 });
                Assert.IsNotNull(outputRecord);
                Assert.AreEqual(record.Id, outputRecord.Id);
            }
        }

        /// <summary>
        /// Creates a mock <see cref="HttpContextBase"/>.
        /// </summary>
        /// <param name="verb">The request verb.</param>
        /// <param name="urlPath">The request URL path, relative to the handler.</param>
        /// <param name="inputStream">The input stream.</param>
        /// <param name="outputStream">The output stream.</param>
        /// <returns>A mock <see cref="HttpContextBase"/>.</returns>
        private static Mock<HttpContextBase> MockHttpContext(string verb, string urlPath, Stream inputStream, Stream outputStream)
        {
            var request = new Mock<HttpRequestBase>();
            request.Setup(r => r.ContentLength).Returns((int)inputStream.Length);
            request.Setup(r => r.CurrentExecutionFilePath).Returns(@"C:\BlueCollar\collar.ashx");
            request.Setup(r => r.Headers).Returns(new NameValueCollection());
            request.Setup(r => r.HttpMethod).Returns(verb);
            request.Setup(r => r.InputStream).Returns(inputStream);
            request.Setup(r => r.PhysicalApplicationPath).Returns(@"C:\BlueCollar");
            request.Setup(r => r.RawUrl).Returns("/collar.ashx" + urlPath);
            request.Setup(r => r.Url).Returns(new Uri("http://localhost/bluecollar/collar.ashx" + urlPath));

            var cachePolicy = new Mock<HttpCachePolicyBase>();

            var response = new Mock<HttpResponseBase>();
            response.SetupAllProperties();
            response.Setup(r => r.Cache).Returns(cachePolicy.Object);
            response.Setup(r => r.Headers).Returns(new NameValueCollection());
            response.Setup(r => r.OutputStream).Returns(outputStream);

            var context = new Mock<HttpContextBase>();
            context.Setup(c => c.Cache).Returns(HttpRuntime.Cache);
            context.Setup(c => c.Request).Returns(request.Object);
            context.Setup(c => c.Response).Returns(response.Object);

            return context;
        }
    }
}
