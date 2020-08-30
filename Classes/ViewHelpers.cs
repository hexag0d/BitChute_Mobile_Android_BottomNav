using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;

namespace BitChute.Classes
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
            public static RelativeLayout.LayoutParams FabLayoutBottom { get; set; }
            public static RelativeLayout.LayoutParams FabLayoutOrig { get; set; }
            //public static int LayoutAbove { get; set; }
            public static Handler UiHandler = new Handler();

            private static bool _navHidden;
            public static bool NavHidden
            {
                get
                {
                    return _navHidden;
                }
                set
                {
                    _navHidden = value;
                    OnNavBarVizChanged();
                }
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

        public class Tab0
        {
            public static View FragmentContainerLayout { get; set; }
            public static View WebViewFragmentLayout { get; set; }
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
            public static LinearLayout TabFragmentLinearLayout { get; set; }
            public static LinearLayout EncoderFlexLinearLayout { get; set; }
            public static TabHost InternalTabHost { get; set; }
            public static TabWidget InternalTabWidget { get; set; }
            public static Button ShowEncoderViewButton { get; set; }
            public static Button ShowWebViewButton { get; set; }
        }

        public class VideoEncoder
        {
            public static View VideoEncoderLayout { get; set; }
            public static TextView EncodingStatusTextView { get; set; }
            public static Button StartEncodingButton { get; set; }
            public static EditText EncoderOutputFileEditText { get; set; }
            public static ProgressBar EncodeProgressBar { get; set; }
            public static Button PickSourceButton { get; set; }
            public static EditText EncoderSourceEditText { get; set; }
        }
    }
}