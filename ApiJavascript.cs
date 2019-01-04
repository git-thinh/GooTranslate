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

        public string f_api_readFile(String file)
        {
            if (File.Exists(file)) return File.ReadAllText(file);
            return JsonConvert.SerializeObject(new { Ok = false, Message = "Cannot find the file: " + file });
        }

        public Boolean f_api_writeFile(String file, String data)
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

        public Boolean f_api_existFile(String file)
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
