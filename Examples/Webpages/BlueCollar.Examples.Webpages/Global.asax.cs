namespace BlueCollar.Examples.Webpages
{
    using System;
    using System.Web;
    using BlueCollar;

    public class Global : HttpApplication
    {
        private static Machine machine;

        protected void Application_Start(object sender, EventArgs e)
        {
            if (!BlueCollarSection.Section.Machine.ServiceExecutionEnabled)
            {
                // Use NLog.config file to configure the NLog logger.
                machine = new Machine(new NLogger());
            }
        }
    }
}