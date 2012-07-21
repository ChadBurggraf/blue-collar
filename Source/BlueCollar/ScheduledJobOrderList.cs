//-----------------------------------------------------------------------
// <copyright file="ScheduledJobOrderList.cs" company="Tasty Codes">
//     Copyright (c) 2012 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a list of scheduled job orders.
    /// </summary>
    public sealed class ScheduledJobOrderList
    {
        private List<ScheduledJobOrderRecord> numbers;

        /// <summary>
        /// Gets the order list.
        /// </summary>
        public IList<ScheduledJobOrderRecord> Numbers
        {
            get { return this.numbers ?? (this.numbers = new List<ScheduledJobOrderRecord>()); }
        }
    }
}
