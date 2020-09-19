using BitChute.Classes;
using BitChute.Web.Ui;
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
            string h = await ExtWebInterface.GetHtmlTextFromUrl("https://www.bitchute.com/");

            await Task.Run(() =>
            {
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
                                CssHelper.CommonCssUrl = "https://www.bitchute.com" + node.Attributes["href"].Value;
                            }
                        }
                    }
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            });
            return false;
        }

    }
}