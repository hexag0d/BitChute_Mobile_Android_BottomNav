using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BitChute.Fragments;

namespace BitChute.Classes
{
    public class AdBlock
    {
        public static async void RemoveDiscusIFrame(int tab)
        {
            switch (tab)
            {
                case 0:
                    TheFragment0.Wv.LoadUrl(JavascriptCommands.RemoveDisqusIframeZero);
                    //TheFragment0.Wv.LoadUrl(JavascriptCommands.RemoveDisqusIframeTwo);
                    break;
                case 1:
                    TheFragment1.Wv.LoadUrl(JavascriptCommands.RemoveDisqusIframeZero);
                    //TheFragment1.Wv.LoadUrl(JavascriptCommands.RemoveDisqusIframeTwo);
                    break;
                case 2:
                    TheFragment2.Wv.LoadUrl(JavascriptCommands.RemoveDisqusIframeZero);
                    //TheFragment2.Wv.LoadUrl(JavascriptCommands.RemoveDisqusIframeTwo);
                    break;
                case 3:
                    TheFragment3.Wv.LoadUrl(JavascriptCommands.RemoveDisqusIframeZero);
                    //TheFragment3.Wv.LoadUrl(JavascriptCommands.RemoveDisqusIframeTwo);
                    break;
                case 4:
                    TheFragment4.Wv.LoadUrl(JavascriptCommands.RemoveDisqusIframeZero);
                    //TheFragment4.Wv.LoadUrl(JavascriptCommands.RemoveDisqusIframeTwo);
                    break;
            }
            await System.Threading.Tasks.Task.Delay(AppSettings.AdBlockDelay);
            switch (tab)
            {
                case 0:
                    TheFragment0.Wv.LoadUrl(JavascriptCommands.RemoveDisqusIframeZero);
                    //TheFragment0.Wv.LoadUrl(JavascriptCommands.RemoveDisqusIframeTwo);
                    break;
                case 1:
                    TheFragment1.Wv.LoadUrl(JavascriptCommands.RemoveDisqusIframeZero);
                    //TheFragment1.Wv.LoadUrl(JavascriptCommands.RemoveDisqusIframeTwo);
                    break;
                case 2:
                    TheFragment2.Wv.LoadUrl(JavascriptCommands.RemoveDisqusIframeZero);
                   //TheFragment2.Wv.LoadUrl(JavascriptCommands.RemoveDisqusIframeTwo);
                    break;
                case 3:
                    TheFragment3.Wv.LoadUrl(JavascriptCommands.RemoveDisqusIframeZero);
                    //TheFragment3.Wv.LoadUrl(JavascriptCommands.RemoveDisqusIframeTwo);
                    break;
                case 4:
                    TheFragment4.Wv.LoadUrl(JavascriptCommands.RemoveDisqusIframeZero);
                    //TheFragment4.Wv.LoadUrl(JavascriptCommands.RemoveDisqusIframeTwo);
                    break;
            }
            await System.Threading.Tasks.Task.Delay(AppSettings.AdBlockDelay);
            switch (tab)
            {
                case 0:
                    //TheFragment0.Wv.LoadUrl(JavascriptCommands.RemoveDisqusIframeZero);
                    TheFragment0.Wv.LoadUrl(JavascriptCommands.RemoveDisqusIframeTwo);
                    break;
                case 1:
                    //TheFragment1.Wv.LoadUrl(JavascriptCommands.RemoveDisqusIframeZero);
                    TheFragment1.Wv.LoadUrl(JavascriptCommands.RemoveDisqusIframeTwo);
                    break;
                case 2:
                    //TheFragment2.Wv.LoadUrl(JavascriptCommands.RemoveDisqusIframeZero);
                    TheFragment2.Wv.LoadUrl(JavascriptCommands.RemoveDisqusIframeTwo);
                    break;
                case 3:
                    //TheFragment3.Wv.LoadUrl(JavascriptCommands.RemoveDisqusIframeZero);
                    TheFragment3.Wv.LoadUrl(JavascriptCommands.RemoveDisqusIframeTwo);
                    break;
                case 4:
                    //TheFragment4.Wv.LoadUrl(JavascriptCommands.RemoveDisqusIframeZero);
                    TheFragment4.Wv.LoadUrl(JavascriptCommands.RemoveDisqusIframeTwo);
                    break;
            }
        }
    }
}