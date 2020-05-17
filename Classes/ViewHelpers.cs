using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace BottomNavigationViewPager.Classes
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
            public static Android.Support.Design.Widget.FloatingActionButton DownloadFAB { get; set; }
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
    }
}