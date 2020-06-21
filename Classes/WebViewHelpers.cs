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
                case 0: TheFragment0.Wv.ScrollTo(0, 0); return;
                case 1: TheFragment1.Wv.ScrollTo(0, 0); return;
                case 2: TheFragment2.Wv.ScrollTo(0, 0); return;
                case 3: TheFragment3.Wv.ScrollTo(0, 0); return;
                case 4: TheFragment4.Wv.ScrollTo(0, 0); return;
            }
        }
    }
}