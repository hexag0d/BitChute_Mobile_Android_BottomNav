using BitChute.Fragments;

namespace BitChute.Classes
{
    public class WebViewHelpers
    {
        public static async void DelayedScrollToTop(int tab)
        {
            await System.Threading.Tasks.Task.Delay(AppSettings.ScrollToTopDelay);
            switch (tab)
            {
                case 0: HomePageFrag.Wv.ScrollTo(0, 0); return;
                case 1: SubscriptionFrag.Wv.ScrollTo(0, 0); return;
                case 2: FeedFrag.Wv.ScrollTo(0, 0); return;
                case 3: MyChannelFrag.Wv.ScrollTo(0, 0); return;
                case 4: SettingsFrag.Wv.ScrollTo(0, 0); return;
            }
        }
    }
}