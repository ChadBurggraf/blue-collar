//-----------------------------------------------------------------------
// <copyright file="StaticFile.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Dashboard
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Caching;

    /// <summary>
    /// Provides access to embedded static files.
    /// </summary>
    public sealed class StaticFile
    {
        private byte[] contents;
        private string hash, fileNameWithHash, url;

        /// <summary>
        /// Prevents a default instance of the StaticFile class from being created.
        /// </summary>
        private StaticFile()
        {
        }

        /// <summary>
        /// Gets the contents of the file.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Array represents raw byte data.")]
        public byte[] Contents
        {
            get { return this.contents ?? (this.contents = GetContents(this.ResourceName)); }
        }

        /// <summary>
        /// Gets the content type to use for the file.
        /// </summary>
        public string ContentType { get; private set; }

        /// <summary>
        /// Gets the file's extension, including the leading ".".
        /// </summary>
        public string Extension { get; private set; }

        /// <summary>
        /// Gets the file's name, including its current hash value.
        /// </summary>
        public string FileNameWithHash
        {
            get { return this.fileNameWithHash ?? (this.fileNameWithHash = string.Concat(this.Name, ".", this.Hash, this.Extension)); }
        }

        /// <summary>
        /// Gets the file's current hash value.
        /// </summary>
        public string Hash
        {
            get { return this.hash ?? (this.hash = GetHash(this.ResourceName)); }
        }

        /// <summary>
        /// Gets the file's name, without the extension.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the file's original full path.
        /// </summary>
        public string OriginalPath { get; private set; }

        /// <summary>
        /// Gets the file's path, not including the file name.
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// Gets the name used to identify the file as an embedded resource.
        /// </summary>
        public string ResourceName { get; private set; }

        /// <summary>
        /// Gets the file's URL, using <see cref="UrlRoot"/> as its base and
        /// using <see cref="FileNameWithHash"/> as the file name.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Easier format to deal with for this use case.")]
        public string Url
        {
            get { return this.url ?? (this.url = string.Concat(this.UrlRoot, "/", this.Path, "/", this.FileNameWithHash)); }
        }

        /// <summary>
        /// Gets the root URL to use when generating static file URLs.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Easier format to deal with for this use case.")]
        public string UrlRoot { get; private set; }

        /// <summary>
        /// Creates a new <see cref="StaticFile"/> instance using the given
        /// root URL and file name.
        /// </summary>
        /// <param name="urlRoot">The root URL to use when generating URLs.</param>
        /// <param name="path">The path of the embedded static file.</param>
        /// <returns>A new <see cref="StaticFile"/> instance.</returns>
        [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Lowercase is appropriate for URLs.")]
        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", Justification = "Easier format to deal with for this use case.")]
        public static StaticFile Create(string urlRoot, string path)
        {
            if (string.IsNullOrEmpty(urlRoot))
            {
                throw new ArgumentNullException("urlRoot", "urlRoot must contain a value.");
            }

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path", "path must contain a value.");
            }

            if (Regex.IsMatch(urlRoot, "^~/"))
            {
                urlRoot = urlRoot.Substring(1);
            }

            if (urlRoot.LastIndexOf("/", StringComparison.Ordinal) == urlRoot.Length - 1)
            {
                urlRoot = urlRoot.Substring(0, urlRoot.Length - 1);
            }

            if (Regex.IsMatch(path, "^~/"))
            {
                path = path.Substring(2);
            }

            path = path.ToLowerInvariant();
            string[] pathParts = path.Split('/');
            string ext = System.IO.Path.GetExtension(path);
            
            return new StaticFile()
            {
                ContentType = GetContentType(ext),
                Extension = ext,
                OriginalPath = path,
                Name = System.IO.Path.GetFileNameWithoutExtension(path),
                Path = pathParts.Length > 1 ? string.Join("/", pathParts.Take(pathParts.Length - 1).ToArray()) : string.Empty,
                ResourceName = string.Concat("BlueCollar.Dashboard.Static.", string.Join(".", pathParts)),
                UrlRoot = urlRoot.ToLowerInvariant()
            };
        }

        /// <summary>
        /// Loads the contents of an embedded static file and converts it to a string.
        /// </summary>
        /// <param name="name">The name of the file to load.</param>
        /// <returns>The loaded file contents as a string.</returns>
        public static string GetContentsAsString(string name)
        {
            return Encoding.UTF8.GetString(GetContents(name));
        }

        /// <summary>
        /// Gets the content-type to use for files with the given extension. Expects
        /// the leading "." to be included with the extension.
        /// </summary>
        /// <param name="extension">The extension to get the content-type for.</param>
        /// <returns>A content-type.</returns>
        public static string GetContentType(string extension)
        {
            if (string.IsNullOrEmpty(extension))
            {
                throw new ArgumentNullException("extension", "extension must contain a value.");
            }

            switch (extension.ToUpperInvariant())
            {
                case ".GIF":
                    return "image/gif";
                case ".PNG":
                    return "image/png";
                case ".CSS":
                    return "text/css";
                case ".JS":
                    return "text/javascript";
                case ".HTML":
                case ".XSLT":
                    return "text/html";
                default:
                    return "application/octet-stream";
            }
        }

        /// <summary>
        /// Gets the hash value of the current contents of the embedded static file with the given name.
        /// </summary>
        /// <param name="name">The name of the file to get the hash of.</param>
        /// <returns>A file hash.</returns>
        [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Lowercase is appropriate for MD5 hashes.")]
        public static string GetHash(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name", "name must contain a value.");
            }

            string key = string.Concat("BlueCollar.Dashboard.StaticFileHandler.Hash.", name).ToUpperInvariant();
            string hash = HttpRuntime.Cache[key] as string;

            if (string.IsNullOrEmpty(hash))
            {
                StringBuilder hex = new StringBuilder();
                byte[] buffer;

                using (MD5 hasher = MD5.Create())
                {
                    buffer = hasher.ComputeHash(GetContents(name));
                }

                foreach (byte b in buffer)
                {
                    hex.Append(b.ToString("X2", CultureInfo.InvariantCulture));
                }

                hash = hex.ToString().ToLowerInvariant();

                HttpRuntime.Cache.Add(
                    key,
                    hash,
                    null,
                    Cache.NoAbsoluteExpiration,
                    Cache.NoSlidingExpiration,
                    CacheItemPriority.Normal,
                    null);
            }

            return hash;
        }

        /// <summary>
        /// Loads the contents of an embedded static file into a buffer and returns it.
        /// </summary>
        /// <param name="name">The name of the file to load.</param>
        /// <returns>The loaded file contents.</returns>
        private static byte[] GetContents(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name", "name must contain a value.");
            }

            byte[] buffer;

            using (Stream stream = typeof(StaticFileHandler).Assembly.GetManifestResourceStream(name))
            {
                if (stream == null)
                {
                    throw new FileNotFoundException("The specified file is not a valid embedded static file resource.", name);
                }

                buffer = new byte[stream.Length];
                int count = 0;

                while (count < buffer.Length)
                {
                    count = stream.Read(buffer, count, buffer.Length);
                }
            }

            return buffer;
        }
    }
}
