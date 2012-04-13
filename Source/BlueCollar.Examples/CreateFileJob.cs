//-----------------------------------------------------------------------
// <copyright file="CreateFileJob.cs" company="Tasty Codes">
//     Copyright (c) 2012 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Examples
{
    using System;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Create file job.
    /// </summary>
    public sealed class CreateFileJob : Job
    {
        private string path;

        /// <summary>
        /// Gets the display name of the job.
        /// </summary>
        public override string Name
        {
            get { return "Create File"; }
        }

        /// <summary>
        /// Gets or sets the path of the file to create.
        /// </summary>
        public string Path
        {
            get
            {
                if (string.IsNullOrEmpty(this.path))
                {
                    this.path = System.IO.Path.GetRandomFileName();
                }

                if (!System.IO.Path.IsPathRooted(this.path))
                {
                    this.path = System.IO.Path.GetFullPath(this.path);
                }

                return this.path;
            }

            set
            {
                this.path = value;
            }
        }

        /// <summary>
        /// Executes the job.
        /// </summary>
        public override void Execute()
        {
            if (!File.Exists(this.Path))
            {
                File.AppendAllText(this.Path, "Hello, world!", Encoding.UTF8);
            }
        }
    }
}
