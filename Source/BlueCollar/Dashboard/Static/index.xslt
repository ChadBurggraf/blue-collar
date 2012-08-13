<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt"
                exclude-result-prefixes="msxsl">
  <xsl:output method="html" indent="yes" encoding="utf-8"/>

  <xsl:variable name="CssUrl" select="/Index/CssUrl"/>
  <xsl:variable name="Html5JSUrl" select="/Index/Html5JSUrl"/>
  <xsl:variable name="JSUrl" select="/Index/JSUrl"/>
  <xsl:variable name="LogoHeaderUrl" select="/Index/LogoHeaderUrl"/>
  <xsl:variable name="UrlRoot" select="/Index/UrlRoot"/>

  <xsl:template match="/Index">
    <html lang="en">
      <head>
        <title>Blue Collar v<xsl:value-of select="Version"/></title>
        <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
        <link rel="stylesheet" href="{$CssUrl}"/>
        <xsl:text disable-output-escaping="yes">&lt;!--[if lt IE 9]&gt;</xsl:text>
        <script src="{$Html5JSUrl}"></script>
        <xsl:text disable-output-escaping="yes">&lt;![endif]--&gt;</xsl:text>
      </head>
      <body>
        <div class="navbar">
          <div class="navbar-inner">
            <div class="container">
              <a class="btn btn-navbar" data-toggle="collapse" data-target=".nav-collapse">
                <span class="icon-bar"></span>
                <span class="icon-bar"></span>
                <span class="icon-bar"></span>
              </a>
              <a class="brand" href="{$UrlRoot}">
                <img src="{$LogoHeaderUrl}" width="32" height="32" alt=""/>
                <span>Blue Collar</span>
              </a>
              <div class="nav-collapse">
                <ul id="nav" class="nav"></ul>
              </div>
            </div>
          </div>
        </div>

        <div id="page" class="container container-page"></div>

        <div class="container">
          <footer class="footer">
            <p>Built with care by <a href="http://github.com/ChadBurggraf">Chad Burggraf</a>.</p>
            <p>Licensed under the <a href="http://opensource.org/licenses/mit-license.php">MIT License</a>.</p>
          </footer>
        </div>

        <xsl:value-of disable-output-escaping="yes" select="TemplatesHtml"/>

        <script type="text/javascript" src="{$JSUrl}"></script>
        <script type="text/javascript">
          var app;
          $(function() {
            app = new App('<xsl:value-of select="ApplicationName"/>', '<xsl:value-of select="UrlRoot"/>', {
              chartsLoaded: false,
              counts: <xsl:value-of select="CountsJson"/>
            });
          });
        </script>
        <script type="text/javascript">
          <xsl:attribute name="src">
            <xsl:value-of disable-output-escaping="yes" select="JSApiUrl"/>
          </xsl:attribute>
        </script>
        <script type="text/javascript">
          google.setOnLoadCallback(function() {
            app.setChartsLoaded(true);
          });
        </script>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>