//-----------------------------------------------------------------------
// <copyright file="StyleSheetHandler.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Dashboard
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;

    /// <summary>
    /// Extens <see cref="StaticFileHandler"/> to handle requests for style sheets.
    /// </summary>
    public sealed class StyleSheetHandler : StaticFileHandler
    {
        /// <summary>
        /// Initializes a new instance of the StyleSheetHandler class.
        /// </summary>
        /// <param name="repositoryFactory">The repository factory to use.</param>
        public StyleSheetHandler(IRepositoryFactory repositoryFactory)
            : base(repositoryFactory)
        {
        }

        /// <summary>
        /// Performs the concrete request operation and returns the output as a byte array.
        /// </summary>
        /// <param name="context">The HTTP context to perform the request for.</param>
        /// <param name="file">The static file to return the contents of.</param>
        /// <returns>The response to write.</returns>
        protected override byte[] PerformRequest(HttpContextBase context, StaticFile file)
        {
            string contents = Regex.Replace(
                Encoding.UTF8.GetString(file.Contents),
                @"url\(('|"")?([^'""#?\)]+)((#|\?)[^""'\)]+)?('|"")?\)", 
                m =>
                {
                    try
                    {
                        return string.Format(
                            CultureInfo.InvariantCulture,
                            "url({0}{1}{2}{3})",
                            m.Groups[1].Value,
                            StaticFile.Create(file.UrlRoot, m.Groups[2].Value).Url,
                            m.Groups[3].Value,
                            m.Groups[5].Value);
                    }
                    catch (FileNotFoundException)
                    {
                        return m.Groups[0].Value;
                    }
                });

            return EncodeString(contents);
        }
    }
}
