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
    public class ViewHelpers
    {
        public static ViewGroup Container { get; set; }

        public class Tab3
        {
            public static View FragmentContainerLayout { get; set; }
            public static View WebViewFragmentLayout { get; set; }
            public static View DownloaderLayout { get; set; }
            public static View WebViewSwapable { get; set; }
            public static Button DownloadButton { get; set; }
            public static EditText DownloadLinkEditText { get; set; }
            public static EditText DownloadFileNameEditText { get; set; }
            public static ProgressBar DownloadProgressBar { get; set; }
            public static LinearLayout TabFragmentLinearLayout { get; set; }
        }
    }
}