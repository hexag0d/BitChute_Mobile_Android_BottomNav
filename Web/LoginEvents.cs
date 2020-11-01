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
using static BitChute.Web.ViewClients;

namespace BitChute.Web.Auth
{
    public class LoginEventArgs : EventArgs
    {
        private string _cfduid;
        private string _csrftoken;
        private string _sessionid;
        private bool _loginDetected;
        private bool _loginRequested;
        private bool _loginSuccess;
        private bool _loginAttemptFailure;
        
        public LoginEventArgs(bool loginDetected = false, bool loginRequested = false, bool loginSuccess = false,
            bool loginAttemptFailure = false, string cfduid = null, string csrftoken = null, string sessionId = null)
        {
            _loginDetected = loginDetected;
            _loginRequested = loginRequested;
            _loginSuccess = loginSuccess;
            if (loginSuccess) {
                if (cfduid != null) { _cfduid = cfduid; }
                if (csrftoken != null) { _csrftoken = csrftoken; }
                if (sessionId != null) { _sessionid = sessionId; }
                if (cfduid != null && csrftoken != null && sessionId != null)
                {
                    RunPostAuthEvent.OnPostLoginSuccess(cfduid, csrftoken, sessionId, true);
                }
                else
                {
                    RunPostAuthEvent.OnPostLoginSuccess(null, null, null, false);
                }
            }
            if (loginAttemptFailure)
            {
                RunPostAuthEvent.OnPostLoginFailure();
            }
        }
        public string Cfduid { get { return _cfduid; } }
        public string CsrfToken { get { return _csrftoken; } }
        public string SessionId { get { return _sessionid; } }
        public bool LoginDetected { get { return _loginDetected; } }
        public bool LoginRequested { get { return _loginRequested; } }
        public bool LoginSuccess { get { return _loginSuccess; } }
        public bool LoginAttemptFailure { get { return _loginAttemptFailure; } }
    }

    public class RunPostAuthEvent
    {
        public static void OnPostLoginSuccess(string cfduid, string csrfToken, string sessionId, bool setSessionStateFromArgs = false)
        {
            CookieManager.Instance.SetAcceptCookie(true);
            CookieManager.Instance.RemoveAllCookie();
            if (setSessionStateFromArgs)
            {
                AppSettings.SessionState.Cfduid = cfduid;
                AppSettings.SessionState.CsrfToken = csrfToken;
                AppSettings.SessionState.SessionId = sessionId;
            }
            string cookieHeaderPt1 = "";
            if (!String.IsNullOrWhiteSpace(AppSettings.SessionState.CsrfToken))
            {
                cookieHeaderPt1 = $"{AppSettings.SessionState.CsrfToken};";

                CookieManager.Instance.SetCookie("https://www.bitchute.com/", cookieHeaderPt1);
            }

            if (!String.IsNullOrWhiteSpace(AppSettings.SessionState.SessionId))
            {
                cookieHeaderPt1 = $"{AppSettings.SessionState.SessionId};";

                CookieManager.Instance.SetCookie("https://www.bitchute.com/", cookieHeaderPt1);
            }

            cookieHeaderPt1 = "preferences={%22theme%22:%22night%22%2C%22autoplay%22:true}; ";

            CookieManager.Instance.SetCookie("https://www.bitchute.com/", cookieHeaderPt1);


            if (!String.IsNullOrWhiteSpace(AppSettings.SessionState.Cfduid))
            {
                cookieHeaderPt1 = $"{AppSettings.SessionState.Cfduid};";
                //CookieManager.Instance.SetCookie("https://www.bitchute.com/", cookieHeaderPt2);

                CookieManager.Instance.SetCookie("https://www.bitchute.com/", cookieHeaderPt1);

            }
            
            BitChute.Web.ViewClients.Run_OnLogin();
        }

        public static void OnPostLoginFailure()
        {
            Run_OnLoginFailure();
        }

        public static void RunPostLoginWebViewClientUpdate(int viewCalled)
        {
            int tabKey = -1;
            foreach (ServiceWebView view in PlaystateManagement.WebViewTabDictionary.Values)
            {
                view.SetWebViewClient(null);
                tabKey++;
                switch (view.RootUrl)
                {
                    case "https://www.bitchute.com/":
                        if (tabKey == 0) { view.SetWebViewClient(new Home()); }
                        else { view.SetWebViewClient(new Feed()); }
                        break;
                    case "https://www.bitchute.com/subscriptions/":
                        view.SetWebViewClient(new Subs());
                        break;
                    case "https://www.bitchute.com/profile/":
                        view.SetWebViewClient(new MyChannel());
                        break;
                    case "https://www.bitchute.com/settings/":
                        view.SetWebViewClient(new Settings());
                        break;
                }
                if (view.Id != viewCalled && view.RootUrl != null)
                {
                    view.ClearCache(false);
                    view.LoadUrl(view.RootUrl);
                }
            }
            AppState.UserIsLoggedIn = true;
        }

        public static void OnPostLogout(LogoutEventArgs e)
        {
            AppState.UserIsLoggedIn = false;
            AppSettings.SessionState.SessionId = "";
        }
    }

    public class LogoutEventArgs : EventArgs
    {
        private bool _logoutDetected;
        private bool _logoutRequested;
        public LogoutEventArgs()
        {
            
        }
        public bool LogoutDetected { get; set; }
        public bool LogoutRequested { get; set; }
    }
}