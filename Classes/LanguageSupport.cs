﻿using System;
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
    public class LanguageSupport
    {
        public class Common
        {
            public class IO
            {
                public static string FileDownloadSuccess()
                {
                    switch (AppSettings.AppLanguage)
                    {
                        case "en":
                            return English.IO.FileDownloadSuccess;
                    }
                    return English.IO.FileDownloadFailed;
                }
                public static string FileDownloadFailed()
                {
                    switch (AppSettings.AppLanguage)
                    {
                        case "en":
                            return English.IO.FileDownloadFailed;
                    }
                    return English.IO.FileDownloadFailed;
                }
            }
        }

        public class English
        {
            public class IO
            {
                public static string FileDownloadSuccess = "File successfully downloaded or already exists";
                public static string FileDownloadFailed = "File failed to download or already exists";
            }
        }
    }
}