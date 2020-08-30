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
                case 0: Tab0Frag.Wv.ScrollTo(0, 0); return;
                case 1: Tab1Frag.Wv.ScrollTo(0, 0); return;
                case 2: Tab2Frag.Wv.ScrollTo(0, 0); return;
                case 3: Tab3Frag.Wv.ScrollTo(0, 0); return;
                case 4: Tab4Frag.Wv.ScrollTo(0, 0); return;
            }
        }
    }
}