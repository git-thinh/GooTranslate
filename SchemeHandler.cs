﻿using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using CefSharp;
using Newtonsoft.Json;

namespace browser
{
    public class AppSchemeHandlerFactory : ISchemeHandlerFactory
    {
        readonly IApp _app;
        public AppSchemeHandlerFactory(IApp app) : base()
        {
            _app = app;
        }

        public ISchemeHandler Create()
        {
            return new AppSchemeHandler(_app);
        }
    }

    public class AppSchemeHandler : ISchemeHandler
    {
        readonly IApp _app;
        public AppSchemeHandler(IApp app) : base()
        {
            _app = app;
        }

        Stream GenerateStreamFromString(string[] files)
        {
            string JSON_LIBS___ = "";
            if (_app.appInfo.coreJs != null) JSON_LIBS___ = JsonConvert.SerializeObject(_app.appInfo.coreJs);

            StringBuilder bi = new StringBuilder(string.Empty);
            foreach (string fi in files)
            {
                if (File.Exists(fi))
                {
                    string temp = File.ReadAllText(fi);
                    if (JSON_LIBS___.Length > 0) temp = temp.Replace("JSON_LIBS___", JSON_LIBS___);

                    bi.Append(temp);
                    bi.Append(Environment.NewLine);
                }
            }
            string s = bi.ToString();

            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public bool ProcessRequest(IRequest request, ref string mimeType, ref Stream stream)
        {
            string requestURL = request.Url, _lower = requestURL.ToLower();


            if (_app.appInfo.hasWriteLog) _app.writeLog("----> " + requestURL);

            _lower = _lower.Split(new char[] { '?', '#' })[0];
            string fileName = Path.GetFileName(_lower);

            if (_app.appInfo.coreJs != null
                && fileName == _app.appInfo.coreJs.fileName
                && File.Exists(fileName))
            {
                mimeType = "text/javascript";
                stream = GenerateStreamFromString(new string[] { fileName, "core.js" });
                return true;
            }

            if (File.Exists(fileName))
            {
                if (fileName.EndsWith(".css"))
                {
                    mimeType = "text/css";
                    stream = GenerateStreamFromString(new string[] { fileName });
                    return true;
                }
                else if (fileName.EndsWith(".js"))
                {
                    mimeType = "text/javascript";
                    stream = GenerateStreamFromString(new string[] { fileName });
                    return true;
                }
            }

            return false;
        }
    }

    //////public class ManifestResourceHandler : IRequestHandler
    //////{
    //////    readonly IApp _app;
    //////    public ManifestResourceHandler(IApp app) : base()
    //////    {
    //////        _app = app;
    //////    }

    //////    public bool GetAuthCredentials(IWebBrowser browser, bool isProxy, string host, int port,
    //////        string realm, string scheme, ref string username, ref string password)
    //////    {
    //////        return false;
    //////    }

    //////    public bool GetDownloadHandler(IWebBrowser browser, string mimeType,
    //////        string fileName, long contentLength, ref IDownloadHandler handler)
    //////    {
    //////        return false;
    //////    }

    //////    public bool OnBeforeBrowse(IWebBrowser browser, IRequest request,
    //////        NavigationType naigationvType, bool isRedirect)
    //////    {
    //////        return false;
    //////    }

    //////    public bool OnBeforeResourceLoad(IWebBrowser browser, IRequestResponse requestResponse)
    //////    {
    //////        string requestURL = requestResponse.Request.Url;
    //////        string host = requestURL.Split('/')[2];
    //////        if (_app.appInfo.disableHosts.Length > 0 && _app.appInfo.disableHosts.Contains(host)) return false;
    //////        if (_app.appInfo.hasWriteLog) _app.writeLog(requestURL);
    //////        return false;
    //////    }

    //////    public void OnResourceResponse(IWebBrowser browser, string url, int status, string statusText,
    //////        string mimeType, WebHeaderCollection headers)
    //////    { }
    //////}

    //////public class ExternalLifeSpanHandler : ILifeSpanHandler
    //////{
    //////    public void OnBeforeClose(IWebBrowser browser) { }
    //////    public bool OnBeforePopup(IWebBrowser browser, string url, ref int x, ref int y, ref int width, ref int height)
    //////    {
    //////        if (!string.IsNullOrEmpty(url) && url.Contains("chrome-devtools://devtools/devtools.html")) return false;

    //////        try
    //////        {
    //////            //System.Diagnostics.Process.Start(url);
    //////        }
    //////        catch { }
    //////        return true;
    //////    }
    //////}
}