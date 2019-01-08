using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace browser
{
    public class ApiJavascript
    {
        readonly IApp _app;
        public ApiJavascript(IApp app) { this._app = app; }

        /*/////////////////////////////////////////////////////////////*/
        /*/////////////////////////////////////////////////////////////*/

        public void playMp3FromUrl(String url, Int32 repeat)
        {
            //_app.playMp3FromUrl("https://s3.amazonaws.com/audio.oxforddictionaries.com/en/mp3/hello_gb_1.mp3");
            //_app.playMp3FromUrl(url, repeat);
            var playThread = new Thread(timeout => _app.playMp3FromUrl("http://audio.oxforddictionaries.com/en/mp3/ranker_gb_1_8.mp3", (int)timeout));
            playThread.IsBackground = true;
            playThread.Start(10000);
        }

        public void speechSentence(String text, Int32 repeat)
        {
            _app.speechSentence(text, repeat);
        }

        public void speechWords(String text, Int32 repeat)
        {
            _app.speechWords(text, repeat);
        }

        public void speechWord(String text, Int32 repeat)
        {
            _app.speechWord(text, repeat);
        }

        public void speechCancel()
        {
            _app.speechCancel();
        }

        /*/////////////////////////////////////////////////////////////*/
        /*/////////////////////////////////////////////////////////////*/

        public String fetchResponse(String url)
        {
            return _app.fetchResponse(url);
        }

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
