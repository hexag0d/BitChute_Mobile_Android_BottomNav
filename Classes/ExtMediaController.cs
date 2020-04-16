using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using StartServices.Servicesclass;
using static Android.Widget.MediaController;

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
}