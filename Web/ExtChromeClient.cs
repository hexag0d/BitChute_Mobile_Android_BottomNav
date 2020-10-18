using Android.App;
using Android.Content;
using Android.Webkit;


namespace BitChute
{
    public class ExtWebChromeClient
    {
        public class ExtendedChromeClient : WebChromeClient
        {
            private static int filechooser = 1;
            private IValueCallback message;
            private MainActivity activity = null;

            public ExtendedChromeClient(MainActivity context)
            {
                this.activity = context;
            }

            public override bool OnShowFileChooser(WebView webView, IValueCallback filePathCallback, FileChooserParams fileChooserParams)
            {
                this.message = filePathCallback;
                Intent chooserIntent = fileChooserParams.CreateIntent();
                chooserIntent.AddCategory(Intent.CategoryOpenable);
                this.activity.StartActivity(Intent.CreateChooser(chooserIntent, "File Chooser"), filechooser, this.OnActivityResult);
                return true;
            }

            private void OnActivityResult(int requestCode, Result resultCode, Intent data)
            {
                if (data != null)
                {
                    if (requestCode == filechooser)
                    {
                        if (null == this.message)
                        {
                            return;
                        }

                        this.message.OnReceiveValue(WebChromeClient.FileChooserParams.ParseResult((int)resultCode, data));
                        this.message = null;
                    }
                }
            }
        }
    }
}