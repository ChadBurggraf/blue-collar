<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="BlueCollar.Examples.Webpages._Default" %>
<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="utf-8"/>
    <title>Machine Log (Blue Collar)</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
    <link rel="stylesheet" href="~/assets/css/bootstrap.css"/>
    <link rel="stylesheet" href="~/assets/css/bootstrap-responsive.css"/>
    <link rel="Stylesheet" href="~/assets/css/bluecollar-webpages.css"/>
    <!--[if lt IE 9]
    <script src="http://html5shim.googlecode.com/svn/trunk/html5.js"></script>
    <![endif]-->
</head>
<body>
    <div class="container">
        <div class="page-header">
            <h1>Machine Log</h1>
        </div>

        <p>
            Viewing the tail of the Blue Collar machine log located at
            <code>~/App_Data/BlueCollar.log</code>. Blue Collar management 
            is configured at <a href="collar.ashx" target="_blank">~/collar.ashx</a>.
        </p>
        
        <% if (Errors.Count > 0)
           { %>
        <div class="validation-summary-errors alert alert-block alert-error">
            <span>Whoops</span>
            <ul>
            <% foreach (string error in Errors)
               { %>
                <li><%= error %></li>
            <% } %>
            </ul>
        </div>
        <% } %>
        
        <% if (Entries.Count > 0)
           { %>
        <table class="table">
            <thead>
                <tr>
                    <th>Date</th>
                    <th>Level</th>
                    <th>Message</th>
                </tr>
            </thead>
            <tbody>
            <% foreach (LogEntry entry in Entries)
               { %>
                <tr>
                    <td>
                        <div class="date-time">
                            <span class="date"><%= entry.Date.ToString("MMM d")%></span>
                            <span class="time"><%= entry.Date.ToString("h:mm:ss tt") %></span>
                        </div>
                    </td>
                    <td>
                        <span class="<%= CssClassForLogLevel(entry.Level) %>"><%= entry.Level %></span>
                    </td>
                    <td>
                        <pre class="plain"><%= entry.Message %></pre>
                    </td>
                </tr>
            <% } %>
            </tbody>
        </table>
        <% } %>
    </div>
</body>
</html>