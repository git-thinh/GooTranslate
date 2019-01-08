using CefSharp;
using System;
using System.Windows.Forms;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using System.Threading;
using System.Net;
using SeasideResearch.LibCurlNet;
using Fleck2.Interfaces;
using System.Speech.Synthesis;
using System.Net.Sockets;
using Fleck2;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;

namespace browser
{
    class App : IApp
    {
        private static int _socketPort = 0;
        public int socketPort { get { return _socketPort; } }

        private static IWebSocketConnection _socketCurrent = null;
        public IWebSocketConnection socketCurrent { get { return _socketCurrent; } }

        private void socketSendMessage(string message)
        {
            this.socketCurrent.Send(message);
        }

        /*/////////////////////////////////////////////////////////////*/
        /*/////////////////////////////////////////////////////////////*/

        private static SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer();
        static string[] _speakWords = new string[] { };
        static Int16 _speakCounter = 0;
        static SynthesizerState _speakState = SynthesizerState.Ready;

        // Write to the console when the SpeakAsync operation has been cancelled.
        static void synth_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            //Console.WriteLine("\nThe SpeakAsync operation was cancelled!!");
            //////////_socketCurrent.Send(string.Format("!{0}", e.Error.Message));
            _speakState = SynthesizerState.Ready;
        }

        // When it changes, write the state of the SpeechSynthesizer to the console.
        static void synth_StateChanged(object sender, StateChangedEventArgs e)
        {
            _speakState = e.State;
            //Console.WriteLine("\nSynthesizer State: {0}    Previous State: {1}\n", e.State, e.PreviousState);
            //_socketCurrent.Send(string.Format("@{0}", e.State));
            if (e.State == SynthesizerState.Ready)
            {
                _speakCounter++;
                if (_speakCounter == _speakWords.Length - 1) _speakCounter = 0;
                speechSynthesizer.Speak(_speakWords[_speakCounter]);
            }
        }

        // Write the text being spoken by the SpeechSynthesizer to the console.
        static void synth_SpeakProgress(object sender, SpeakProgressEventArgs e)
        {
            //////_socketCurrent.Send(string.Format("#{0}", e.Text));
            //Console.WriteLine(e.Text);
        }

        public void speechSentence(string text, int repeat)
        {
            if (_speakState != SynthesizerState.Ready) return;

            try
            {
                _speakCounter = 0;
                _speakWords = text.ToLower().Split(' ')
                    .Where(x => !EL._WORD_SKIP_WHEN_READING.Any(w => w == x))
                    .ToArray();
                speechSynthesizer.Speak(_speakWords[_speakCounter]);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void speechWords(string text, int repeat)
        {
            if (_speakState != SynthesizerState.Ready) return;

            try
            {
                _speakCounter = 0;
                _speakWords = text.ToLower().Split(' ')
                    .Where(x => !EL._WORD_SKIP_WHEN_READING.Any(w => w == x))
                    .ToArray();
                speechSynthesizer.Speak(_speakWords[_speakCounter]);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void speechWord(string text, int repeat)
        {
            if (_speakState != SynthesizerState.Ready) return;

            try
            {
                _speakCounter = 0;
                _speakWords = new string[] { text };
                speechSynthesizer.Speak(_speakWords[_speakCounter]);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void speechCancel()
        {
            try
            {
                _speakCounter = 0;
                // Cancel the SpeakAsync operation and wait one second.
                speechSynthesizer.SpeakAsyncCancelAll();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /*/////////////////////////////////////////////////////////////*/
        /*/////////////////////////////////////////////////////////////*/

        private static fMain _formMain;
        private static IApp _app;

        /*/////////////////////////////////////////////////////////////*/
        /*/////////////////////////////////////////////////////////////*/

        private static oApp _objApp = null;
        public oApp appInfo { get { return _objApp; } }
        static App()
        {
            _objApp = new oApp();
            try
            {
                if (File.Exists("app.json")) _objApp = JsonConvert.DeserializeObject<oApp>(File.ReadAllText("app.json"));
            }
            catch { }
        }
        public void webViewMain_Load(string url) { if (_formMain != null) _formMain.webViewMain_Load(url); }
        public void webViewMain_Reload() { if (_formMain != null) _formMain.webViewMain_Reload(); }
        public void webViewMain_ShowDevTools() { if (_formMain != null) _formMain.webViewMain_ShowDevTools(); }
        public void webViewMain_Stop() { if (_formMain != null) _formMain.webViewMain_Stop(); }

        public void f_link_getHtmlOnline(string url)
        {
            ///////* https://stackoverflow.com/questions/4291912/process-start-how-to-get-the-output */
            //////Process process = new Process();
            //////process.StartInfo.FileName = "curl.exe";
            ////////process.StartInfo.Arguments = url;
            ////////process.StartInfo.Arguments = "--insecure " + url;
            ////////process.StartInfo.Arguments = "--max-time 5 -v " + url; /* -v url: handle error 302 found redirect localtion*/
            //////process.StartInfo.Arguments = "-m 5 -v " + url; /* -v url: handle error 302 found redirect localtion*/
            //////process.StartInfo.UseShellExecute = false;
            //////process.StartInfo.RedirectStandardOutput = true;
            //////process.StartInfo.RedirectStandardError = true;
            //////process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            //////process.Start();
            ////////* Read the output (or the error)
            //////string html = process.StandardOutput.ReadToEnd();
            //////if (string.IsNullOrEmpty(html))
            //////{
            //////    string err = process.StandardError.ReadToEnd(), urlDirect = string.Empty;

            //////    int pos = err.IndexOf("< Location: ");
            //////    if (pos != -1)
            //////    {
            //////        urlDirect = err.Substring(pos + 12, err.Length - (pos + 12)).Split(new char[] { '\r', '\n' })[0].Trim();
            //////        if (urlDirect[0] == '/')
            //////        {
            //////            Uri uri = new Uri(url);
            //////            urlDirect = uri.Scheme + "://" + uri.Host + urlDirect;
            //////        }

            //////        Console.WriteLine("-> Redirect: " + urlDirect);


            //////        html = f_link_getHtmlCache(urlDirect);
            //////        if (string.IsNullOrEmpty(html))
            //////        {
            //////            return "<script> location.href='" + urlDirect + "'; </script>";
            //////        }
            //////        else
            //////            return html;
            //////    }
            //////    else
            //////    {
            //////        Console.WriteLine(" ??????????????????????????????????????????? ERROR: " + url);
            //////    }

            //////    Console.WriteLine(" -> Fail: " + url);

            //////    return null;
            //////}

            //////Console.WriteLine(" -> Ok: " + url);

            //////string title = Html.f_html_getTitle(html);
            //////html = _htmlFormat(url, html);
            //////f_cacheUrl(url);
            //////CACHE.TryAdd(url, html);

            ////////string err = process.StandardError.ReadToEnd();
            //////process.WaitForExit();

            //////if (_fomMain != null) _fomMain.f_browser_updateInfoPage(url, title);

            //////return html;

            ////////////* Create your Process
            //////////Process process = new Process();
            //////////process.StartInfo.FileName = "curl.exe";
            //////////process.StartInfo.Arguments = url;
            //////////process.StartInfo.UseShellExecute = false;
            //////////process.StartInfo.RedirectStandardOutput = true;
            //////////process.StartInfo.RedirectStandardError = true;
            //////////process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            ////////////* Set your output and error (asynchronous) handlers
            //////////process.OutputDataReceived += (se, ev) => {
            //////////    string html = ev.Data;

            //////////    _link.TryAdd(url, _link.Count + 1);
            //////////    _html.TryAdd(url, html);
            //////////};
            ////////////process.ErrorDataReceived += new DataReceivedEventHandler(OutputHandler);
            ////////////* Start process and handlers
            //////////process.Start();
            //////////process.BeginOutputReadLine();
            //////////process.BeginErrorReadLine();
            //////////process.WaitForExit(); 
        }

        /*/////////////////////////////////////////////////////////////*/
        /*/////////////////////////////////////////////////////////////*/

        static ConcurrentDictionary<string, string> _dicHtml = new ConcurrentDictionary<string, string>() { };

        public string fetchResponse(string url)
        {
            if (_dicHtml.ContainsKey(url)) return _dicHtml[url];
            return string.Empty;
        }

        public void fetchHttp(string url)
        {
            if (_dicHtml.ContainsKey(url))
            {
                socketSendMessage("@" + url);
                return;
            }

            try
            {
                StringBuilder bi = new StringBuilder(string.Empty);
                Curl.GlobalInit((int)CURLinitFlag.CURL_GLOBAL_ALL);

                Easy easy = new Easy();
                Easy.WriteFunction wf = new Easy.WriteFunction((Byte[] buf, Int32 size, Int32 nmemb, Object extraData) =>
                {
                    string s = System.Text.Encoding.UTF8.GetString(buf).Trim();
                    if (s.Length > 0)
                    {
                        bi.Append(s);
                        if (s.Contains("</html>"))
                        {
                            string htm = bi.ToString();
                            _dicHtml.TryAdd(url, htm);
                            socketSendMessage("@" + url);
                        }
                    }
                    return size * nmemb;
                });

                easy.SetOpt(CURLoption.CURLOPT_URL, url);
                easy.SetOpt(CURLoption.CURLOPT_WRITEFUNCTION, wf);
                easy.Perform();
                //easy.Cleanup();
                easy.Dispose();
                Curl.GlobalCleanup();
            }
            catch
            {
                //Console.WriteLine(ex);
            }
        }

        public void fetchHttps(string url)
        {
            if (_dicHtml.ContainsKey(url))
            {
                socketSendMessage("@" + url);
                return;
            }

            try
            {
                StringBuilder bi = new StringBuilder(string.Empty);
                Curl.GlobalInit((int)CURLinitFlag.CURL_GLOBAL_ALL);
                Easy easy = new Easy();
                Easy.WriteFunction wf = new Easy.WriteFunction((Byte[] buf, Int32 size, Int32 nmemb, Object extraData) =>
                {
                    string s = System.Text.Encoding.UTF8.GetString(buf).Trim();
                    if (s.Length > 0)
                    {
                        bi.Append(s);
                        if (s.Contains("</html>"))
                        {
                            string htm = bi.ToString();
                            _dicHtml.TryAdd(url, htm);
                            socketSendMessage("@" + url);
                        }
                    }
                    return size * nmemb;
                });
                easy.SetOpt(CURLoption.CURLOPT_WRITEFUNCTION, wf);

                Easy.SSLContextFunction sf = new Easy.SSLContextFunction((SSLContext ctx, Object extraData) => { return CURLcode.CURLE_OK; });
                easy.SetOpt(CURLoption.CURLOPT_SSL_CTX_FUNCTION, sf);

                //easy.SetOpt(CURLoption.CURLOPT_URL, "https://dictionary.cambridge.org/grammar/british-grammar/above-or-over");
                easy.SetOpt(CURLoption.CURLOPT_URL, url);
                easy.SetOpt(CURLoption.CURLOPT_CAINFO, "ca-bundle.crt");

                easy.Perform();
                //easy.Cleanup();
                easy.Dispose();
                Curl.GlobalCleanup();
                Console.WriteLine("Enter to exit ...");
            }
            catch
            {
                //Console.WriteLine(ex);
            }
        }

        /*/////////////////////////////////////////////////////////////*/
        /*/////////////////////////////////////////////////////////////*/

        [STAThread]
        static void Main(string[] args)
        {
            if (!CEF.Initialize(new Settings())) return;
            //----------------------------------------------------------------------
            ThreadPool.SetMaxThreads(25, 25);
            ServicePointManager.DefaultConnectionLimit = 1000;
            //----------------------------------------------------------------------
            _app = new App();
            //======================================================================
            // WEB_SOCKET
            TcpListener l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            _socketPort = ((IPEndPoint)l.LocalEndpoint).Port;
            _app.appInfo.socketPort = _socketPort;
            l.Stop();

            //FleckLog.Level = LogLevel.Debug;
            FleckLog.Level = LogLevel.None;
            var server = new WebSocketServer("ws://localhost:" + _socketPort.ToString());
            server.Start(socket =>
            {
                socket.OnMessage = message =>
                {
                    Console.WriteLine(message);
                    if (message.Length > 0)
                    {
                        switch (message[0])
                        {
                            case '#':
                                try
                                {
                                    message = message.Substring(1).Trim();
                                    _speakWords = message.ToLower().Split(' ')
                                        .Where(x => !EL._WORD_SKIP_WHEN_READING.Any(w => w == x))
                                        .ToArray();
                                    speechSynthesizer.Speak(_speakWords[_speakCounter]);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                                break;
                            case '!':
                                // Cancel the SpeakAsync operation and wait one second.
                                speechSynthesizer.SpeakAsyncCancelAll();
                                break;
                            default:
                                //////long id = msgProcess.Push(socket.ConnectionInfo.Id.ToString(), message);
                                ////////socket.Send(id.ToString());
                                break;
                        }
                    }
                };
                socket.OnOpen = () =>
                {
                    _socketCurrent = socket;
                    //Thread thread = new Thread(new ParameterizedThreadStart(DoMethod));
                    //thread.Start(socket); 
                    //msgProcess.Join(socket); 
                    //Thread thread = new Thread(new ParameterizedThreadStart(DoMethod));
                    //thread.Start(socket);                    
                    //socket.Send("ID=" + socket.ConnectionInfo.Id.ToString());
                };
                socket.OnClose = () =>
                {
                    //Console.WriteLine("Close!");
                    //allSockets.Remove(socket);
                };
            });

            //======================================================================
            // SPEECH_MP3
            // Configure the audio output. 
            speechSynthesizer.SetOutputToDefaultAudioDevice();
            // Subscribe to the StateChanged event.
            speechSynthesizer.StateChanged += new EventHandler<StateChangedEventArgs>(synth_StateChanged);
            // Subscribe to the SpeakProgress event.
            speechSynthesizer.SpeakProgress += new EventHandler<SpeakProgressEventArgs>(synth_SpeakProgress);
            // Subscribe to the SpeakCompleted event.
            speechSynthesizer.SpeakCompleted += new EventHandler<SpeakCompletedEventArgs>(synth_SpeakCompleted);

            //======================================================================
            //test_curl_https();

            //======================================================================
            // CHROMIUM
            if (_app.appInfo.coreJs != null) CEF.RegisterScheme(_app.appInfo.coreJs.Scheme, _app.appInfo.coreJs.Host, true, new AppSchemeHandlerFactory(_app));
            CEF.RegisterJsObject("___API", new ApiJavascript(_app));

            _formMain = new fMain(_app);
            Application.Run(_formMain);
            CEF.Shutdown();

            //======================================================================
            // LOG - WRITE DATA
            string log = _log.ToString();
            if (_app.appInfo.hasWriteLog) File.WriteAllText("log.txt", log);

            string jsonApp = JsonConvert.SerializeObject(_app.appInfo, Formatting.Indented);
            File.WriteAllText("app.json", jsonApp);
            Thread.Sleep(1000);
        }

        /*/////////////////////////////////////////////////////////////*/
        /*/////////////////////////////////////////////////////////////*/

        private static StringBuilder _log = new StringBuilder(string.Empty);
        public void writeLog(string text)
        {
            _log.Append(text);
            _log.Append(Environment.NewLine);
        }
    }
}
