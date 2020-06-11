using System;
using System.Threading.Tasks;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using StartServices.Servicesclass;

namespace BitChute.Classes
{
    public class ExtMediaController : MediaController
    {
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
            this.SetPrevNextListeners(new NextClick(), new PreviousClick());
            this.SetOnClickListener(new PlayClick());
        }

        protected override void OnCreateContextMenu(IContextMenu menu)
        {           // this.GetChildAt(0).Click += Child0_OnClick;

            base.OnCreateContextMenu(menu);
        }

        public void Child0_OnClick (object sender, EventArgs e)
        {

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
            ExtStickyService.SkipToPrev(MainActivity.ViewPager.CurrentItem);
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
            ExtStickyService.SkipToNext(null);
        }

        public bool OnLongClick(View v)
        {
            return false;
        }
    }
}