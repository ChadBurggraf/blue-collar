

namespace BlueCollar.Examples.Mvc
{
    using System;
    using System.Configuration;
    using System.Globalization;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using BlueCollar;

    public class MvcApplication : HttpApplication
    {
        private static Machine machine;

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("collar/{*pathInfo}");
            
            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}", 
                new { controller = "Home", action = "Index", id = UrlParameter.Optional });
        }

        protected void Application_End()
        {
            if (machine != null)
            {
                machine.Dispose();
                machine = null;
            }
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            if (!BlueCollarSection.Section.Machine.ServiceExecutionEnabled)
            {
                // Use NLog.config file to configure the NLog logger.
                machine = new Machine(new NLogger());
            }
        }
    }
}