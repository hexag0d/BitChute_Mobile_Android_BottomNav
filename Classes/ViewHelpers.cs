using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using BitChute.Fragments;

namespace BitChute.Classes
{
    /// <summary>
    /// static class that contains views to make UI 
    /// easier to access universally across the app
    /// </summary>
    public class ViewHelpers
    {
        public static ViewGroup Container { get; set; }

        public class Main
        {
            public static FloatingActionButton DownloadFAB { get; set; }
            public static RelativeLayout.LayoutParams FabLayoutBottom { get; set; }
            public static RelativeLayout.LayoutParams FabLayoutOrig { get; set; }
            //public static int LayoutAbove { get; set; }
            public static Handler UiHandler = new Handler();

            private static bool _navHidden;
            public static bool NavHidden
            {
                get
                {
                    return _navHidden;
                }
                set
                {
                    _navHidden = value;
                    OnNavBarVizChanged();
                }
            }

            public static async void OnNavBarVizChanged()
            {
                await System.Threading.Tasks.Task.Run(() =>
                {
                    if (FabLayoutOrig == null)
                    {
                        FabLayoutOrig = (RelativeLayout.LayoutParams)DownloadFAB.LayoutParameters;
                        FabLayoutBottom = FabLayoutOrig;
                        FabLayoutBottom.AddRule(LayoutRules.AlignParentBottom);
                    }
                });
                if (NavHidden)
                {
                    FabLayoutBottom.AddRule(LayoutRules.AlignParentBottom);
                    UiHandler.Post(() => DownloadFAB.LayoutParameters = FabLayoutBottom);
                }
                else
                {
                    FabLayoutBottom.RemoveRule(LayoutRules.AlignParentBottom);
                    UiHandler.Post(() => DownloadFAB.LayoutParameters = FabLayoutBottom);
                }
            }
        }

        /// <summary>
        /// Restores the missing disqus iFrame
        /// Disqus disappears due to scripting conflicts between
        /// the lazy loaded disqus iFrames and BitChute.com 
        /// video detail page script.  This will restore the missing iFrame
        /// </summary>
        /// <param name="tab"></param>
        public static void RestoreDisqusIFrame(int tab)
        {
            switch (tab)
            {
                case 0: Tab0Frag.Wv.LoadUrl(JavascriptCommands.RestoreDisqusIFrame); break;
                case 1: Tab1Frag.Wv.LoadUrl(JavascriptCommands.RestoreDisqusIFrame); break;
                case 2: Tab2Frag.Wv.LoadUrl(JavascriptCommands.RestoreDisqusIFrame); break;
                case 3: Tab3Frag.Wv.LoadUrl(JavascriptCommands.RestoreDisqusIFrame); break;
                case 4: Tab4Frag.Wv.LoadUrl(JavascriptCommands.RestoreDisqusIFrame); break;
            }
        }
        
        /// <summary>
        /// Provides an interface to inject javascript commands into any tab
        /// Designed for advanced users or debugging only, should be disabled on releases
        /// </summary>
        /// <param name="tab">the tab you want to inject into</param>
        /// <param name="jsCode">the jsCode to inject, null = get from settings EditText</param>
        public static void InjectJavascriptIntoWebView(int tab, string jsCode)
        {
            if (jsCode == null) { jsCode = Tab4.JavascriptInjectionTextBox.Text; }
            string jsf = @"javascript:(function() { ";
            string jsc = @"})()";
            string js2i = jsf + jsCode + jsc;
            switch (tab)
            {
                case 0: Tab0Frag.Wv.LoadUrl(js2i); break;
                case 1: Tab1Frag.Wv.LoadUrl(js2i); break;
                case 2: Tab2Frag.Wv.LoadUrl(js2i); break;
                case 3: Tab3Frag.Wv.LoadUrl(js2i); break;
                case 4: Tab4Frag.Wv.LoadUrl(js2i); break;
            }
        }

        public class Tab0
        {
            public static View FragmentContainerLayout { get; set; }
            public static View WebViewFragmentLayout { get; set; }
            public static View DownloaderLayout { get; set; }
            public static Button DownloadButton { get; set; }
            public static RelativeLayout TabFragmentLinearLayout { get; set; }
        }

        public class Tab3
        {
            public static View FragmentContainerLayout { get; set; }
            public static View WebViewFragmentLayout { get; set; }
            public static View DownloaderLayout { get; set; }
            public static View WebViewSwapable { get; set; }
            public static Button DownloadButton { get; set; }
            public static Button GetDownloadFilesButton { get; set; }
            public static Button CancelDownloadButton { get; set; }
            public static EditText DownloadLinkEditText { get; set; }
            public static EditText DownloadFileNameEditText { get; set; }
            public static ProgressBar DownloadProgressBar { get; set; }
            public static TextView DownloadProgressTextView { get; set; }
            public static LinearLayout TabFragmentLinearLayout { get; set; }
            public static CheckBox AutoFillVideoTitleText { get; set; }
            public static Android.Support.V7.Widget.RecyclerView.LayoutManager FileLayoutManager { get; set; }
            public static Android.Support.V7.Widget.RecyclerView FileRecyclerView { get; set; }
        }

        public class Tab4
        {
            public static EditText JavascriptInjectionTextBox { get; set; }
        }
    }
}