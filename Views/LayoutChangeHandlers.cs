using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BitChute.ViewModel;
using static BitChute.ViewModel.VideoDetailLoader;

namespace BitChute.Views
{
    public class LayoutChangeHandlers
    {
        public static void OnDeviceRotation(bool horizontal, int newHeight, int newWidth)
        {
            foreach (var videoView in VideoViewDictionary)
            {
                videoView.Value.LayoutParameters = AppState.Display.GetCurrentVideoContainerLayout();
            }
            if (AppState.Display.Horizontal)
            {
                foreach (var tv in VideoDetailLoader.GetVideoDetailViewTitles())
                {
                    tv.Value.Visibility = ViewStates.Gone;
                }
            }
            else
            {
                foreach (var tv in VideoDetailLoader.GetVideoDetailViewTitles())
                {
                    tv.Value.Visibility = ViewStates.Visible;
                }
            }
        }
    }
}