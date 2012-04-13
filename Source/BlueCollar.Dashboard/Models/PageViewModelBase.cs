//-----------------------------------------------------------------------
// <copyright file="PageViewModelBase.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Dashboard.Models
{
    using System;
    using System.Globalization;
    using System.Web.Mvc;
    using BlueCollar;
    using Newtonsoft.Json;

    /// <summary>
    /// Base implementation of <see cref="IPageViewModel"/>.
    /// </summary>
    public abstract class PageViewModelBase : IPageViewModel
    {
        /// <summary>
        /// Gets the page's CSS class name.
        /// </summary>
        public abstract string ClassName { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the model
        /// is the result of an HTTP POST action.
        /// </summary>
        public virtual bool IsPosted { get; set; }

        /// <summary>
        /// Gets the page's title.
        /// </summary>
        public abstract string Title { get; }

        /// <summary>
        /// Gets the current Blue Collar version.
        /// </summary>
        public virtual string Version
        {
            get { return GetType().Assembly.GetName().Version.ToString(3); }
        }

        /// <summary>
        /// Fills the model with data.
        /// </summary>
        /// <param name="repository">The repository to use.</param>
        /// <param name="modelState">The model state dictionary to add errors to.</param>
        /// <returns>True if the fill was successful, false otherwise.</returns>
        public virtual bool Fill(IRepository repository, ModelStateDictionary modelState)
        {
            return true;
        }
    }
}