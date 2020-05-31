using System;
using Android.Content;
using Android.Runtime;
using BitChute.Fragments;
using StartServices.Servicesclass;

namespace BitChute.Classes
{
    public class HeadphoneIntent
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
                //else if(intent.Action ==)
            }

            public static void SendPauseVideoCommand()
            {
                try {
                    TheFragment0.Wv.LoadUrl(JavascriptCommands._jsPauseVideo);
                    TheFragment1.Wv.LoadUrl(JavascriptCommands._jsPauseVideo);
                    TheFragment2.Wv.LoadUrl(JavascriptCommands._jsPauseVideo);
                    TheFragment3.Wv.LoadUrl(JavascriptCommands._jsPauseVideo);
                    TheFragment4.Wv.LoadUrl(JavascriptCommands._jsPauseVideo);
                }
                catch ( Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            //private ComponentName remoteComponentName;
            //private void RegisterRemoteClient()
            //{
            //       remoteComponentName = new ComponentName(PackageName, new RemoteControlBroadcastReceiver().ComponentName);
            //    try
            //    {
            //        if (remoteControlClient == null)
            //        {
            //            ExtStickyService.AudioMan.RegisterMediaButtonEventReceiver(remoteComponentName);
            //            //Create a new pending intent that we want triggered by remote control client
            //            var mediaButtonIntent = new Intent(Intent.ActionMediaButton);
            //            mediaButtonIntent.SetComponent(remoteComponentName);
            //            // Create new pending intent for the intent
            //            var mediaPendingIntent = PendingIntent.GetBroadcast(this, 0, mediaButtonIntent, 0);
            //            // Create and register the remote control client
            //            remoteControlClient = new RemoteControlClient(mediaPendingIntent);
            //            audioManager.RegisterRemoteControlClient(remoteControlClient);
            //        }
            //        //add transport control flags we can to handle
            //        remoteControlClient.SetTransportControlFlags(RemoteControlFlags.Play |
            //                                 RemoteControlFlags.Pause |
            //                                 RemoteControlFlags.PlayPause |
            //                                 RemoteControlFlags.Stop |
            //                                 RemoteControlFlags.Previous |
            //                                 RemoteControlFlags.Next);
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine(ex);
            //    }
            //}
        }
    }
}