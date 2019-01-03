using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace browser
{
    public class WebViewSearch : UserControl, IRequestHandler
    {
        readonly IApp _app;
        readonly WebView _webView;
        string _urlApi;

        public WebViewSearch(IApp app)
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
                {
                    string url = "https://www.google.com/cse?q=javascript+%22commonly+used%22&cof=FORID:11&ie=UTF-8&sa=Search&cx=partner-pub-2694630391511205:bm47g3z09jd&ad=n9&num=10&rurl=https%3A%2F%2Fwww.thefreedictionary.com%2F_%2Fsearch%2Fgoogle.aspx%3Fq%3Djavascript%2B%2522commonly%2Bused%2522%26cof%3DFORID%3A11%26ie%3DUTF-8%26sa%3DSearch%26cx%3Dpartner-pub-2694630391511205%3Abm47g3z09jd&siteurl=https%3A%2F%2Fwww.thefreedictionary.com%2F_%2Fsearch%2Fgoogle.aspx%3Fq%3Dcommon%26cof%3DFORID%3A11%26ie%3DUTF-8%26sa%3DSearch%26cx%3Dpartner-pub-2694630391511205%3Abm47g3z09jd";
                    _webView.Load(url);
                }
            };
            _webView.MenuHandler = new MenuHandler();
            _webView.RequestHandler = this;

            //////////_webView.LifeSpanHandler
            //////////_webView.LoadHandler
            //////////_webView.JsDialogHandler = new WebViewDialogHandler(this);
            ////////_webView.LifeSpanHandler = new ExternalLifeSpanHandler();

            this.Controls.Add(_webView);
        }

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
    }
}
