//-----------------------------------------------------------------------
// <copyright file="IRepositoryFactory.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;

    /// <summary>
    /// Interface definition for <see cref="IRepository"/> factories.
    /// </summary>
    public interface IRepositoryFactory
    {
        /// <summary>
        /// Creates a new <see cref="IRepository"/> instance.
        /// </summary>
        /// <returns>An <see cref="IRepository"/> instance.</returns>
        IRepository Create();
    }
}
