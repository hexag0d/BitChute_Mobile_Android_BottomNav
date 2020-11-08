using BitChute.Fragments;

namespace BitChute
{
    public class WebViewHelpers
    {
        public static async void DelayedScrollToTop(int tab)
        {
            await System.Threading.Tasks.Task.Delay(AppSettings.ScrollToTopDelay);
            CommonFrag.GetFragmentById(-1, null, MainActivity.ViewPager.CurrentItem).Wv.ScrollTo(0, 0);
        }
    }
}