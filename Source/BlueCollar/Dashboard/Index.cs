//-----------------------------------------------------------------------
// <copyright file="Index.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Dashboard
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Caching;
    using System.Web.UI;
    using System.Xml;
    using System.Xml.Serialization;
    using System.Xml.XPath;
    using System.Xml.Xsl;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents the data used to fill in values when transforming
    /// the Static/index.xslt stylesheet into HTML.
    /// </summary>
    public sealed class Index
    {
        private StaticFile file;

        /// <summary>
        /// Initializes a new instance of the Index class.
        /// </summary>
        public Index()
            : this(StaticFile.Create(BlueCollarSection.Section.Dashboard.HandlerUrl, "index.xslt"), new StatisticsRecord())
        {
        }

        /// <summary>
        /// Initializes a new instance of the Index class.
        /// </summary>
        /// <param name="file">The <see cref="StaticFile"/> instance representing the index XSLT stylesheet.</param>
        /// <param name="stats">The stats record to initialize this instance with.</param>
        public Index(StaticFile file, StatisticsRecord stats)
        {
            if (file == null)
            {
                throw new ArgumentNullException("file", "file cannot be null.");
            }

            if (!"index.xslt".Equals(file.FileName, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("file must represent index.xslt.", "file");
            }

            if (stats == null)
            {
                throw new ArgumentNullException("stats", "stats cannot be null.");
            }

            this.file = file;

            this.BlueCollarCssUrl = StaticFile.Create(this.file.UrlRoot, "bc.css").Url;
            this.BlueCollarJSUrl = StaticFile.Create(this.file.UrlRoot, "bc.js").Url;
            this.Html5JSUrl = StaticFile.Create(this.file.UrlRoot, "html5.js").Url;
            this.StatsJson = JsonConvert.SerializeObject(stats);
            this.TemplatesHtml = GetTemplatesHtml();
            this.UrlRoot = file.UrlRoot;
            this.Version = typeof(Index).Assembly.GetName().Version.ToString(3);
        }

        /// <summary>
        /// Gets or sets the URL to the primary CSS file.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Easier format to deal with for this use case.")]
        public string BlueCollarCssUrl { get; set; }

        /// <summary>
        /// Gets or sets the URL to the primary JS file.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Easier format to deal with for this use case.")]
        public string BlueCollarJSUrl { get; set; }

        /// <summary>
        /// Gets or sets the URL to the HTML5 JS file.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Easier format to deal with for this use case.")]
        public string Html5JSUrl { get; set; }

        /// <summary>
        /// Gets or sets the JSON string representing the current system stats.
        /// </summary>
        public string StatsJson { get; set; }

        /// <summary>
        /// Gets or sets the Undersocre.js templates as a string.
        /// </summary>
        public string TemplatesHtml { get; set; }

        /// <summary>
        /// Gets or sets the URL root used when resolving URLs.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Easier format to deal with for this use case.")]
        public string UrlRoot { get; set; }

        /// <summary>
        /// Gets or sets the current system version string.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Serializes this instance to XML.
        /// </summary>
        /// <returns>The serialized XML.</returns>
        public IXPathNavigable ToXml()
        {
            XmlSerializer serializer = new XmlSerializer(GetType());
            StringBuilder sb = new StringBuilder();
            StringWriter sw = null;

            try
            {
                sw = new StringWriter(sb, CultureInfo.InvariantCulture);

                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    sw = null;
                    serializer.Serialize(htw, this);
                }
            }
            finally
            {
                if (sw != null)
                {
                    sw.Dispose();
                }
            }

            XmlDocument document = new XmlDocument();
            document.LoadXml(sb.ToString());

            return document;
        }

        /// <summary>
        /// Transforms this instance into an HTML string using the embedded XSLT stylesheet.
        /// </summary>
        /// <returns>A string of transformed HTML.</returns>
        public string Transform()
        {
            XmlDocument stylesheet = new XmlDocument();
            MemoryStream outputStream = null;
            string html;
            
            using (Stream stream = GetType().Assembly.GetManifestResourceStream("BlueCollar.Dashboard.Static.index.xslt"))
            {
                stylesheet.Load(stream);
            }

            XslCompiledTransform transform = new XslCompiledTransform();
            transform.Load(stylesheet);

            try
            {
                outputStream = new MemoryStream();
                transform.Transform(this.ToXml(), null, outputStream);
                outputStream.Position = 0;

                using (StreamReader reader = new StreamReader(outputStream))
                {
                    outputStream = null;
                    html = reader.ReadToEnd();
                }
            }
            finally
            {
                if (outputStream != null)
                {
                    outputStream.Dispose();
                }
            }

            const string Boilerplate =
@"<!DOCTYPE html>
<!--[if lte IE 7]><html class=""no-js ie7"" lang=""en""><![endif]-->
<!--[if IE 8]><html class=""no-js ie8"" lang=""en""><![endif]-->
<!--[if gt IE 8]><!--><html class=""no-js"" lang=""en""><!--<![endif]-->";

            // Replace the opening HTML with the HTML5 Boilerplate stuff.
            html = html.Replace("<html>", Boilerplate);

            return html;
        }

        /// <summary>
        /// Gets the templates HTML as a string.
        /// </summary>
        /// <returns>The templates HTML as a string.</returns>
        private static string GetTemplatesHtml()
        {
            const string Key = "BlueCollar.Dashboard.Index.Templates";
            string templates = HttpRuntime.Cache[Key] as string;

            if (string.IsNullOrEmpty(templates))
            {
                templates = StaticFile.GetContentsAsString("templates.html");

                HttpRuntime.Cache.Add(
                    Key,
                    templates,
                    null,
                    Cache.NoAbsoluteExpiration,
                    Cache.NoSlidingExpiration,
                    CacheItemPriority.Normal,
                    null);
            }

            return templates;
        }
    }
}