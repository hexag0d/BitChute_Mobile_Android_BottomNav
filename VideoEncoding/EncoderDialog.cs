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

namespace BitChute.VideoEncoding
{
    public class EncoderDialog : Java.Lang.Object, IDialogInterfaceOnClickListener
    {
        public static EncoderDialog StatEncDiag;
        public EncoderDialog() {  }
        public static void ShowDialogQuestion(Context ctx) { StatEncDiag.AskIfUserDesiresPreEncoding(ctx); }
        
        public static string TryVideoPreProcessing = $"Would you like to try video pre-processing?  It uses " +
            $"less data and increases the likelihood that your video will be available on BitChute.  Also, " +
            $"it reduces the EMF exposure to you and those around you, while uploading.  " +
            $"" +
            $"Please be advised that this feature is experimental, and the video will need to process locally." +
            $"Also be aware that you may have to put the app in background while the video processes, as it uses" +
            $"a lot of resources.";

        public bool AskIfUserDesiresPreEncoding (Context ctx)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(ctx);
                    builder.SetMessage(TryVideoPreProcessing).SetPositiveButton("Yes", this)
                      .SetNegativeButton("No", this).Show();
            return true;
        }

        public void OnClick(IDialogInterface dialog, int which)
        {
            switch (which)
            {
                case (int)Android.Content.DialogButtonType.Positive:
                    AppSettings.VideoPreProcessingApproved = true;
                    break;

                case (int)Android.Content.DialogButtonType.Negative:
                    AppSettings.VideoPreProcessingApproved = false;
                    break;
            }
        }
    }
}