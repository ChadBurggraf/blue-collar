//-----------------------------------------------------------------------
// <copyright file="DashboardRouter.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Dashboard
{
    using System;

    /// <summary>
    /// Provides simple static routing for the dashboard.
    /// </summary>
    internal static class DashboardRouter
    {
        private static readonly IDashboardRoute[] Routes = new IDashboardRoute[]
        {
            new DashboardRoute<IndexHandler>(@"^$", new string[] { "GET" }),
            new DashboardRoute<IndexHandler>(@"^index.html$", new string[] { "GET" }),
            new DashboardRoute<StyleSheetHandler>(@"^(.+?\.)([a-f0-9]+\.)?(css)$", new string[] { "GET" }),
            new DashboardRoute<StaticFileHandler>(@"^(.+?\.)([a-f0-9]+\.)?(gif|js|png|eot|svg|ttf|woff)$", new string[] { "GET" }),
            new DashboardRoute<CountsHandler>(@"^counts$", new string[] { "GET" }),
            new DashboardRoute<HistoryListHandler>(@"^history$", new string[] { "GET" }),
            new DashboardRoute<HistoryDetailsHandler>(@"^history/(\d+)$", new string[] { "GET" }),
            new DashboardRoute<QueuedListHandler>(@"^queue$", new string[] { "GET" }),
            new DashboardRoute<SaveQueuedHandler>(@"^queue$", new string[] { "POST" }),
            new DashboardRoute<QueuedDetailsHandler>(@"^queue/(\d+)$", new string[] { "GET" }),
            new DashboardRoute<DeleteQueuedHandler>(@"^queue/(\d+)$", new string[] { "DELETE" }),
            new DashboardRoute<ScheduleListHandler>(@"^schedules$", new string[] { "GET" }),
            new DashboardRoute<SaveScheduleHandler>(@"^schedules$", new string[] { "POST" }),
            new DashboardRoute<SaveScheduleHandler>(@"^schedules/(\d+)$", new string[] { "PUT" }),
            new DashboardRoute<DeleteScheduleHandler>(@"^schedules/(\d+)$", new string[] { "DELETE" }),
            new DashboardRoute<ScheduledJobListHandler>(@"^schedules/(\d+)/jobs$", new string[] { "GET" }),
            new DashboardRoute<SaveScheduledJobHandler>(@"^schedules/(\d+)/jobs$", new string[] { "POST" }),
            new DashboardRoute<SaveScheduledJobHandler>(@"^schedules/(\d+)/jobs/(\d+)$", new string[] { "PUT" }),
            new DashboardRoute<DeleteScheduledJobHandler>(@"^schedules/(\d+)/jobs/(\d+)$", new string[] { "DELETE" }),
            new DashboardRoute<DashboardStatsHandler>(@"^stats$", new string[] { "GET" }),
            new DashboardRoute<WorkerListHandler>(@"^workers$", new string[] { "GET" }),
            new DashboardRoute<SaveWorkerHandler>(@"^workers$", new string[] { "POST" }),
            new DashboardRoute<SaveWorkerHandler>(@"^workers/(\d+)$", new string[] { "PUT" }),
            new DashboardRoute<DeleteWorkerHandler>(@"^workers/(\d+)$", new string[] { "DELETE" }),
            new DashboardRoute<SignalWorkerHandler>(@"^workers/(\d+)/signal$", new string[] { "PUT" }),
            new DashboardRoute<WorkingListHandler>(@"^working$", new string[] { "GET" }),
            new DashboardRoute<WorkingDetailsHandler>(@"^working/(\d+)$", new string[] { "GET" }),
            new DashboardRoute<SignalWorkingHandler>(@"^working/(\d+)/signal$", new string[] { "PUT" })
        };

        /// <summary>
        /// Gets the <see cref="IDashboardHandler"/> mapped to the route identified by the given verb and handler-relative URL.
        /// </summary>
        /// <param name="verb">The verb to get the handler for.</param>
        /// <param name="handlerRelativeUrl">The handler-relative URL to get the handler for.</param>
        /// <param name="repositoryFactory">The repository factory to use when instantiating the handler.</param>
        /// <returns>An <see cref="IDashboardHandler"/> instance, or null if no routes matched.</returns>
        public static IDashboardHandler GetHandler(string verb, string handlerRelativeUrl, IRepositoryFactory repositoryFactory)
        {
            verb = NormalizeVerb(verb);
            string path = NormalizeHandlerRelativeUrl(handlerRelativeUrl);
            IDashboardRoute route = GetRoute(verb, handlerRelativeUrl);

            if (route != null)
            {
                return route.CreateHandler(verb, path, repositoryFactory);
            }

            return null;
        }

        /// <summary>
        /// Gets the <see cref="IDashboardRoute"/> matching the given verb and handler-relative URL.
        /// </summary>
        /// <param name="verb">The verb to get the route for.</param>
        /// <param name="handlerRelativeUrl">The handler-relative URL to get the verb for.</param>
        /// <returns>An <see cref="IDashboardRoute"/> instance, or null if no routes matched.</returns>
        public static IDashboardRoute GetRoute(string verb, string handlerRelativeUrl)
        {
            verb = NormalizeVerb(verb);
            string path = NormalizeHandlerRelativeUrl(handlerRelativeUrl);

            foreach (IDashboardRoute route in Routes)
            {
                if (route.IsMatch(verb, path))
                {
                    return route;
                }
            }

            return null;
        }

        /// <summary>
        /// Normalizes a handler-relative URL in preparation for route matching.
        /// </summary>
        /// <param name="handlerRelativeUrl">The handler-relative URL to normalize.</param>
        /// <returns>A normalized handler-relative URL string.</returns>
        public static string NormalizeHandlerRelativeUrl(string handlerRelativeUrl)
        {
            handlerRelativeUrl = (handlerRelativeUrl ?? string.Empty).Trim().ToUpperInvariant();

            if (handlerRelativeUrl.StartsWith("~/", StringComparison.Ordinal))
            {
                handlerRelativeUrl = handlerRelativeUrl.Substring(2).ToUpperInvariant();
            }

            if (!string.IsNullOrEmpty(handlerRelativeUrl) && handlerRelativeUrl[handlerRelativeUrl.Length - 1] == '/')
            {
                handlerRelativeUrl = handlerRelativeUrl.Substring(0, handlerRelativeUrl.Length - 1);
            }

            return handlerRelativeUrl;
        }

        /// <summary>
        /// Normalizes an HTTP verb in preparation for route matching.
        /// </summary>
        /// <param name="verb">The HTTP verb string to normalize.</param>
        /// <returns>A normalized HTTP verb string.</returns>
        public static string NormalizeVerb(string verb)
        {
            return (!string.IsNullOrEmpty(verb) ? verb : "GET").ToUpperInvariant();
        }
    }
}
