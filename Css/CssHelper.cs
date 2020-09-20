using System;
using System.Threading.Tasks;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using BitChute.Classes;
using BitChute.Services;

namespace BitChute.Web.Ui
{
    public class CssHelper
    {
        public static bool CustomCssReadyForRead = false;
        public static string CommonCss = "";
        public static string SearchCss = "";
        public static string CommonCssSubs = "";
        public static string CommonCssFeed = "";
        public static string CommonCssMyChannel = "";
        public static string CommonCssSettings = "";
        private static string _commonCssUrl;
        public static string CommonCssUrl
        {
            get { return _commonCssUrl; }
            set
            {
                if (value != null)
                {
                    _commonCssUrl = value;
                }
            }
        }
        private static string _searchCssUrl;
        public static string SearchCssUrl
        {
            get { return _searchCssUrl; }
            set
            {
                if (value != null)
                {
                    _searchCssUrl = value;
                }
            }
        }

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

        public static async Task<string> GetCommonCss(string sourceUrl, bool process, bool setNext = true)
        {
            string c = await ExtWebInterface.GetHtmlTextFromUrl(sourceUrl); 
            if (process) { c = await GetOverrideCss(c); }
            if (setNext)
            {
                await GetSearchCss(SearchCssUrl);
                await GetFeedCommonCss();
                await GetMyChannelCommonCss();
                await GetSettingsCommonCss();
            }
            CustomCssReadyForRead = true;
            return c;
        }

        public static async Task<string> GetSearchCss(string url = null)
        {
            if (SearchCss == "" || SearchCss == null){ SearchCss = await ExtWebInterface.GetHtmlTextFromUrl(url); }

            Task<string> fct = Task.FromResult<string>(SearchCss
                .Replace(@"""",@"'")
                .Replace(Strings.VidResultImageOrg, Strings.VidResultImageNew)
                .Replace(Strings.VidResultImageContOrg, Strings.VidResultImageContNew)
                .Replace(Strings.ResultListImgContOrg, Strings.ResultListImgContNew)
                .Replace(Strings.VidResultTextOrg, Strings.VidResultTextNew)
                .Replace(Strings.VidResultTextContainerOrg, Strings.VidResultTextContainerNew)
                .Replace("270px", "100%")
                .Replace(@"[max-width~='592px']", "")
                );
            SearchCss = await fct; return SearchCss;
        }
        
        public static async Task<string> GetFeedCommonCss(string bCss = null)
        {
            if (bCss == null) { bCss = CommonCss; } 
            Task<string> fct = Task.FromResult<string>(bCss.Replace(Strings.CarouselOrg(true), Strings.CarouselNew));
            CommonCssFeed = await fct; return CommonCssFeed;
        }

        public static async Task<string> GetMyChannelCommonCss(string bCss = null)
        {
            if (bCss == null) { bCss = CommonCss; }
            Task<string> fct = Task.FromResult<string>(bCss.Replace(Strings.TabScrollInnerNew, Strings.TabScrollInnerOrg));
            CommonCssMyChannel = await fct; return CommonCssMyChannel;
        }

        public static async Task<string> GetSettingsCommonCss(string bCss = null)
        {
            if (bCss == null) { bCss = CommonCss; }
            Task<string> fct = Task.FromResult<string>(bCss.Replace(Strings.TabScrollInnerNew, Strings.TabScrollInnerOrg)
                .Replace(Strings.TopNavBuffNew, Strings.TopNavBuffOrg)
                .Replace(Strings.TopNavNew, Strings.TopNavOrg));
            CommonCssSettings = await fct; return CommonCssSettings;
        }
        
        public static async Task<string> GetOverrideCss(string css, string tabType = null)
        {
            Task<string> gC;
            switch (tabType)
            {
                case null:
                    gC = Task.FromResult<string>(css.Replace("#nav-top-menu{", "#nav-top-menu{display:none;")
                        .Replace("#nav-menu-buffer{", "#nav-menu-buffer{display:none;")
                        .Replace(Strings.TabScrollInnerOrg, Strings.TabScrollInnerNew)
                        .Replace(Strings.OriginalVideoCard, Strings.VideoCardLazy + Strings.NewVideoCard)
                        .Replace(Strings.ChannelCardOriginal, Strings.ChannelCardNew)
                        .Replace(Strings.SubContainerOrg, Strings.SubContainerNew)
                        //.Replace(Strings.ChannelBannerOrg, Strings.ChannelBannerNew) // @TODO these need to be tweaked because they don't look right
                        //.Replace(Strings.ChannelBannerImgOrg, Strings.ChannelBannerImgNew)                    //on the video detail page
                        //.Replace(Strings.CarouselOrg(!AppSettings.Tab1FeaturedOn), Strings.CarouselNew)
                        + Strings.LinkOverflowHide + Strings.ExpandAd + Strings.ExpandFeatured)
                        ; 
                    CommonCss = await gC;
                    break;
            }
            return CommonCss;
        }

        public class Strings
        {
            public static string TopNavNew = @"#nav-top-menu{display:none;";
            public static string TopNavOrg = @"#nav-top-menu{";
            public static string TopNavBuffNew = @"#nav-menu-buffer{display:none;";
            public static string TopNavBuffOrg = @"#nav-menu-buffer{";
            public static string ExpandFeatured = @".img-responsive.hidden-md.hidden-lg.lazyloaded{width:100%}";
            public static string OriginalVideoCard = @".video-card{position:relative;display:inline-block;text-align:left;width:100%;max-width:320px;margin:10px auto;overflow:hidden;-webkit-backface-visibility:hidden}";
            public static string NewVideoCard = @".video-card{position:relative;display:inline-block;text-align:left;width:100%;margin:10px auto;overflow:hidden;-webkit-backface-visibility:hidden}";
            public static string VideoCardLazy = @".video-card .lazyloaded{box-sizing:border-box;width:100%}";
            public static string TabScrollInnerOrg = @".tab-scroll-inner{position:relative;height:50px;overflow:hidden}";
            public static string TabScrollInnerNew = @".tab-scroll-inner{position:relative;overflow:hidden}";
            public static string LinkOverflowHide = @"#video-description{overflow:hidden}";
            public static string ExpandAd = @".img-responsive.lazyloaded{width:100%}";
            public static string ChannelCardOriginal = @".channel-card{position:relative;background-color:#fff;display:inline-block;width:100%;max-width:570px;overflow:hidden}";
            public static string ChannelCardNew = @".channel-card{position:relative;background-color:#fff;display:inline-block;width:100%;overflow:hidden}";
            public static string CarouselOrg(bool hide = false) { if (hide) { return @"#carousel{"; } else { return ""; } }
            public static string CarouselNew = @"#carousel{display:none;";
            public static string ShowFeatured = @"#carousel{";
            public static string SubContainerOrg = @".subscription-container{display:inline-block;text-align:left;width:100%;max-width:400px;margin:10px 0 20px;padding:90px 0 0 10px;position:relative;overflow:hidden}";
            public static string SubContainerNew = @".subscription-container{display:inline-block;text-align:left;width:100%;margin:10px 0 20px;padding:90px 0 0 10px;position:relative;overflow:hidden}";
            public static string ChannelBannerOrg = @".channel-banner .image-container{position:absolute;top:0;left:0;height:106px;width:106px;border-radius:30px;overflow:hidden}";
            public static string ChannelBannerNew = @".channel-banner .image-container{position:absolute;top:0;left:0;height:100%;border-radius:30px;overflow:hidden}";
            public static string ChannelBannerImgOrg = @".channel-banner .image{width:100px;height:100px}";
            public static string ChannelBannerImgNew = @".channel-banner .image{height:100%}";
            public static string VidResultImageOrg = @".video-result-image img{max-width:270px}";
            public static string VidResultImageNew = @".video-result-image img{max-width:100%}";
            public static string VidResultImageContOrg = @".video-result-image-container,.video-result-image,.video-result-image img{max-width:270px}";
            public static string VidResultImageContNew = @".video-result-image-container,.video-result-image,.video-result-image img{max-width:none}";
            public static string ResultListImgContOrg = @".results-list[max-width~='592px'] .video-result-container{flex-direction:column;max-width:270px;max-height:100%}";
            public static string ResultListImgContNew = @".results-list[max-width~='592px'] .video-result-container{flex-direction:column;max-width:100%}";
            public static string VidResultTextOrg = @".video-result-text{margin:5px 0;display:block;display:-webkit-box;min-height:8em;max-height:8em;line-height:1.2em;-webkit-line-clamp:8;-webkit-box-orient:vertical;-moz-box-orient:vertical;-ms-box-orient:vertical;overflow:hidden}";
            public static string VidResultTextNew = @".video-result-text{margin:5px 0;display:none;display:none;max-height:100px;max-width:100%;line-height:1.2em;-webkit-line-clamp:8;-webkit-box-orient:vertical;-moz-box-orient:vertical;-ms-box-orient:vertical;overflow:hidden}";
            public static string VidResultTextContainerOrg = @".video-result-text-container{margin:0;min-width:100%}";
            public static string VidResultTextContainerNew = @".video-result-text-container{margin:0;min-width:100%;max-height:100px;overflow:hidden}";

        }
    }
}
