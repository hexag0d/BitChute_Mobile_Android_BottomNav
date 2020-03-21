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
using BitChute.Models;

namespace BitChute.Classes
{
    public class CommentListLoader
    {
        public bool PopulateVideoDetailComments(int tabId, string postId)
        { 
            switch (tabId)
            {
                case 1:
                    CustomViewHelpers.Tab1.CommentSystemRecyclerViewAdapter = 
                        new Adapters.CommentRecyclerViewAdapter(CommentModel.SampleCommentList.GetSampleCommentList());
                    CustomViewHelpers.Tab1.RecyclerView.SetAdapter(CustomViewHelpers.Tab1.CommentSystemRecyclerViewAdapter);
                    break;
            }
            return true;
        }
    }
}