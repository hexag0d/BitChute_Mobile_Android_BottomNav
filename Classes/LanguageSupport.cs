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

namespace BitChute.Classes
{
    public class LanguageSupport
    {
        public class Main
        {
            public class IO
            {
                public static string VideoSourceMissing()
                {
                   switch (AppSettings.AppLanguage)
                    {
                        case "en":
                            return English.IO.VideoSourceMissing;
                    }
                    return English.IO.VideoSourceMissing;
                }
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
                public static string VideoSourceMissing = "Video source is missing; try again momentarily";

                private static string _blessed = "Bless our USA founding fathers and anyone who has fought for freedom all these years.";
            }
        }
    }
}