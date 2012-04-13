<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" 
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
                xmlns:msxsl="urn:schemas-microsoft-com:xslt" 
                exclude-result-prefixes="msxsl">
  <xsl:output method="html" indent="yes" encoding="utf-8"/>

  <xsl:variable name="BlueCollarCssUrl" select="/Index/BlueCollarCssUrl"/>
  <xsl:variable name="BlueCollarJSUrl" select="/Index/BlueCollarJSUrl"/>
  <xsl:variable name="Html5JSUrl" select="/Index/Html5JSUrl"/>

  <xsl:template match="/Index">
    <html>
      <head>
        <meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1"/>
        <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
        <title>Blue Collar v<xsl:value-of select="Version"/></title>

        <xsl:text disable-output-escaping="yes">&lt;!--[if lt IE 9]&gt;</xsl:text>
        <script src="{$Html5JSUrl}"></script>
        <xsl:text disable-output-escaping="yes">&lt;![endif]--&gt;</xsl:text>

        <link href="{$BlueCollarCssUrl}" rel="stylesheet"/>

        <script type="text/javascript" src="//www.google.com/jsapi"></script>
        <script type="text/javascript">
          google.load('visualization', '1.0', {'packages':['corechart']});
        </script>
      </head>
      <body>
        <header>
          <div class="container-fluid">
            <div id="logo">
              <h3>Dashboard</h3>
              <p>Blue Collar v<xsl:value-of select="Version"/></p>
            </div>
          </div>
        </header>

        <div class="container-fluid">
          <div class="sidebar">
            <nav role="navigation">
              <ul id="nav"></ul>
            </nav>
          </div>

          <div class="content">
            <table id="layout">
              <tbody>
                <tr>
                  <td id="main"></td>
                  <td id="forms"></td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>

        <footer>
          <div class="container-fluid">
            <a class="badge" href="http://tastycodes.com/" target="_blank">
              <span>A product of Tasty Codes. Delicious software.</span>
            </a>
          </div>
        </footer>

        <xsl:value-of disable-output-escaping="yes" select="TemplatesHtml"/>

        <script type="text/javascript" src="{$BlueCollarJSUrl}"></script>
        <script type="text/javascript">
          function onLoad() {
            $(function() {
              new App('<xsl:value-of select="UrlRoot"/>', {
                stats:<xsl:value-of select="StatsJson"/>
              });
            });
          }

          google.setOnLoadCallback(onLoad);
        </script>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>
