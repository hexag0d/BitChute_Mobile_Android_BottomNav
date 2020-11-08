using System;
using Android.Content;
using Android.Runtime;
using BitChute.Fragments;
using BitChute.Services;

namespace BitChute
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
                            PlaystateManagement.SendPauseVideoCommand();
                            break;
                        case 1:
                            //headset plugged
                            break;
                        default:
                            break;
                    }
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
                            PlaystateManagement.SendPauseVideoCommand();
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
        }
    }
}