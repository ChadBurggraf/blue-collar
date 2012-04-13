//-----------------------------------------------------------------------
// <copyright file="BlueCollarController.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Dashboard.Controllers
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Web;
    using System.Web.Mvc;
    using BlueCollar.Dashboard.Models;

    /// <summary>
    /// Base Blue Collar controller implementation.
    /// </summary>
    public abstract class BlueCollarController : Controller
    {
        /// <summary>
        /// Throws a 404 <see cref="HttpException"/>.
        /// </summary>
        /// <returns>The action result.</returns>
        protected static ActionResult Throw404Result()
        {
            throw new HttpException(404, "Not found");
        }

        /// <summary>
        /// Basic implementation of a GET action.
        /// </summary>
        /// <typeparam name="T">The model type to create and fill.</typeparam>
        /// <returns>The action result.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "No such design exists.")]
        protected ActionResult Get<T>() where T : IPageViewModel, new()
        {
            T model = new T();

            using (IRepository repository = new ConfigurationRepositoryFactory().Create())
            {
                if (model.Fill(repository, ModelState))
                {
                    return View(model);
                }
            }

            return Throw404Result();
        }
    }
}