using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using CefSharp;

namespace browser
{
    public class ManifestResourceHandler : IRequestHandler
    {
        readonly IApp _app;
        public ManifestResourceHandler(IApp app) : base() {
            _app = app;
        }

        public bool GetAuthCredentials(IWebBrowser browser, bool isProxy, string host, int port,
            string realm, string scheme, ref string username, ref string password)
        {
            return false;
        }

        public bool GetDownloadHandler(IWebBrowser browser, string mimeType,
            string fileName, long contentLength, ref IDownloadHandler handler)
        {
            return false;
        }

        public bool OnBeforeBrowse(IWebBrowser browser, IRequest request,
            NavigationType naigationvType, bool isRedirect)
        {
            return false;
        }

        public bool OnBeforeResourceLoad(IWebBrowser browser, IRequestResponse requestResponse)
        {
            string requestURL = requestResponse.Request.Url, _lower = requestURL.ToLower();

            switch (_lower)
            {
                case "https://www.gstatic.com/images/icons/material/anim/mspin/mspin_googblue_medium.css":
                    using (FileStream stream = File.OpenRead("mspin_googblue_medium.css"))
                        requestResponse.RespondWith(stream, "text/css");
                    return true;
                case "https://www.google-analytics.com/analytics.js":
                    using (FileStream stream = File.OpenRead("analytics.js"))
                        requestResponse.RespondWith(stream, "text/javascript");
                    return true;
            }

            if (_app.appInfo.hasWriteLog) _app.writeLog(requestURL);
            return false;
        }

        public void OnResourceResponse(IWebBrowser browser, string url, int status, string statusText,
            string mimeType, System.Net.WebHeaderCollection headers)
        { }
    }

    public class ExternalLifeSpanHandler : ILifeSpanHandler
    {
        public void OnBeforeClose(IWebBrowser browser) { }
        public bool OnBeforePopup(IWebBrowser browser, string url, ref int x, ref int y, ref int width, ref int height)
        {
            if (!string.IsNullOrEmpty(url) && url.Contains("chrome-devtools://devtools/devtools.html")) return false;

            try
            {
                //System.Diagnostics.Process.Start(url);
            }
            catch { }
            return true;
        }
    }
}
