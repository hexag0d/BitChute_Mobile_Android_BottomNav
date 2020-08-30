using System;
using Android.Content;
using Android.Runtime;
using BitChute.Fragments;
using StartServices.Servicesclass;

namespace BitChute.Classes
{
    public class CustomIntent
    {
        public class ControlIntentReceiver : BroadcastReceiver
        {
            public ControlIntentReceiver()
            {
            }

            public ControlIntentReceiver(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
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
                            SendPauseVideoCommand();
                            break;
                        case 1:
                            //headset plugged
                            break;
                        default:
                            break;
                    }
                }
            }

            public static void SendPauseVideoCommand()
            {
                try {
                    Tab0Frag.Wv.LoadUrl(JavascriptCommands._jsPauseVideo);
                    Tab1Frag.Wv.LoadUrl(JavascriptCommands._jsPauseVideo);
                    Tab2Frag.Wv.LoadUrl(JavascriptCommands._jsPauseVideo);
                    Tab3Frag.Wv.LoadUrl(JavascriptCommands._jsPauseVideo);
                    Tab4Frag.Wv.LoadUrl(JavascriptCommands._jsPauseVideo);
                }
                catch ( Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        
        public class BackgroundIntentReceiver : BroadcastReceiver
        {
            public BackgroundIntentReceiver()
            {
            }

            public BackgroundIntentReceiver(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
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
                            SendPauseVideoCommand();
                            break;
                        case 1:
                            //headset plugged
                            break;
                        default:
                            break;
                    }
                    return;
                }
                intent = ExtNotifications.SwapToBackgroundNotification(intent);
            }

            public static void SendPauseVideoCommand()
            {
                try
                {
                    Tab0Frag.Wv.LoadUrl(JavascriptCommands._jsPauseVideo);
                    Tab1Frag.Wv.LoadUrl(JavascriptCommands._jsPauseVideo);
                    Tab2Frag.Wv.LoadUrl(JavascriptCommands._jsPauseVideo);
                    Tab3Frag.Wv.LoadUrl(JavascriptCommands._jsPauseVideo);
                    Tab4Frag.Wv.LoadUrl(JavascriptCommands._jsPauseVideo);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}