using System;
using System.Threading.Tasks;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using BitChute.Classes;
using StartServices.Servicesclass;

namespace BitChute.Ui
{
    public class CssHelper
    {
        public static string CommonCss = "";
        public static string CommonCssSubs = "";
        public static string CommonCssFeed = "";
        public static string CommonCssMyChannel = "";
        public static string CommonCssSettings = "";

        public static void DuplicateCss(){ CommonCss = CommonCssSubs = CommonCssFeed = CommonCssMyChannel = CommonCssSettings; }

        public static Android.Webkit.WebResourceResponse GetCssResponse(string c = null)
        {
            if (c == null) { c = CommonCss; }
            var stream = new System.IO.MemoryStream();
            var writer = new System.IO.StreamWriter(stream);
            writer.Write(c);
            stream.Position = 0;
            return new Android.Webkit.WebResourceResponse("text/css", "UTF-8", stream);
        }

        public async static Task<string> GetCommonCss(bool process, bool setNext = false)
        {
            string c = await ExtWebInterface.GetHtmlTextFromUrl("https://www.bitchute.com/static/v123/css/common.css");
            if (process) { c = await GetOverrideCss(c); }
            if (setNext) { }
            return c;
        }


        public static async Task<string> GetOverrideCss(string css, string tabType = null)
        {
            Task<string> gC;
            switch (tabType)
            {
                case null:
                    gC = Task.FromResult<string>(css.Replace("#nav-top-menu{", "#nav-top-menu{display:none;")
                        .Replace("#nav-menu-buffer{", "#nav-menu-buffer{display:none;")
                        .Replace(Strings.TabScrollInner, Strings.TabScrollInnerNew)
                        .Replace(Strings.OriginalVideoCard, Strings.VideoCardLazy + Strings.NewVideoCard)
                        .Replace(Strings.ChannelCardOriginal, Strings.ChannelCardNew)
                        .Replace(Strings.SubContainerOrg, Strings.SubContainerNew)
                        .Replace(Strings.ChannelBannerOrg, Strings.ChannelBannerNew)
                        .Replace(Strings.ChannelBannerImgOrg, Strings.ChannelBannerImgNew) +
                        Strings.LinkOverflowHide + Strings.ExpandAd + Strings.ExpandFeatured + Strings.HideFeatured(AppSettings.Tab1FeaturedOn)
                        );

                    CommonCss = await gC;
                    break;
                    
            }
            return CommonCss;
        }
        
        public static string GetEmbeddedCssHtml(string css)
        {
            return AddStyleTag(css);
        }

        public static string AddStyleTag(string css)
        {
            return $"<style>{css}</style>";
        }

        public static string GetStyles(string type = "general")
        {
            return AddStyleTag(Strings.HideTopNav + Strings.ExpandFeatured);
        }

        public class Strings
        {
            public static string HideTopNav = @"#nav-top-menu{display:none;}#nav-menu-buffer{display:none;}";
            public static string ExpandFeatured = @".img-responsive.hidden-md.hidden-lg.lazyloaded{width:100%}";
            public static string OriginalVideoCard = @".video-card{position:relative;display:inline-block;text-align:left;width:100%;max-width:320px;margin:10px auto;overflow:hidden;-webkit-backface-visibility:hidden}";
            public static string NewVideoCard = @".video-card{position:relative;display:inline-block;text-align:left;width:100%;margin:10px auto;overflow:hidden;-webkit-backface-visibility:hidden}";
            public static string VideoCardLazy = @".video-card .lazyloaded{box-sizing:border-box;width:100%}";
            public static string TabScrollInner = @".tab-scroll-inner{position:relative;height:50px;overflow:hidden}";
            public static string TabScrollInnerNew = @".tab-scroll-inner{position:relative;overflow:hidden}";
            public static string LinkOverflowHide = @"#video-description{overflow:hidden}";
            public static string ExpandAd = @".img-responsive.lazyloaded{width:100%}";
            public static string ChannelCardOriginal = @".channel-card{position:relative;background-color:#fff;display:inline-block;width:100%;max-width:570px;overflow:hidden}";
            public static string ChannelCardNew = @".channel-card{position:relative;background-color:#fff;display:inline-block;width:100%;overflow:hidden}";
            public static string HideFeatured(bool t1f) { if (t1f) { return ""; } else { return @"#carousel{display:none}"; } }
            public static string SubContainerOrg = @".subscription-container{display:inline-block;text-align:left;width:100%;max-width:400px;margin:10px 0 20px;padding:90px 0 0 10px;position:relative;overflow:hidden}";
            public static string SubContainerNew = @".subscription-container{display:inline-block;text-align:left;width:100%;margin:10px 0 20px;padding:90px 0 0 10px;position:relative;overflow:hidden}";
            public static string ChannelBannerOrg = @".channel-banner .image-container{position:absolute;top:0;left:0;height:106px;width:106px;border-radius:30px;overflow:hidden}";
            public static string ChannelBannerNew = @".channel-banner .image-container{position:absolute;top:0;left:0;height:100%;border-radius:30px;overflow:hidden}";
            public static string ChannelBannerImgOrg = @".channel-banner .image{width:100px;height:100px}";
            public static string ChannelBannerImgNew = @".channel-banner .image{height:100%}";
        }
    }
}



/*

 #nav-top-menu {
display: none;
}

#nav-menu-buffer {
display: none;
}



 */
