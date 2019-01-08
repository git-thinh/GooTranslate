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
using System.Collections.Concurrent;
using System.Linq;
using NAudio.Wave;
using core2.MMF;
using System.Collections.Generic;

namespace browser
{
    class App : IApp
    {
        #region [ APP ]

        private static fMain _formMain;
        private static IApp _app;

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

            if (_objApp.storePaths != null && _objApp.storePaths.Length > 0)
            {
                for (int i = 0; i < _objApp.storePaths.Length; i++)
                {
                    if (File.Exists(_objApp.storePaths[i]))
                    {
                        _objApp.storePathCurrent = _objApp.storePaths[i];
                        break;
                    }
                }
            }
        }

        #endregion

        #region [ WEB_SOCKET ]

        private static int _socketPort = 0;
        public int socketPort { get { return _socketPort; } }

        private static bool _socketOpen = false;
        private static IWebSocketConnection _socketCurrent = null;
        public IWebSocketConnection socketCurrent { get { return _socketCurrent; } }

        public void socketSendMessage(string message)
        {
            if (_socketOpen) this.socketCurrent.Send(message);
        }

        #endregion

        #region [ SPEECH ]

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

        #endregion

        #region [ MP3 ]

        public void downloadMp3(string url, int timeOutDownloadMp3 = 30000)
        {
        }

        bool MP3_PLAYING = false;
        AutoResetEvent MP3_TERMINAL = new AutoResetEvent(false);
        public void playMp3FromUrl(string url, int repeat, bool isRunOnline = false, int timeOutDownloadMp3 = 30000)
        {
            if (MP3_PLAYING) return;
            MP3_PLAYING = true;

            //terminal
            //if (MP3_TERMINAL) MP3_TERMINAL.Set();

            if (isRunOnline)
            {
                #region [1] Play the online url mp3

                using (Stream ms = new MemoryStream())
                {
                    try
                    {
                        using (Stream stream = WebRequest.Create(url).GetResponse().GetResponseStream())
                        {
                            byte[] buffer = new byte[32768];
                            int read;
                            while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                ms.Write(buffer, 0, read);
                            }
                        }
                    }
                    catch
                    {
                        socketPushMessage("#FAIL:" + url);
                        // Cannot download the file mp3
                        return;
                    }

                    ms.Position = 0;
                    using (WaveStream baStream = new BlockAlignReductionStream(WaveFormatConversionStream.CreatePcmStream(new Mp3FileReader(ms))))
                    {
                        using (WaveOut waveOut = new WaveOut(WaveCallbackInfo.FunctionCallback()))
                        {
                            //waveOut.Init(baStream);
                            //waveOut.Play();
                            //while (waveOut.PlaybackState == PlaybackState.Playing)
                            //{
                            //    System.Threading.Thread.Sleep(100);
                            //}

                            waveOut.Init(baStream);
                            waveOut.PlaybackStopped += (sender, e) =>
                            {
                                waveOut.Stop();
                            };
                            waveOut.Play();
                        }
                    }
                }

                #endregion
            }
            else
            {
                #region [2] Download and play the file mp3

                //String url = "https:/api.twilioxxx.com/2010-04-01xxx/AccountSidxxxx/Residxxx.mp3";//Not real Path
                string filename = Path.GetFileName(url);
                if (!File.Exists(filename))
                {
                    byte[] data;
                    try
                    {
                        WebClient client = new WebClient();
                        data = client.DownloadData(new Uri(url));
                        int MAX_BYTES = data.Length;
                        MemoryMappedFile map = MemoryMappedFile.Create(filename, MapProtection.PageReadWrite, MAX_BYTES);
                        using (Stream view = map.MapView(MapAccess.FileMapWrite, 0, MAX_BYTES))
                            view.Write(data, 0, MAX_BYTES);
                        map.Close();
                    }
                    catch
                    {
                        socketPushMessage("#FAIL:" + url);
                        // Cannot download the file mp3
                        return;
                    }
                }

                using (var ms = File.OpenRead(filename))
                using (var rdr = new Mp3FileReader(ms))
                using (var wavStream = WaveFormatConversionStream.CreatePcmStream(rdr))
                using (var baStream = new BlockAlignReductionStream(wavStream))
                using (var waveOut = new WaveOut(WaveCallbackInfo.FunctionCallback()))
                {
                    //waveOut.Init(baStream);
                    //waveOut.Play();
                    //while (waveOut.PlaybackState == PlaybackState.Playing)
                    //{
                    //    Thread.Sleep(100);
                    //}

                    waveOut.Init(baStream);
                    waveOut.PlaybackStopped += (sender, e) =>
                    {
                        waveOut.Stop();
                    };
                    waveOut.Play();
                }

                #endregion
            }

            MP3_PLAYING = false;
            socketPushMessage("#DONE:" + url);
        }

        private Stream ms = new MemoryStream();
        public void playMp3FromUrl22(string url, int repeat)
        {
            new Thread(delegate (object o)
            {
                var response = WebRequest.Create(url).GetResponse();
                using (var stream = response.GetResponseStream())
                {
                    byte[] buffer = new byte[65536]; // 64KB chunks
                    int read;
                    while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        var pos = ms.Position;
                        ms.Position = ms.Length;
                        ms.Write(buffer, 0, read);
                        ms.Position = pos;
                    }
                }
            }).Start();

            // Pre-buffering some data to allow NAudio to start playing
            while (ms.Length < 65536 * 10)
                Thread.Sleep(1000);

            ms.Position = 0;
            using (WaveStream blockAlignedStream = new BlockAlignReductionStream(WaveFormatConversionStream.CreatePcmStream(new Mp3FileReader(ms))))
            {
                using (WaveOut waveOut = new WaveOut(WaveCallbackInfo.FunctionCallback()))
                {
                    waveOut.Init(blockAlignedStream);
                    waveOut.Play();
                    while (waveOut.PlaybackState == PlaybackState.Playing)
                    {
                        System.Threading.Thread.Sleep(100);
                    }
                }
            }
        }

        static ConcurrentDictionary<string, byte[]> _dicMp3 = new ConcurrentDictionary<string, byte[]>() { };
        bool playMp3FromUrl_waiting = false;
        AutoResetEvent playMp3FromUrl_stop = new AutoResetEvent(false);
        public void playMp3FromUrl333(string url, int timeout)
        {
            if (playMp3FromUrl_waiting) return;

            //terminal
            //if (playMp3FromUrl_waiting) playMp3FromUrl_stop.Set();

            Stream ms = null;
            if (_dicMp3.ContainsKey(url))
                ms = new MemoryStream(_dicMp3[url]);
            else
            {
                ms = new MemoryStream();
                using (Stream stream = WebRequest.Create(url).GetResponse().GetResponseStream())
                {
                    byte[] buffer = new byte[32768];
                    int read;
                    while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, read);
                    }
                    _dicMp3.TryAdd(url, buffer);
                }

                ms.Position = 0;
                using (WaveStream blockAlignedStream = new BlockAlignReductionStream(WaveFormatConversionStream.CreatePcmStream(new Mp3FileReader(ms))))
                {
                    using (WaveOut waveOut = new WaveOut(WaveCallbackInfo.FunctionCallback()))
                    {
                        waveOut.Init(blockAlignedStream);
                        waveOut.PlaybackStopped += (sender, e) =>
                        {
                            waveOut.Stop();
                        };
                        waveOut.Play();
                        playMp3FromUrl_waiting = true;
                        playMp3FromUrl_stop.WaitOne(timeout);
                        playMp3FromUrl_waiting = false;
                    }
                }
            }
        }

        #endregion

        #region [ WEB VIEW MAIN ]

        public void webViewMain_Load(string url) { if (_formMain != null) _formMain.webViewMain_Load(url); }
        public void webViewMain_Reload() { if (_formMain != null) _formMain.webViewMain_Reload(); }
        public void webViewMain_ShowDevTools() { if (_formMain != null) _formMain.webViewMain_ShowDevTools(); }
        public void webViewMain_Stop() { if (_formMain != null) _formMain.webViewMain_Stop(); }

        #endregion

        #region [ CRAWLER ]

        static ConcurrentDictionary<string, string> _dicHtml = new ConcurrentDictionary<string, string>() { };

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

        public string fetchResponse(string url)
        {
            if (_dicHtml.ContainsKey(url)) return _dicHtml[url];
            return string.Empty;
        }

        public void fetchHttp(string url)
        {
            if (_dicHtml.ContainsKey(url))
            {
                socketPushMessage("@" + url);
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
                            socketPushMessage("@" + url);
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
                socketPushMessage("@" + url);
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
                            socketPushMessage("@" + url);
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

        #endregion

        #region [ MSG_QUEUE ]

        static ConcurrentQueue<string> _msgQueue = new ConcurrentQueue<string> { };
        public void socketPushMessage(string message) { if (!string.IsNullOrEmpty(message)) _msgQueue.Enqueue(message); }

        static ConcurrentQueue<string> _mp3Queue = new ConcurrentQueue<string> { };

        #endregion

        #region [ DICTIONARY ]

        static ConcurrentDictionary<string, string> _dicWordPhrase = new ConcurrentDictionary<string, string>() { };
        static ConcurrentDictionary<int, string> _dicSentence = new ConcurrentDictionary<int, string>() { };
        static ConcurrentDictionary<string, List<int>> _dicWordSentence = new ConcurrentDictionary<string, List<int>>() { };

        public bool dicWordPhraseAdd(string name, string phonics, string mean)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(mean)) return false;
            name = name.ToLower().Trim();
            mean = mean.ToLower().Trim();

            if (_dicWordPhrase.ContainsKey(name) == false)
            {
                string file = Path.Combine(_objApp.storePathCurrent, "word.txt");
                string line = string.Format("{0};{1};{2}", name, phonics, mean);
                if (string.IsNullOrEmpty(phonics))
                {
                    phonics = string.Empty;
                    file = Path.Combine(_objApp.storePathCurrent, "phrase.txt");
                }
                else
                {
                    if (string.IsNullOrEmpty(phonics)) return false;

                    line = string.Format("{0};{1}", name, mean);
                    phonics = phonics.ToLower().Trim();
                    _mp3Queue.Enqueue(name);
                }

                _dicWordPhrase.TryAdd(name, line);
                using (StreamWriter sw = File.AppendText(file))
                    sw.WriteLine(line);
                return true;
            }
            return false;
        }

        public bool dicSentenceAdd(string text)
        {
            string file = Path.Combine(_objApp.storePathCurrent, "sentence.txt");
            bool notExist = _dicSentence.Where(kv => kv.Value == text).Count() == 0;
            if (notExist)
            {
                _dicSentence.TryAdd(_dicSentence.Count, text);
                using (StreamWriter sw = File.AppendText(file))
                    sw.WriteLine(text);
                return true;
            }
            return false;
        }

        #endregion

        [STAThread]
        static void Main(string[] args)
        {
            if (!CEF.Initialize(new Settings())) return;
            //----------------------------------------------------------------------
            ThreadPool.SetMaxThreads(25, 25);
            ServicePointManager.DefaultConnectionLimit = 1000;
            //----------------------------------------------------------------------
            _app = new App();
            //----------------------------------------------------------------------
            System.Timers.Timer aTimer = new System.Timers.Timer(300);
            aTimer.Elapsed += (se, ev) =>
            {
                if (_msgQueue.Count > 0)
                {
                    string msg;
                    if (_msgQueue.TryDequeue(out msg))
                        if (!string.IsNullOrEmpty(msg)) _app.socketSendMessage(msg);
                }
            };
            aTimer.AutoReset = true;
            aTimer.Enabled = true;

            //_app.playMp3FromUrl("https://s3.amazonaws.com/audio.oxforddictionaries.com/en/mp3/hello_gb_1.mp3", 1);
            //_app.playMp3FromUrl("http://audio.oxforddictionaries.com/en/mp3/ranker_gb_1_8.mp3", 1);
            //Thread.Sleep(15000);
            //_app.playMp3FromUrl("http://audio.oxforddictionaries.com/en/mp3/ranker_gb_1_8.mp3", 1);

            //var playThread = new Thread(timeout => _app.playMp3FromUrl("http://translate.google.com/translate_tts?q=" + HttpUtility.UrlEncode("commonly used"), (int)timeout));
            //playThread.IsBackground = true;
            //playThread.Start(10000);

            //var playThread = new Thread(timeout => _app.playMp3FromUrl("http://audio.oxforddictionaries.com/en/mp3/ranker_gb_1_8.mp3", (int)timeout));
            //playThread.IsBackground = true;
            //playThread.Start(10000);
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
                };
                socket.OnOpen = () =>
                {
                    _socketOpen = true;
                    _socketCurrent = socket;
                    //socket.ConnectionInfo.Id.ToString();
                };
                socket.OnClose = () =>
                {
                    _socketOpen = false;
                };
                socket.OnError = (err) =>
                {
                    _socketOpen = false;
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

        #region [ LOG ]

        private static StringBuilder _log = new StringBuilder(string.Empty);
        public void writeLog(string text)
        {
            _log.Append(text);
            _log.Append(Environment.NewLine);
        }

        #endregion
    }
}
