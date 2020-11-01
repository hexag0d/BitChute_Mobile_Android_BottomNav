using BitChute;
using BitChute.Web.Ui;
using HtmlAgilityPack;
using System;
using System.Threading.Tasks;

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
        public static async Task<bool> GetObjectsFromHtmlResponse()
        {
            string h = "";
            if (!String.IsNullOrWhiteSpace(AppSettings.SessionState.Cfduid) &&
                !String.IsNullOrWhiteSpace(AppSettings.SessionState.CsrfToken) &&
                !String.IsNullOrWhiteSpace(AppSettings.SessionState.SessionId))
            {
                h = await ExtWebInterface.GetHtmlTextFromUrl("https://www.bitchute.com/notifications", false, true, true);
            }
            else if (String.IsNullOrWhiteSpace(AppSettings.SessionState.Cfduid) || 
                String.IsNullOrWhiteSpace(AppSettings.SessionState.CsrfToken) || 
                String.IsNullOrWhiteSpace(AppSettings.SessionState.SessionId))
            {
                AppSettings.SessionState.Cfduid = "";
                AppSettings.SessionState.CsrfToken = "";
                AppSettings.SessionState.SessionId = "";
                h = await ExtWebInterface.GetHtmlTextFromUrl("https://www.bitchute.com/", true);
                    
                
            }
            else
            {
                h = await ExtWebInterface.GetHtmlTextFromUrl("https://www.bitchute.com/", true);
            }

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            try
            {
                doc.LoadHtml(h);
            }
            catch
            {

            }
            try
            {
                if (doc == null)
                {
                    return false;
                }
                foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//title"))
                {
                    var _tagContents = node.InnerText;
                    if (_tagContents.Contains("Notifications"))
                    {
                        AppState.UserIsLoggedIn = true;
                        ExtNotifications.DecodeHtmlNotifications(h, true, null);

                    }
                    else
                    {
                        AppState.UserIsLoggedIn = false;
                        BitChute.Fragments.HomePageFrag.ShowLoginOnStartup = true;
                    }
                }
            }
            catch
            {
                AppState.UserIsLoggedIn = false;
                BitChute.Fragments.HomePageFrag.ShowLoginOnStartup = true;
            }
            try
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
                foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//input[@name='csrfmiddlewaretoken']"))
                {
                    var csrf = Login.GetLatestCsrfToken(node.Attributes["value"].Value).Value;
                }

                CssHelper.GetCommonCss(CssHelper.CommonCssUrl, true, true);
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            return false;
        }
    }
}