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
        /// Gets the contents of the file.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Array represents raw byte data.")]
        public byte[] Contents
        {
            get { return this.contents ?? (this.contents = GetContents(this.FileName)); }
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
        /// Gets or sets the original file name.
        /// </summary>
        public string FileName { get; set; }

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
            get { return this.hash ?? (this.hash = GetHash(this.FileName)); }
        }

        /// <summary>
        /// Gets the file's name, without the extension.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the file's URL, using <see cref="UrlRoot"/> as its base and
        /// using <see cref="FileNameWithHash"/> as the file name.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Easier format to deal with for this use case.")]
        public string Url
        {
            get { return this.url ?? (this.url = string.Concat(this.UrlRoot, "/", this.FileNameWithHash)); }
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
        /// <param name="fileName">The file name of the embedded static file.</param>
        /// <returns>A new <see cref="StaticFile"/> instance.</returns>
        [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Lowercase is appropriate for URLs.")]
        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", Justification = "Easier format to deal with for this use case.")]
        public static StaticFile Create(string urlRoot, string fileName)
        {
            if (string.IsNullOrEmpty(urlRoot))
            {
                throw new ArgumentNullException("urlRoot", "urlRoot must contain a value.");
            }

            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException("fileName", "fileName must contain a value.");
            }

            if (Regex.IsMatch(urlRoot, "^~/"))
            {
                urlRoot = urlRoot.Substring(1);
            }

            if (urlRoot.LastIndexOf("/", StringComparison.Ordinal) == urlRoot.Length - 1)
            {
                urlRoot = urlRoot.Substring(0, urlRoot.Length - 1);
            }

            if (Regex.IsMatch(fileName, "^~/"))
            {
                fileName = fileName.Substring(2);
            }

            urlRoot = urlRoot.ToLowerInvariant();
            fileName = fileName.ToLowerInvariant();
            string ext = Path.GetExtension(fileName);
            
            return new StaticFile()
            {
                ContentType = GetContentType(ext),
                Extension = ext,
                FileName = fileName,
                Name = Path.GetFileNameWithoutExtension(fileName),
                UrlRoot = urlRoot
            };
        }

        /// <summary>
        /// Loads the contents of an embedded static file and converts it to a string.
        /// </summary>
        /// <param name="fileName">The name of the file to load.</param>
        /// <returns>The loaded file contents as a string.</returns>
        public static string GetContentsAsString(string fileName)
        {
            return Encoding.UTF8.GetString(GetContents(fileName));
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
                case ".EOT":
                    return "application/vnd.bw-fontobject";
                case ".SVG":
                    return "image/svg+xml";
                case ".TTF":
                    return "application/x-font-ttf";
                case ".WOFF":
                    return "application/x-woff";
                default:
                    return "application/octet-stream";
            }
        }

        /// <summary>
        /// Gets the hash value of the current contents of the embedded static file with the given name.
        /// </summary>
        /// <param name="fileName">The name of the file to get the hash of.</param>
        /// <returns>A file hash.</returns>
        [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Lowercase is appropriate for MD5 hashes.")]
        public static string GetHash(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException("fileName", "fileName must contain a value.");
            }

            string key = string.Concat("BlueCollar.Dashboard.StaticFileHandler.Hash.", fileName).ToUpperInvariant();
            string hash = HttpRuntime.Cache[key] as string;

            if (string.IsNullOrEmpty(hash))
            {
                StringBuilder hex = new StringBuilder();
                byte[] buffer;

                using (MD5 hasher = MD5.Create())
                {
                    buffer = hasher.ComputeHash(GetContents(fileName));
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
        /// <param name="fileName">The name of the file to load.</param>
        /// <returns>The loaded file contents.</returns>
        private static byte[] GetContents(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException("fileName", "fileName must contain a value.");
            }

            byte[] buffer;

            using (Stream stream = typeof(StaticFileHandler).Assembly.GetManifestResourceStream("BlueCollar.Dashboard.Static." + fileName))
            {
                if (stream == null)
                {
                    throw new FileNotFoundException("The specified file is not a valid embedded static file resource.", fileName);
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
