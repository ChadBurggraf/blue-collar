//-----------------------------------------------------------------------
// <copyright file="IPageViewModel.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Dashboard.Models
{
    using System;
    using System.Web.Mvc;

    /// <summary>
    /// Interface definition for paged view models.
    /// </summary>
    public interface IPageViewModel
    {
        /// <summary>
        /// Gets the page's CSS class name.
        /// </summary>
        string ClassName { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the model
        /// is the result of an HTTP POST action.
        /// </summary>
        bool IsPosted { get; set; }

        /// <summary>
        /// Gets the page's title.
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Gets the current Blue Collar version.
        /// </summary>
        string Version { get; }

        /// <summary>
        /// Fills the model with data.
        /// </summary>
        /// <param name="repository">The repository to use.</param>
        /// <param name="modelState">The model state dictionary to add errors to.</param>
        /// <returns>True if the fill was successful, false otherwise.</returns>
        bool Fill(IRepository repository, ModelStateDictionary modelState);
    }
}