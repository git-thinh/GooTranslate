﻿using System;
using System.Collections.Generic;
using System.Text;

namespace browser
{
    public interface IApp
    {
        oApp appInfo { get; }
        void writeLog(string text);

        void webViewMain_Load(string url);
        void webViewMain_Reload();
        void webViewMain_ShowDevTools();
        void webViewMain_Stop();
    }

    public class oAppCoreJs
    {
        public string Scheme { set; get; }
        public string Host { set; get; }
        public string fileName { set; get; }
        public string[] appendFiles { set; get; }
    }

    public class oApp
    {
        public bool alwayOnTop { set; get; }
        public bool hasWriteLog { set; get; }
        public string Url { set; get; }

        public int Width { set; get; }
        public int Height { set; get; }
        public int Top { set; get; }
        public int Left { set; get; }

        public oAppCoreJs coreJs { set; get; }
        public string[] disableHosts { set; get; }

        public oApp()
        {
            this.Width = 600;
            this.Height = 480;
            this.alwayOnTop = false;
            this.hasWriteLog = true;
            //this.disableHosts = new string[] { "devtools", "play.google.com" };
            this.disableHosts = new string[] { };
            //===========================================================================================================
            this.coreJs = new oAppCoreJs()
            {
                Scheme = "https",
                Host = "www.google-analytics.com",
                fileName = "analytics.js",
                appendFiles = new string[] { "core.translate.js", "core.css", "jquery.min.js", "w2ui.min.js", "vue.min.js", "components.js", "components.css", "w2ui.min.css", "https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.css" }
            };
            this.Url = "https://translate.google.com/#view=home&op=translate&sl=en&tl=vi";
            //-----------------------------------------------------------------------------------------------------------
            //this.coreJs = new oAppCoreJs()
            //{
            //    Scheme = "https",
            //    Host = "www.google-analytics.com",
            //    fileName = "analytics.js"
            //};
            //this.Url = "https://www.google.com/cse?q=javascript+%22commonly+used%22&cof=FORID:11&ie=UTF-8&sa=Search&cx=partner-pub-2694630391511205:bm47g3z09jd&ad=n9&num=10&rurl=https%3A%2F%2Fwww.thefreedictionary.com%2F_%2Fsearch%2Fgoogle.aspx%3Fq%3Djavascript%2B%2522commonly%2Bused%2522%26cof%3DFORID%3A11%26ie%3DUTF-8%26sa%3DSearch%26cx%3Dpartner-pub-2694630391511205%3Abm47g3z09jd&siteurl=https%3A%2F%2Fwww.thefreedictionary.com%2F_%2Fsearch%2Fgoogle.aspx%3Fq%3Dcommon%26cof%3DFORID%3A11%26ie%3DUTF-8%26sa%3DSearch%26cx%3Dpartner-pub-2694630391511205%3Abm47g3z09jd";
            //-----------------------------------------------------------------------------------------------------------
            //this.Url = "https://translate.google.com/#vi/en/";
            //this.Url = "https://www.thefreedictionary.com/_/search/google.aspx?q=javascript+%22commonly+used%22&cof=FORID:11&ie=UTF-8&sa=Search&cx=partner-pub-2694630391511205:bm47g3z09jd";
            //this.Url = "https://www.google.com/cse?q=javascript+%22commonly+used%22&cof=FORID:11&ie=UTF-8&sa=Search&cx=partner-pub-2694630391511205:bm47g3z09jd&ad=n9&num=10&rurl=https%3A%2F%2Fwww.thefreedictionary.com%2F_%2Fsearch%2Fgoogle.aspx%3Fq%3Djavascript%2B%2522commonly%2Bused%2522%26cof%3DFORID%3A11%26ie%3DUTF-8%26sa%3DSearch%26cx%3Dpartner-pub-2694630391511205%3Abm47g3z09jd&siteurl=https%3A%2F%2Fwww.thefreedictionary.com%2F_%2Fsearch%2Fgoogle.aspx%3Fq%3Dcommon%26cof%3DFORID%3A11%26ie%3DUTF-8%26sa%3DSearch%26cx%3Dpartner-pub-2694630391511205%3Abm47g3z09jd";
            //===========================================================================================================
        }
    }


}
