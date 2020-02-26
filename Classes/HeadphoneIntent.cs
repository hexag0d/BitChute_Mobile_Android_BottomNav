using System;
using Android.Content;
using Android.Runtime;
using BottomNavigationViewPager.Fragments;

namespace BottomNavigationViewPager.Classes
{
    public class HeadphoneIntent
    {
        public class MusicIntentReceiver : BroadcastReceiver
        {
            public MusicIntentReceiver()
            {
            }

            public MusicIntentReceiver(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
            {
            }

            public override void OnReceive(Context context, Intent intent)
            {
                if (intent.Action == Intent.ActionHeadsetPlug)
                {
                    int state = intent.GetIntExtra("state", -1);
                    switch (state)
                    {
                        case 0:
                            //headset unplugged
                            SendStopVideoCommand();
                            break;
                        case 1:
                            //headset plugged
                            break;
                        default:
                            break;
                    }
                }
            }

            private void SendStopVideoCommand()
            {
                try {
                    TheFragment1._wv.LoadUrl(JavascriptCommands._jsPauseVideo);
                    Fragments.SubscriptionFragment._wv.LoadUrl(JavascriptCommands._jsPauseVideo);
                    TheFragment3._wv.LoadUrl(JavascriptCommands._jsPauseVideo);
                    TheFragment4._wv.LoadUrl(JavascriptCommands._jsPauseVideo);
                    TheFragment5._wv.LoadUrl(JavascriptCommands._jsPauseVideo);
                }
                catch ( Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}