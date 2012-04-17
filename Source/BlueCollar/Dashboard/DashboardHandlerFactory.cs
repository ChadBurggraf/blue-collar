//-----------------------------------------------------------------------
// <copyright file="DashboardHandlerFactory.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Dashboard
{
    using System;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Web;

    /// <summary>
    /// Implements <see cref="IHttpHandlerFactory"/> for the dashboard.
    /// </summary>
    public class DashboardHandlerFactory : IHttpHandlerFactory
    {
        /// <summary>
        /// Initializes a new instance of the DashboardHandlerFactory class.
        /// </summary>
        public DashboardHandlerFactory()
            : this(
                BlueCollarSection.Section.ApplicationName, 
                BlueCollarSection.Section.Dashboard.Mode, 
                BlueCollarSection.Section.Dashboard.HandlerUrl,
                new ConfigurationRepositoryFactory())
        {
        }

        /// <summary>
        /// Initializes a new instance of the DashboardHandlerFactory class.
        /// </summary>
        /// <param name="applicationName">The application name.</param>
        /// <param name="mode">The enabled mode.</param>
        /// <param name="handlerUrl">The URL the handler is mapped to.</param>
        /// <param name="repositoryFactory">The repository factory.</param>
        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", Justification = "The easiest format to handle this property in.")]
        public DashboardHandlerFactory(string applicationName, DashboardEnabledMode mode, string handlerUrl, IRepositoryFactory repositoryFactory)
        {
            if (string.IsNullOrEmpty(applicationName))
            {
                throw new ArgumentNullException("applicationName", "applicationName must contain a value.");
            }

            if (string.IsNullOrEmpty(handlerUrl))
            {
                throw new ArgumentNullException("handlerUrl", "handlerUrl must contain a value.");
            }

            if (repositoryFactory == null)
            {
                throw new ArgumentNullException("repositoryFactory", "repositoryFactory cannot be null.");
            }

            this.ApplicationName = applicationName;
            this.Mode = mode;
            this.HandlerUrl = handlerUrl;
            this.RepositoryFactory = repositoryFactory;
        }

        /// <summary>
        /// Gets the application name.
        /// </summary>
        public string ApplicationName { get; private set; }

        /// <summary>
        /// Gets the URL the handler is mapped to.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "The easiest format to handle this property in.")]
        public string HandlerUrl { get; private set; }

        /// <summary>
        /// Gets the enabled mode.
        /// </summary>
        public DashboardEnabledMode Mode { get; private set; }

        /// <summary>
        /// Gets the repository factory.
        /// </summary>
        public IRepositoryFactory RepositoryFactory { get; private set; }

        /// <summary>
        /// Returns an instance of a class that implements the <see cref="IHttpHandler"/> interface.
        /// </summary>
        /// <param name="context">An instance of the <see cref="HttpContext"/> class that provides references to intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests. </param>
        /// <param name="requestType">The HTTP data transfer method (GET or POST) that the client uses.</param>
        /// <param name="url">The <see cref="HttpRequest.RawUrl"/> of the requested resource. </param>
        /// <param name="pathTranslated">The <see cref="HttpRequest.PhysicalApplicationPath"/> to the requested resource. </param>
        /// <returns>A new <see cref="IHttpHandler"/> object that processes the request.</returns>
        public IHttpHandler GetHandler(HttpContext context, string requestType, string url, string pathTranslated)
        {
            return this.GetHandler(new HttpContextWrapper(context), requestType, url, pathTranslated);
        }

        /// <summary>
        /// Returns an instance of a class that implements the <see cref="IHttpHandler"/> interface.
        /// </summary>
        /// <param name="context">An instance of the <see cref="HttpContextBase"/> class that provides references to intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests. </param>
        /// <param name="requestType">The HTTP data transfer method (GET or POST) that the client uses.</param>
        /// <param name="url">The <see cref="HttpRequest.RawUrl"/> of the requested resource. </param>
        /// <param name="pathTranslated">The <see cref="HttpRequest.PhysicalApplicationPath"/> to the requested resource. </param>
        /// <returns>A new <see cref="IHttpHandler"/> object that processes the request.</returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Mock-able overload of interface method.")]
        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", Justification = "Overload of interface method.")]
        public IHttpHandler GetHandler(HttpContextBase context, string requestType, string url, string pathTranslated)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context", "context cannot be null.");
            }

            requestType = requestType ?? "GET";

            if (this.Mode == DashboardEnabledMode.On
                || (this.Mode == DashboardEnabledMode.LocalOnly 
                && context.Request.IsLocal))
            {
                string handlerUrl = ResolveHandlerUrl(this.HandlerUrl, context);
                string handlerRelativeUrl = GetHandlerRelativeUrl(handlerUrl, context.Request.RawUrl);
                QueryString queryString = QueryString.FromUrl(context.Request.Url);
                IDashboardHandler handler = DashboardRouter.GetHandler(requestType, handlerRelativeUrl, this.RepositoryFactory);

                if (handler != null)
                {
                    handler.ApplicationName = this.ApplicationName;
                    handler.HandlerUrl = handlerUrl;
                    handler.HandlerRelativeRequestUrl = handlerRelativeUrl;
                    handler.QueryString = queryString;
                    handler.Verb = requestType.ToUpperInvariant();
                }
                else
                {
                    context.Response.StatusCode = 404;
                    context.Response.End();
                }

                return handler;
            }
            else
            {
                context.Response.StatusCode = 403;
                context.Response.End();
            }

            return null;
        }

        /// <summary>
        /// Enables a factory to reuse an existing handler instance.
        /// </summary>
        /// <param name="handler">The <see cref="IHttpHandler"/> object to reuse. </param>
        public void ReleaseHandler(IHttpHandler handler)
        {
            IDashboardHandler apiHandler = handler as IDashboardHandler;

            if (apiHandler != null && !apiHandler.IsReusable)
            {
                apiHandler.Dispose();
                apiHandler = null;
            }
        }

        /// <summary>
        /// Combines all of the URL path parts given by normalizing their separating '/'.
        /// </summary>
        /// <param name="paths">The path parts to combine.</param>
        /// <returns>The combined URL path.</returns>
        internal static string CombineUrlPaths(params string[] paths)
        {
            StringBuilder sb = new StringBuilder();
            bool rooted = false;

            if (paths != null)
            {
                foreach (string path in paths)
                {
                    string p = (path ?? string.Empty).Trim();

                    if (sb.Length == 0 && p.StartsWith("/", StringComparison.Ordinal))
                    {
                        rooted = true;
                    }

                    while (p.StartsWith("/", StringComparison.Ordinal))
                    {
                        p = p.Substring(1);
                    }

                    if (!string.IsNullOrEmpty(p))
                    {
                        if (sb.Length > 0 && sb[sb.Length - 1] != '/')
                        {
                            sb.Append('/');
                        }

                        sb.Append(p);
                    }
                }
            }

            if (rooted)
            {
                sb.Insert(0, '/');
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets a handler-relative URL from the given handler path and raw URL, using tilde (~) syntax.
        /// </summary>
        /// <param name="handlerPath">The path of the handler handling the request.</param>
        /// <param name="rawUrl">The raw URL.</param>
        /// <returns>A handler-relative URL.</returns>
        [SuppressMessage("Microsoft.Naming", "CA2204:LiteralsShouldBeSpelledCorrectly", MessageId = "blueCollar", Justification = "The spelling is correct.")]
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "handlerUrl", Justification = "The spelling is correct.")]
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "HttpHandler", Justification = "The spelling is correct.")]
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "httpHandlers", Justification = "The spelling is correct.")]
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "webServer", Justification = "The spelling is correct.")]
        internal static string GetHandlerRelativeUrl(string handlerPath, string rawUrl)
        {
            rawUrl = string.IsNullOrEmpty(rawUrl) ? "/" : rawUrl;
            handlerPath = string.IsNullOrEmpty(handlerPath) ? "/" : handlerPath;

            if (handlerPath.StartsWith("~/", StringComparison.Ordinal))
            {
                handlerPath = handlerPath.Substring(1);
            }

            if (handlerPath.Length > rawUrl.Length || !rawUrl.StartsWith(handlerPath, StringComparison.OrdinalIgnoreCase))
            {
                throw new ConfigurationErrorsException(
                    "Ensure that handlerUrl is configured under blueCollar/dashboard to the same URL as the HttpHandler registered under system.web/httpHandlers and system.webServer/handlers.",
                    new ArgumentException("handlerPath must be a substring of rawUrl.", "rawUrl"),
                    BlueCollarSection.Section.ElementInformation.Source,
                    BlueCollarSection.Section.Dashboard.ElementInformation.LineNumber);
            }

            rawUrl = rawUrl.Substring(handlerPath.Length);
            int queryIndex = rawUrl.IndexOf('?');

            if (queryIndex >= 0)
            {
                rawUrl = rawUrl.Substring(0, queryIndex);
            }

            if (!rawUrl.StartsWith("/", StringComparison.Ordinal))
            {
                rawUrl = "/" + rawUrl;
            }

            return "~" + rawUrl;
        }

        /// <summary>
        /// Resolves the given handler URL.
        /// </summary>
        /// <param name="handlerUrl">The URL of the handler to resolve.</param>
        /// <param name="httpContext">The HTTP context to use when resolving the URL.</param>
        /// <returns>The resolved handler URL.</returns>
        internal static string ResolveHandlerUrl(string handlerUrl, HttpContextBase httpContext)
        {
            return ResolveHandlerUrl(handlerUrl, httpContext, BlueCollarSection.Section.ElementInformation.Source);
        }

        /// <summary>
        /// Resolves the given handler URL.
        /// </summary>
        /// <param name="handlerUrl">The URL of the handler to resolve.</param>
        /// <param name="httpContext">The HTTP context to use when resolving the URL.</param>
        /// <param name="configPath">The filesystem path of the configuration file to use when resolving the URL.</param>
        /// <returns>The resolved handler URL.</returns>
        internal static string ResolveHandlerUrl(string handlerUrl, HttpContextBase httpContext, string configPath)
        {
            return ResolveHandlerUrl(handlerUrl, AppDomain.CurrentDomain.BaseDirectory, httpContext.Request.ApplicationPath, configPath);
        }

        /// <summary>
        /// Resolves the given handler URL.
        /// </summary>
        /// <param name="handlerUrl">The URL of the handler to resolve.</param>
        /// <param name="baseDirectory">The base directory of the application to resolve the handler for.</param>
        /// <param name="applicationPath">The URL path of the application to resolve the handler for.</param>
        /// <param name="configPath">The filesystem path of the configuration file to use when resolving the URL.</param>
        /// <returns>The resolved handler URL.</returns>
        internal static string ResolveHandlerUrl(string handlerUrl, string baseDirectory, string applicationPath, string configPath)
        {
            handlerUrl = (handlerUrl ?? string.Empty).Trim();
            
            if (handlerUrl.StartsWith("~/", StringComparison.Ordinal))
            {
                return CombineUrlPaths(applicationPath, handlerUrl.Substring(2));
            }
            else if (!handlerUrl.StartsWith("/", StringComparison.Ordinal))
            {
                if (configPath.StartsWith(baseDirectory, StringComparison.Ordinal))
                {
                    if (baseDirectory.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
                    {
                        baseDirectory = baseDirectory.Substring(0, baseDirectory.Length - 1);
                    }

                    string subPath = Path.GetDirectoryName(configPath)
                        .Substring(baseDirectory.Length)
                        .Replace(Path.DirectorySeparatorChar, '/');

                    handlerUrl = CombineUrlPaths(applicationPath, subPath, handlerUrl);
                }
                else
                {
                    throw new ConfigurationErrorsException(
                        string.Format(CultureInfo.InvariantCulture, "The configuration file must be underneath the application root in order to resolve the Blue Collar handler URL relative to the configuration file (i.e., not rooted or application-relative). The handler URL is set to '{0}', the application base is '{1}' and the configuration file is located at '{2}'", handlerUrl, baseDirectory, configPath),
                        BlueCollarSection.Section.ElementInformation.Source,
                        BlueCollarSection.Section.Dashboard.ElementInformation.LineNumber);
                }
            }

            return handlerUrl;
        }
    }
}
