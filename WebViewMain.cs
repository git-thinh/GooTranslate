using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace browser
{
    public class WebViewMain : UserControl, IRequestHandler
    {
        readonly IApp _app;
        readonly WebView _webView;
        string _urlApi;

        public WebViewMain(IApp app)
        {
            this.Location = new System.Drawing.Point(-1000, -1000);
            _app = app;

            _webView = new WebView("about:blank", new BrowserSettings()
            {
                PageCacheDisabled = true,
                WebSecurityDisabled = true,
                ApplicationCacheDisabled = true
            })
            {
                Dock = DockStyle.Fill
            };
            _webView.PropertyChanged += (sei, evi) =>
            {
                if (evi.PropertyName == "IsBrowserInitialized")
                    _webView.Load(this._app.appInfo.Url);
            };
            _webView.MenuHandler = new MenuHandler();
            _webView.RequestHandler = this;

            //////////_webView.LifeSpanHandler
            //////////_webView.LoadHandler
            //////////_webView.JsDialogHandler = new WebViewDialogHandler(this);
            ////////_webView.LifeSpanHandler = new ExternalLifeSpanHandler();

            this.Controls.Add(_webView);
        }

        public void onReady(Form main)
        {
            this.Location = new System.Drawing.Point(0, 0);
            this.Width = main.Width;
            this.Height = main.Height;
            this.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
        }

        /*///////////////////////////////////////////////////////////////////////////////////////////////////*/
        /*///////////////////////////////////////////////////////////////////////////////////////////////////*/
        public bool OnBeforeResourceLoad(IWebBrowser browser, IRequestResponse requestResponse)
        {
            string requestURL = requestResponse.Request.Url;
            if (requestURL.StartsWith("https://cse.google.com/cse/element/v1"))
            {
                _urlApi = requestURL;
                browser.Stop();
                return true;
            }
            //string host = requestURL.Split('/')[2];
            //if (_app.appInfo.disableHosts.Length > 0 && _app.appInfo.disableHosts.Contains(host)) return false;
            //if (_app.appInfo.hasWriteLog) _app.writeLog(requestURL);
            return false;
        }
        public void OnResourceResponse(IWebBrowser browser, string url, int status, string statusText, string mimeType, WebHeaderCollection headers) { }
        public bool GetAuthCredentials(IWebBrowser browser, bool isProxy, string host, int port, string realm, string scheme, ref string username, ref string password) { return false; }
        public bool GetDownloadHandler(IWebBrowser browser, string mimeType, string fileName, long contentLength, ref IDownloadHandler handler) { return false; }
        public bool OnBeforeBrowse(IWebBrowser browser, IRequest request, NavigationType naigationvType, bool isRedirect) { return false; }
        /*///////////////////////////////////////////////////////////////////////////////////////////////////*/
        /*///////////////////////////////////////////////////////////////////////////////////////////////////*/
        public void Load(string url) { if (_webView != null) _webView.Load(url); }
        public void Reload() { if (_webView != null) _webView.Reload(); }
        public void ShowDevTools() { if (_webView != null) _webView.ShowDevTools(); }
        public void Stop() { if (_webView != null) _webView.Stop(); }
    }
}
