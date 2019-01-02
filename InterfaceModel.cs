using System;
using System.Collections.Generic;
using System.Text;

namespace browser
{
    public interface IApp
    {
        oApp appInfo { get; }
        void writeLog(string text);
    }

    public class oRegisterSchemes
    {
        public string Scheme { set; get; }
        public string Host { set; get; }
    }

    public class oApp
    {
        public bool hasWriteLog { set; get; }
        public string Url { set; get; }

        public int Width { set; get; }
        public int Height { set; get; }
        public int Top { set; get; }
        public int Left { set; get; }

        public oRegisterSchemes[] registerSchemes { set; get; }

        public oApp()
        {
            this.registerSchemes = new oRegisterSchemes[] {
                new oRegisterSchemes() { Scheme = "https", Host = "www.google-analytics.com" },
            };
            this.hasWriteLog = true;

            //this.Url = "https://translate.google.com/#vi/en/";
            this.Url = "https://translate.google.com/#view=home&op=translate&sl=en&tl=vi";

            this.Width = 600;
            this.Height = 480;
        }
    }


}
