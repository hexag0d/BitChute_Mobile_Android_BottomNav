using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using static BitChute.Services.MainPlaybackSticky;

namespace BitChute
{
    public class CustomComments
    {
        public static string DomainRoot = "https://bitchute.com/profile/";
        public static string BaseCommentAvatarClickAdder = @"var _rootUrl = '" + DomainRoot + @"';" +
            @"for (i=0;i<document.getElementsByClassName('profile-picture').length;i++){" +
                @"$('.profile-picture')[i].outerHTML = (" + "\"" + @"<a href='"+ DomainRoot + "\"" + @"+" 
        + @"$('.profile-picture')[i].getAttribute('data-user-id') +" + "\"" + @"'>" + "\"" + @"+" + @" $('.profile-picture')[i].outerHTML" 
                + @"+" + "\"" + @"</a>" + "\"" + @")}";

        public static string AddLoadEndDocumentListener(string javascript)
        {
            string loadEnd = "";
            return loadEnd;
        }


        public static async void MakeAvatarsClickable(ServiceWebView serviceWebView = null, WebView webView = null, bool useInternalListener = false, int delay = 2000)
        {
            
            if (serviceWebView != null)
            {
                ViewHelpers.DoActionOnUiThread(() =>
                {
                    serviceWebView.LoadUrl(JavascriptCommands.GetInjectable(BaseCommentAvatarClickAdder));
                });
                return;
            }
            else if (webView != null)
            {
                ViewHelpers.DoActionOnUiThread(() =>
                {
                    webView.LoadUrl(JavascriptCommands.GetInjectable(BaseCommentAvatarClickAdder));

                    //ViewHelpers.Tab4.JavascriptInjectionTextBox.Text = BaseCommentAvatarClickAdder;
                });
                return;
            }
            return;
        }
    }
}