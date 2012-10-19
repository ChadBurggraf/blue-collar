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
            : this(BlueCollarSection.Section.ApplicationName, StaticFile.Create(BlueCollarSection.Section.Dashboard.HandlerUrl, "index.xslt"), new CountsRecord())
        {
        }

        /// <summary>
        /// Initializes a new instance of the Index class.
        /// </summary>
        /// <param name="applicationName">The name of the current application.</param>
        /// <param name="file">The <see cref="StaticFile"/> instance representing the index XSLT stylesheet.</param>
        /// <param name="counts">The counts record to initialize this instance with.</param>
        public Index(string applicationName, StaticFile file, CountsRecord counts)
        {
            if (string.IsNullOrEmpty(applicationName))
            {
                throw new ArgumentNullException("applicationName", "applicationName must contain a value.");
            }

            if (file == null)
            {
                throw new ArgumentNullException("file", "file cannot be null.");
            }

            if (!"index.xslt".Equals(file.OriginalPath, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("file must represent index.xslt.", "file");
            }

            if (counts == null)
            {
                throw new ArgumentNullException("counts", "counts cannot be null.");
            }

            this.ApplicationName = applicationName;
            this.file = file;

            this.BootstrapResponsiveUrl = StaticFile.Create(this.file.UrlRoot, "css/bootstrap-responsive.css").Url;
            this.BootstrapUrl = StaticFile.Create(this.file.UrlRoot, "css/bootstrap.css").Url;
            this.CountsJson = JsonConvert.SerializeObject(counts);
            this.CssUrl = StaticFile.Create(this.file.UrlRoot, "css/collar.css").Url;
            this.Html5JSUrl = StaticFile.Create(this.file.UrlRoot, "js/html5.js").Url;
            this.JSApiUrl = "//www.google.com/jsapi?autoload={'modules':[{'name':'visualization','version':'1','packages':['corechart']}]}";
            this.JSUrl = StaticFile.Create(this.file.UrlRoot, "js/collar.js").Url;
            this.LogoHeaderUrl = StaticFile.Create(this.file.UrlRoot, "img/logo-header.png").Url;
            this.TemplatesHtml = GetTemplatesHtml();
            this.UrlRoot = file.UrlRoot;
            this.Version = typeof(Index).Assembly.GetName().Version.ToString(2);
        }

        /// <summary>
        /// Gets or sets the name of the current application.
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// Gets or sets the URL to the bootstrap responsive CSS file.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Easier format to deal with for this use case.")]
        public string BootstrapResponsiveUrl { get; set; }

        /// <summary>
        /// Gets or sets the URL to the bootstrap CSS file.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Easier format to deal with for this use case.")]
        public string BootstrapUrl { get; set; }

        /// <summary>
        /// Gets or sets the JSON string representing the current system counts.
        /// </summary>
        public string CountsJson { get; set; }

        /// <summary>
        /// Gets or sets the URL to the primary CSS file.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Easier format to deal with for this use case.")]
        public string CssUrl { get; set; }

        /// <summary>
        /// Gets or sets the URL to the HTML5 JS file.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Easier format to deal with for this use case.")]
        public string Html5JSUrl { get; set; }

        /// <summary>
        /// Gets or sets the URL to the Google JSAPI script.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Easier format to deal with for this use case.")]
        public string JSApiUrl { get; set; }

        /// <summary>
        /// Gets or sets the URL to the primary JS file.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Easier format to deal with for this use case.")]
        public string JSUrl { get; set; }

        /// <summary>
        /// Gets or sets the URL to the logo header image.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Easier format to deal with for this use case.")]
        public string LogoHeaderUrl { get; set; }

        /// <summary>
        /// Gets or sets the Underscore.js templates as a string.
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

            return "<!DOCTYPE html>\r\n" + html;
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
                templates = StaticFile.GetContentsAsString("BlueCollar.Dashboard.Static.templates.html");

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