using System;
using Android.Content;
using Android.Runtime;
using BitChute.Fragments;

namespace BitChute.Classes
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
                }
                catch ( Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}