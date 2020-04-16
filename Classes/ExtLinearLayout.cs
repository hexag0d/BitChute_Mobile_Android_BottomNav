using System;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using BitChute.Models;

namespace BitChute.Classes
{
    public class ExtLinearLayout : LinearLayout
    {
        public override bool DispatchKeyEventPreIme(KeyEvent e)
        {
            if (e.KeyCode == Keycode.Back && e.Action == KeyEventActions.Down)
            {
                if (CustomViewHelpers.Common.SoftKeyboardIsVisible)
                {
                    CustomViewHelpers.Common.FocusCommentTextView();
                }
            }
            return base.DispatchKeyEventPreIme(e);
        }

        public ExtLinearLayout(Context context) : base(context)
        {
        }

        public ExtLinearLayout(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public ExtLinearLayout(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
        }

        public ExtLinearLayout(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
        }

        protected ExtLinearLayout(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }
    }
}