using Android.Content;
using Android.Content.Res;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Util;
using Android.Webkit;
using BitChute.Classes;
using BitChute.Web.Ui;
using HtmlAgilityPack;
using Java.IO;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using static Android.InputMethodServices.InputMethodService;

namespace BitChute.Web
{
    /// <summary>
    /// This class contains web related methods 
    /// that should run when the app first starts
    /// and objects that are related to an app startup
    /// event.
    /// 
    /// examples of usage include checking json objects
    /// and html documents on BitChute.com to retrieve 
    /// dynamic data that will be used in-app upon startup.
    /// </summary>
    public class Startup 
    {
        public Startup()
        {

        }

        public static async Task<bool> GetObjectsFromHtmlResponse()
        {
            string h = await ExtWebInterface.GetHtmlTextFromUrl("https://www.bitchute.com/");

            try
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(h);

                if (doc != null)
                {
                    foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//link[@href]"))
                    {
                        if (node.OuterHtml.Contains("/common.css"))
                        {
                            CssHelper.CommonCssUrl = await Task.FromResult("https://www.bitchute.com" + node.Attributes["href"].Value);
                        }
                        if (node.OuterHtml.Contains("/search.css"))
                        {
                            CssHelper.SearchCssUrl = await Task.FromResult("https://www.bitchute.com" + node.Attributes["href"].Value);
                        }
                    }
                }
                CssHelper.GetCommonCss(CssHelper.CommonCssUrl, true, true);
            }
            catch (Exception ex) { System.Console.WriteLine(ex.Message); }
            return false;
        }



        public class SplashWebView : WebView
        {
            string HTMLText = "<html>" + "<body>" + "<img src='bitchute_splash.gif'/></body>" + "</html>";
            //Load HTML Data in WebView


            public SplashWebView(Context context) : base(context)
            {
            }

            public SplashWebView(Context context, IAttributeSet attrs) : base(context, attrs)
            {
                var sps = MainActivity.Main.GetSplash();
               // var page = Path.Combine(Java.IO.File.Separator, "splash.html");
                
                //this.LoadUrl("splash.html");
                this.LoadData(sps, "text/html", "utf-8");
            }

            public SplashWebView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
            {
            }

            public SplashWebView(Context context, IAttributeSet attrs, int defStyleAttr, bool privateBrowsing) : base(context, attrs, defStyleAttr, privateBrowsing)
            {
            }

            public SplashWebView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
            {
            }

            protected SplashWebView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
            {
            }
        }
    }
}