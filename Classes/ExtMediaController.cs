using System;
using System.Threading.Tasks;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using BitChute.Services;
using static BitChute.Services.MainPlaybackSticky;

namespace BitChute
{
    public class ExtMediaController : MediaController, MediaController.IMediaPlayerControl
    {
        public int AudioSessionId
        {
            get { return 0; }
        }

        public int BufferPercentage
        {
            get
            {
                if (MediaPlayerDictionary.ContainsKey(PlaystateManagement.MediaPlayerNumberIsStreaming))
                {
                    return 100;
                }
                return 0;
            }
        }

        public int CurrentPosition
        {
            get
            {
                if (MediaPlayerDictionary.ContainsKey(PlaystateManagement.MediaPlayerNumberIsStreaming))
                {
                    return MediaPlayerDictionary[PlaystateManagement.MediaPlayerNumberIsStreaming].CurrentPosition;
                }
                return 0;
            }
            set
            {
                if (MediaPlayerDictionary.ContainsKey(PlaystateManagement.MediaPlayerNumberIsStreaming))
                {
                    MediaPlayerDictionary[PlaystateManagement.MediaPlayerNumberIsStreaming].SeekTo(value);
                }
            }
        }

        public int Duration
        {
            get
            {
                if (MediaPlayerDictionary.ContainsKey(PlaystateManagement.MediaPlayerNumberIsStreaming))
                {
                    return MediaPlayerDictionary[PlaystateManagement.MediaPlayerNumberIsStreaming].Duration;
                }
                return 0;
            }
        }

        public bool IsPlaying
        {
            get
            {
                if (MediaPlayerDictionary.ContainsKey(PlaystateManagement.MediaPlayerNumberIsStreaming))
                {
                    return MediaPlayerDictionary[PlaystateManagement.MediaPlayerNumberIsStreaming].IsPlaying;
                }
                else { return false; }
            }
        }

        public override void Hide()
        {
            base.Hide();
        }

        public bool HideAfterDelay(int delay)
        {
            Task.Delay(delay);
            this.Visibility = ViewStates.Gone;
            return true;
        }
        
        public ExtMediaController(Context context) : base(context)
        {
            this.NextClick += Next_OnClick;
            this.PreviousClick += Prev_OnClick;
            this.SetPrevNextListeners(new NextClick(), new PreviousClick());
            this.SetOnClickListener(new PlayClick());
        }

        public static void Next_OnClick(object sender, EventArgs e)
        {

        }

        public static void Prev_OnClick (object sender, EventArgs e)
        {

        }

        protected override void OnCreateContextMenu(IContextMenu menu)
        {           // this.GetChildAt(0).Click += Child0_OnClick;

            base.OnCreateContextMenu(menu);
        }

        public void Child0_OnClick (object sender, EventArgs e)
        {

        }

        public bool CanPause()
        {
            if (MediaPlayerDictionary.ContainsKey(PlaystateManagement.MediaPlayerNumberIsStreaming))
            {
                return MediaPlayerDictionary[PlaystateManagement.MediaPlayerNumberIsStreaming].IsPlaying;
            }
            return false;
        }

        public bool CanSeekBackward() { return true; }

        public bool CanSeekForward()
        {
            return true;
        }

        void ExtMediaController.IMediaPlayerControl.Pause()
        {
            ExtStickyServ.Pause();
        }

        public void SeekTo(int pos)
        {
            CurrentPosition = pos;
        }

        void ExtMediaController.IMediaPlayerControl.Start()
        {
            ExtStickyServ.Play();
        }


        public ExtMediaController(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            
        }

        public ExtMediaController(Context context, bool useFastForward) : base(context, useFastForward)
        {
        }

        protected ExtMediaController(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }
    }

    public class PlayClick : Java.Lang.Object, View.IOnClickListener
    {
        public void OnClick(View v)
        {
        }
    }

    public class PreviousClick : Java.Lang.Object, View.IOnClickListener, View.IOnLongClickListener
    {
        public void OnClick(View v)
        {
            MainPlaybackSticky.SkipToPrev();
        }

        public bool OnLongClick(View v)
        {
            return false;
        }
    }

    public class NextClick : Java.Lang.Object, View.IOnClickListener, View.IOnLongClickListener
    {
        public void OnClick(View v)
        {
            MainPlaybackSticky.SkipToNext(null);
        }

        public bool OnLongClick(View v)
        {
            return false;
        }
    }
}