//-----------------------------------------------------------------------
// <copyright file="HomeIndex.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Dashboard.Models
{
    using System;
    using System.Web.Mvc;
    using Newtonsoft.Json;

    /// <summary>
    /// View model for Home/Index actions.
    /// </summary>
    public sealed class HomeIndex : PageViewModelBase
    {
        /// <summary>
        /// Gets the page's CSS class name.
        /// </summary>
        public override string ClassName
        {
            get { return string.Empty; }
        }

        /// <summary>
        /// Gets a json string of system statistics.
        /// </summary>
        public string StatsJson { get; private set; }

        /// <summary>
        /// Gets the page's title.
        /// </summary>
        public override string Title
        {
            get { return "Dashboard"; }
        }

        /// <summary>
        /// Fills the model with data.
        /// </summary>
        /// <param name="repository">The repository to use.</param>
        /// <param name="modelState">The model state dictionary to add errors to.</param>
        /// <returns>True if the fill was successful, false otherwise.</returns>
        public override bool Fill(IRepository repository, ModelStateDictionary modelState)
        {
            if (repository == null)
            {
                throw new ArgumentNullException("repository", "repository cannot be null.");
            }

            bool success = base.Fill(repository, modelState);

            if (success)
            {
                DateTime value = DateTime.UtcNow;
                DateTime now = new DateTime(value.Ticks - (value.Ticks % TimeSpan.TicksPerSecond), value.Kind);

                this.StatsJson = JsonConvert.SerializeObject(
                    repository.GetStatistics(
                        BlueCollarSection.Section.ApplicationName,
                        now.AddDays(-1),
                        now.AddDays(-14),
                        now,
                        null));
            }

            return success;
        }
    }
}