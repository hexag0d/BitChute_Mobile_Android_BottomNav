using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BitChute.Adapters;
using BitChute.Classes;
using BitChute.Fragments;

namespace BitChute.Models
{
    class CreatorDetailLoader
    {
        public async static Task<bool> LoadCreatorPage(View v, CreatorModel.CreatorCard cc)
        {
            switch (MainActivity.ViewPager.CurrentItem)
            {
                case 1:
                    CustomViewHelpers.Tab1.CreatorDetailAvatarImageView.SetImageDrawable(cc.Creator.CreatorThumbnailDrawable);
                    CustomViewHelpers.Tab1.CreatorNameTextView.Text = cc.CreatorName;
                    await Task.Run(() =>
                    {
                        ////set the fragment video card set
                        //SubscriptionFragment.CreatorDetailVideoCardSet = new List<VideoModel.VideoCardNoCreator>(BitChuteAPI.Inbound.GetCreatorRecentVideos(cc.Creator).Result);
                        ////make new adapter
                        //CustomViewHelpers.Tab1.CreatorDetailRecyclerViewAdapter = 
                        //    new CreatorDetailRecyclerViewAdapter(SubscriptionFragment.CreatorDetailVideoCardSet);
                        ////set the new adapter
                        //CustomViewHelpers.Tab1.CreatorDetailRecyclerView.SetAdapter(
                        //    CustomViewHelpers.Tab1.CreatorDetailRecyclerViewAdapter);
                        //CustomViewHelpers.Tab1.CreatorDetailRecyclerViewAdapter.ItemClick 
                        //    += SubscriptionFragment.CreatorDetailAdapter_ItemClick;
                    });
                    SubscriptionFragment.SwapView(CustomViewHelpers.Tab1.CreatorDetailView);
                    break;
            }
            return true;
        }
    }
}