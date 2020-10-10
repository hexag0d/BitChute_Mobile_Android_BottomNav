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

namespace BitChute
{
    public class MediaHelper
    {
        public static int GetDuration(Android.Net.Uri uri)
        {
            Android.Media.MediaMetadataRetriever m = new Android.Media.MediaMetadataRetriever();
            m.SetDataSourceAsync(Android.App.Application.Context, uri);
            String durationStr = m.ExtractMetadata(Android.Media.MetadataKey.Duration);
            int millSecond = int.Parse(durationStr);
            return millSecond;
        }

        public static long GetDurationBitsByUri(Android.Net.Uri uri, int bitRate)
        {
            Android.Media.MediaMetadataRetriever m = new Android.Media.MediaMetadataRetriever();
            m.SetDataSourceAsync(Android.App.Application.Context, uri);
            String durationStr = m.ExtractMetadata(Android.Media.MetadataKey.Duration);
            int millSecond = int.Parse(durationStr);
            long db = (long)(((decimal)millSecond / (decimal)1000) * bitRate);
            return db ;
        }
    }
}