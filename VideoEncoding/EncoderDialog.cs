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
        public static bool UserHasRequestedPreEncoding = false;
        public bool AskIfUserDesiresPreEncoding (Context ctx)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(ctx);
                    builder.SetMessage("Are you sure?").SetPositiveButton("Yes", this)
                      .SetNegativeButton("No", this).Show();
            return true;
        }

        public void OnClick(IDialogInterface dialog, int which)
        {
            switch (which)
            {
                case (int)Android.Content.DialogButtonType.Positive:
                    UserHasRequestedPreEncoding = true;
                    break;

                case (int)Android.Content.DialogButtonType.Negative:
                    UserHasRequestedPreEncoding = false;
                    break;
            }
        }
    }
}