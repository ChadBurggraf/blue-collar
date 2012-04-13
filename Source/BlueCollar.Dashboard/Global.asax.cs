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
                for (int i = 0; i < 1000; i++)
                {
                    BroJob job = new BroJob() { Index = i };

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

            if (Convert.ToBoolean(ConfigurationManager.AppSettings["CreateBlueCollarMachineInProcess"], CultureInfo.InvariantCulture))
            {
                this.machine = new Machine(new NLogger());
            }
        }

        #region BroJob Class

        /// <summary>
        /// Bro <see cref="IJob"/> implementation.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Used for testing in the dashboard UI.")]
        private sealed class BroJob : Job
        {
            /// <summary>
            /// Initializes a new instance of the BroJob class.
            /// </summary>
            public BroJob()
            {
                this.Id = Guid.NewGuid();
            }

            /// <summary>
            /// Gets the bro's ID.
            /// </summary>
            public Guid Id { get; private set; }

            /// <summary>
            /// Gets or sets the bro's index.
            /// </summary>
            public long Index { get; set; }

            /// <summary>
            /// Gets the display name of the job.
            /// </summary>
            public override string Name
            {
                get { return "Brodius Maximus"; }
            }

            /// <summary>
            /// Gets or sets the bro's sleep duration, in seconds.
            /// </summary>
            public int Sleep { get; set; }

            /// <summary>
            /// Gets the maximum timeout, in milliseconds, the job is allowed to run in.
            /// Use 0 for infinite; defaults to 6,000 (1 minute).
            /// </summary>
            public override int Timeout
            {
                get { return 0; }
            }

            /// <summary>
            /// Executes the job.
            /// </summary>
            public override void Execute()
            {
                if (this.Sleep <= 0)
                {
                    System.Threading.Thread.Sleep(10);

                    if (this.Index % 12 == 0)
                    {
                        throw new InvalidOperationException("Index is divisible by 12.");
                    }
                }
                else
                {
                    System.Threading.Thread.Sleep(this.Sleep * 1000);
                }
            }
        }

        #endregion
    }
}