namespace BlueCollar.Examples.NuGet.Controllers
{
    using System;
    using System.Web.Mvc;
    using BlueCollar.Examples.NuGet.Models;

    /// <summary>
    /// Home controller.
    /// </summary>
    public sealed class HomeController : Controller
    {
        /// <summary>
        /// GET implementation of the Index action.
        /// </summary>
        /// <returns>The action result.</returns>
        public ActionResult Index()
        {
            HomeIndex model = new HomeIndex()
            {
                LogPath = Server.MapPath("~/App_Data/BlueCollar.log")
            };

            model.Fill(ModelState, false);
            return View(model);
        }
    }
}
