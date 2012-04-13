//-----------------------------------------------------------------------
// <copyright file="HomeTest.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Dashboard.Models
{
    using System;

    /// <summary>
    /// View model for Home/Test actions.
    /// </summary>
    public sealed class HomeTest : PageViewModelBase
    {
        /// <summary>
        /// Gets the page's CSS class name.
        /// </summary>
        public override string ClassName
        {
            get { return "home-test"; }
        }

        /// <summary>
        /// Gets the page's title.
        /// </summary>
        public override string Title
        {
            get { return "Tests"; }
        }
    }
}