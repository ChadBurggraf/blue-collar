//-----------------------------------------------------------------------
// <copyright file="Global.asax.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Dashboard
{
    using System;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    /// <summary>
    /// Represents the primary application container.
    /// </summary>
    /// <remarks>
    /// For instructions on enabling IIS6 or IIS7 classic mode, visit http://go.microsoft.com/?LinkId=9394801
    /// </remarks>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Mvc", Justification = "The spelling is correct.")]
    public class MvcApplication : HttpApplication
    {
        private Machine machine;

        /// <summary>
        /// Registers routes to the given route collection.
        /// </summary>
        /// <param name="routes">The route collection to register routes with.</param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("collar.ashx/{*pathInfo}");
            routes.IgnoreRoute("collar/{*pathInfo}");

            routes.MapRoute(
                "Test",
                "test",
                new { controller = "Home", action = "Test" });

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional });
        }

        /// <summary>
        /// Raises the application's End event.
        /// </summary>
        protected void Application_End()
        {
            if (this.machine != null)
            {
                this.machine.Dispose();
            }
        }

        /// <summary>
        /// Raises the application's Start event.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Convention-based event handler.")]
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RegisterRoutes(RouteTable.Routes);

            /*using (IRepository repository = new ConfigurationRepositoryFactory().Create())
            {
                for (int i = 0; i < 100; i++)
                {
                    var job = new BlueCollar.Examples.SleepJob() { Duration = 60000 };

                    repository.CreateQueued(
                        new QueueRecord()
                        {
                            ApplicationName = BlueCollarSection.Section.ApplicationName,
                            Data = JobSerializer.Serialize(job),
                            JobName = job.Name,
                            JobType = JobSerializer.GetTypeName(job.GetType()),
                            QueuedOn = DateTime.UtcNow,
                            TryNumber = 1
                        },
                        null);
                }
            }*/

            if (!BlueCollarSection.Section.Machine.ServiceExecutionEnabled)
            {
                this.machine = new Machine(new NLogger());
            }
        }
    }
}