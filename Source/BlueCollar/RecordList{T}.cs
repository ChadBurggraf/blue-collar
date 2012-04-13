//-----------------------------------------------------------------------
// <copyright file="RecordList{T}.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a list of records.
    /// </summary>
    /// <typeparam name="T">The record type the list contains.</typeparam>
    public class RecordList<T> where T : class
    {
        private List<T> records;

        /// <summary>
        /// Gets or sets the current system counts.
        /// </summary>
        public CountsRecord Counts { get; set; }

        /// <summary>
        /// Gets the record list.
        /// </summary>
        public IList<T> Records
        {
            get { return this.records ?? (this.records = new List<T>()); }
        }

        /// <summary>
        /// Gets or sets the total number of pages in the list.
        /// </summary>
        public long PageCount { get; set; }

        /// <summary>
        /// Gets or sets the page number this instance represents.
        /// </summary>
        public long PageNumber { get; set; }

        /// <summary>
        /// Gets or sets the total number of records in the list.
        /// </summary>
        public long TotalCount { get; set; }

        /// <summary>
        /// Sets this instance's paging values.
        /// </summary>
        /// <param name="totalCount">The total record count to set.</param>
        /// <param name="limit">The paging limit used.</param>
        /// <param name="offset">The paging offset used.</param>
        public void SetPaging(long totalCount, int limit, int offset)
        {
            if (totalCount < 0)
            {
                totalCount = 0;
            }

            if (limit < 0)
            {
                limit = 0;
            }

            if (offset < 0)
            {
                offset = 0;
            }

            this.PageCount = limit > 0 ? (long)Math.Ceiling((double)totalCount / limit) : 1;
            this.PageNumber = limit > 0 ? (offset / limit) + 1 : 1;
            this.TotalCount = totalCount;

            if (this.PageCount < 1)
            {
                this.PageCount = 1;
            }

            if (this.PageNumber < 1)
            {
                this.PageNumber = 1;
            }
        }
    }
}
