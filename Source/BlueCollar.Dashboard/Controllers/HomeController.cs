//-----------------------------------------------------------------------
// <copyright file="HomeController.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Dashboard.Controllers
{
    using System;
    using System.Web.Mvc;
    using BlueCollar.Dashboard.Models;

    /// <summary>
    /// Implements the home controller.
    /// </summary>
    public sealed class HomeController : BlueCollarController
    {
        /// <summary>
        /// GET implementation of the index action.
        /// </summary>
        /// <returns>The action result.</returns>
        public ActionResult Index()
        {
            return Get<HomeIndex>();
        }

        /// <summary>
        /// GET implementation of the test action.
        /// </summary>
        /// <returns>The action result.</returns>
        public ActionResult Test()
        {
            return Get<HomeTest>();
        }
    }
}