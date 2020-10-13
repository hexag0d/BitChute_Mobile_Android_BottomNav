using System.Collections.Generic;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using static BitChute.Services.MainPlaybackSticky;

namespace BitChute
{
    /// <summary>
    /// static class that contains views to make UI 
    /// easier to access universally across the app
    /// </summary>
    public class ViewHelpers
    {
        public static ViewGroup Container { get; set; }

        public class Main
        {
            public static FloatingActionButton DownloadFAB { get; set; }
            public static int FabHeight { get; set; }
            public static RelativeLayout.LayoutParams FabLayoutBottom { get; set; }
            public static RelativeLayout.LayoutParams FabLayoutOrig { get; set; }
            //public static int LayoutAbove { get; set; }
            public static Handler UiHandler = new Handler();

            private static bool _navHidden;
            public static bool NavHidden
            {
                get { return _navHidden; }
                set {_navHidden = value;OnNavBarVizChanged(); }
            }

            public static bool UpdateView(object toBeUpdated, object updateWith)
            {
                UiHandler.Post(() => toBeUpdated = updateWith);
                return true;
            }
            
            public static async void OnNavBarVizChanged()
            {
                await System.Threading.Tasks.Task.Run(() =>
                {
                    if (FabLayoutOrig == null)
                    {
                        FabLayoutOrig = (RelativeLayout.LayoutParams)DownloadFAB.LayoutParameters;
                        FabLayoutBottom = FabLayoutOrig;
                        FabLayoutBottom.AddRule(LayoutRules.AlignParentBottom);
                    }
                });
                if (NavHidden)
                {
                    FabLayoutBottom.AddRule(LayoutRules.AlignParentBottom);
                    UiHandler.Post(() => DownloadFAB.LayoutParameters = FabLayoutBottom);
                }
                else
                {
                    FabLayoutBottom.RemoveRule(LayoutRules.AlignParentBottom);
                    UiHandler.Post(() => DownloadFAB.LayoutParameters = FabLayoutBottom);
                }
            }
        }

        public class PlayStateNotification
        {
            public static RadioButton NoteShouldPlayInBackgroundOnRb { get; set; }
            public static RadioButton NoteShouldPlayInBackgroundOffRb { get; set; }
        }

        public class Tab0
        {
            public static View FragmentContainerLayout { get; set; }
            public static View WebViewFragmentLayout { get; set; }
            public static WebView Wv { get; set; }
            public static View DownloaderLayout { get; set; }
            public static RelativeLayout TabFragmentLinearLayout { get; set; }
        }

        public class Tab1
        {
            public static View FragmentContainerLayout { get; set; }
            public static View WebViewFragmentLayout { get; set; }
            public static WebView Wv { get; set; }
            public static RelativeLayout TabFragmentLinearLayout { get; set; }
        }

        public class Tab2
        {
            public static View FragmentContainerLayout { get; set; }
            public static View WebViewFragmentLayout { get; set; }
            public static WebView Wv { get; set; }
            public static View DownloaderLayout { get; set; }
            public static Button DownloadButton { get; set; }
            public static RelativeLayout TabFragmentLinearLayout { get; set; }
        }

        public class Tab3
        {
            public static View FragmentContainerLayout { get; set; }
            public static View WebViewFragmentLayout { get; set; }
            public static View DownloaderLayout { get; set; }
            public static View WebViewSwapable { get; set; }
            public static WebView Wv { get; set; }
            public static Button DownloadButton { get; set; }
            public static Button GetDownloadFilesButton { get; set; }
            public static Button CancelDownloadButton { get; set; }
            public static EditText DownloadLinkEditText { get; set; }
            public static EditText DownloadFileNameEditText { get; set; }
            public static ProgressBar DownloadProgressBar { get; set; }
            public static TextView DownloadProgressTextView { get; set; }
            public static LinearLayout TabFragmentLinearLayout { get; set; }
            public static CheckBox AutoFillVideoTitleText { get; set; }
            public static Android.Support.V7.Widget.RecyclerView.LayoutManager FileLayoutManager { get; set; }
            public static Android.Support.V7.Widget.RecyclerView FileRecyclerView { get; set; }
        }

        public class Tab4
        {
            public static View FragmentContainerLayout { get; set; }
            public static View WebViewFragmentLayout { get; set; }
            public static View InternalTabbedLayout { get; set; }
            public static View SettingsTabLayout { get; set; }
            public static WebView Wv { get; set; }
            public static LinearLayout TabFragmentLinearLayout { get; set; }
            public static LinearLayout EncoderFlexLinearLayout { get; set; }
            public static LinearLayout WvLayout;
            public static LinearLayout AppSettingsLayout;
            public static View _view;
            public static Button ShowEncoderViewButton { get; set; }
            public static Button ShowWebViewButton { get; set; }
            public static EditText JavascriptInjectionTextBox { get; set; }
            //settings items
            public static Spinner SearchOverrideSourceSpinner { get; set; }
            public static RadioButton SearchOverrideOffRb { get; set; }
            public static RadioButton SearchOverrideOnRb { get; set; }
            public static RadioButton SearchOverrideWithStaticBarRb { get; set; }
            public static RadioButton _fmoffrb;
            public static RadioButton _fmonrb;
            public static RadioButton _zcoffrb;
            public static RadioButton _zconrb;
            public static RadioButton _t3honrb;
            public static RadioButton _t3hoffrb;
            public static RadioButton _t1fonrb;
            public static RadioButton _t1foffrb;
            public static RadioButton _stoverrideoffrb;
            public static RadioButton _stoverrideonrb;
            public static RadioButton _notificationonrb;
            public static RadioButton _notificationoffrb;
            public static RadioButton _hidehorizontalnavbaronrb;
            public static RadioButton _hidehorizontalnavbaroffrb;
            public static RadioButton _hideverticalnavbaronrb;
            public static RadioButton _hideverticalnavbaroffrb;
            public static RadioButton _showdlbuttononpress;
            public static RadioButton _showdlbuttonalways;
            public static RadioButton _showdlbuttonnever;
            public static RadioButton _backgroundkeyfeed;
            public static RadioButton _backgroundkeyany;
            public static RadioButton _autoplayminimizedon;
            public static RadioButton _autoplayfeedonly;
            public static RadioButton _autoplayminimizedoff;
            public static TextView _versionTextView;
            public static List<string> _tabOverrideStringList = new List<string>();
            public static ArrayAdapter<string> _tab4SpinOverrideAdapter;
            public static ArrayAdapter<string> _tab5SpinOverrideAdapter;
            public static List<object> _settingsList = new List<object>();
            public static Spinner _tab4OverrideSpinner;
            public static Spinner _tab5OverrideSpinner;
        }

        public class VideoEncoder
        {
            public static View VideoEncoderLayout { get; set; }
            public static TextView EncodingStatusTextView { get; set; }
            public static TextView AudioEncodingStatusTextView { get; set; }
            public static Button StartEncodingButton { get; set; }
            public static EditText EncoderOutputFileEditText { get; set; }
            public static ProgressBar EncodeProgressBar { get; set; }
            public static ProgressBar AudioEncodeProgressBar { get; set; }
            public static Button PickSourceButton { get; set; }
            public static EditText EncoderSourceEditText { get; set; }
            public static EditText EncoderBitRateEditText { get; set; }
            public static EditText EncoderWidthEditText { get; set; }
            public static EditText EncoderHeightEditText { get; set; }
            public static EditText EncoderFpsEditText { get; set; }
        }
    }
}