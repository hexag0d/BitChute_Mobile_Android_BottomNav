using System.Threading.Tasks;
using Android.Webkit;
using static BitChute.Services.MainPlaybackSticky;

namespace BitChute.App
{
    public class CustomComments
    {
        public static string DomainRoot = "https://bitchute.com/profile/";
        public static string BaseCommentAvatarClickAdder = @"for(i=0;i<document.getElementsByClassName('profile-picture').length;i++){" +
                @"$('.profile-picture')[i].outerHTML=(" + "\"" + @"<a href='"+ DomainRoot + "\"" + @"+" 
        + @"$('.profile-picture')[i].getAttribute('data-user-id') +" + "\"" + @"'>" + "\"" + @"+" + @" $('.profile-picture')[i].outerHTML" 
                + @"+" + "\"" + @"</a>" + "\"" + @")}";

        public static async void MakeAvatarsClickable(ServiceWebView serviceWebView = null, WebView webView = null, bool useInternalListener = false, int delay = 2000)
        {
            await Task.Delay(delay);
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
                });
                return;
            }
            return;
        }
    }
}