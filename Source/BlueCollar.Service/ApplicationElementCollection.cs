//-----------------------------------------------------------------------
// <copyright file="ApplicationElementCollection.cs" company="Tasty Codes">
//     Copyright (c) 2012 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Service
{
    using System;
    using System.Configuration;
    using System.Linq;

    /// <summary>
    /// Represents a collection of <see cref="ApplicationElement"/>s.
    /// </summary>
    public sealed class ApplicationElementCollection : ConfigurationElementCollection<ApplicationElement>
    {
        /// <summary>
        /// Gets the element at the given index from the collection.
        /// </summary>
        /// <param name="index">The index of the element to get.</param>
        /// <returns>The element at the specified index.</returns>
        public ApplicationElement this[int index]
        {
            get { return (ApplicationElement)BaseGet(index); }
        }

        /// <summary>
        /// Gets a value indicating whether the collection contains the given item.
        /// </summary>
        /// <param name="item">The item to check for.</param>
        /// <returns>True if the collection contains the item, false otherwise.</returns>
        public override bool Contains(ApplicationElement item)
        {
            return this.Any(e => item.ApplicationPath.Equals(e.ApplicationPath, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets the unique key of the given element.
        /// </summary>
        /// <param name="element">The element to get the key of.</param>
        /// <returns>The given element's key.</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ApplicationElement)element).ApplicationPath;
        }
    }
}
