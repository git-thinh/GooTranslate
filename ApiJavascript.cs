using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace browser
{
    public class ApiJavascript
    {
        readonly IApp _app;
        public ApiJavascript(IApp app) { this._app = app; }

        /*/////////////////////////////////////////////////////////////*/
        /*/////////////////////////////////////////////////////////////*/

        public void speechSentence(string text, int repeat)
        {
            _app.speechSentence(text, repeat);
        }

        public void speechWords(string text, int repeat)
        {
            _app.speechWords(text, repeat);
        }

        public void speechWord(string text, int repeat)
        {
            _app.speechWord(text, repeat);
        }

        public void speechCancel()
        {
            _app.speechCancel();
        }

        /*/////////////////////////////////////////////////////////////*/
        /*/////////////////////////////////////////////////////////////*/

        public void fetchHttp(String url)
        {
            _app.fetchHttp(url);
        }

        public void fetchHttps(String url)
        {
            _app.fetchHttps(url);
        }

        /*/////////////////////////////////////////////////////////////*/
        /*/////////////////////////////////////////////////////////////*/

        public void f_api_devtool(String menu)
        {
            _app.webViewMain_ShowDevTools();
        }

        public void f_main_openUrl(String url, String title)
        {
            _app.webViewMain_Load(url);
        }

        public void f_api_reload(String menu)
        {
            _app.webViewMain_Reload();
        }

        /*/////////////////////////////////////////////////////////////*/
        /*/////////////////////////////////////////////////////////////*/

        public string readFile(String file)
        {
            if (File.Exists(file)) return File.ReadAllText(file);
            return JsonConvert.SerializeObject(new { Ok = false, Message = "Cannot find the file: " + file });
        }

        public Boolean writeFile(String file, String data)
        {
            if (string.IsNullOrEmpty(file)) return false;
            string fi = file;
            if (fi[0] == '/' || fi[0] == '\\') fi = fi.Substring(1).Replace('\\', '/').Trim();

            try
            {
                fi = Path.GetFullPath(fi);
                string dir = Path.GetDirectoryName(fi);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                File.WriteAllText(fi, data);
                return true;
            }
            catch { }
            return false;
        }

        public Boolean existFile(String file)
        {
            if (string.IsNullOrEmpty(file)) return false;
            string fi = file;
            if (fi[0] == '/' || fi[0] == '\\') fi = fi.Substring(1).Replace('\\', '/').Trim();

            try
            {
                fi = Path.GetFullPath(fi);
                return File.Exists(fi);
            }
            catch { }
            return false;
        }
    }
}
