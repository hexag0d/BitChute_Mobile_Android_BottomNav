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
using BitChute.Fragments;
using BitChute.Models;

namespace BitChute.Classes
{
    public class NavigationStack
    {
        public class Tab1
        {
            static List<object> CustomNagivationStack = new List<object>();

            public static bool AddToBackStack(object page)
            {
                CustomNagivationStack.Add(page);
                return true;
            }

            public static bool NavigateBackFromStack()
            {
                if (CustomNagivationStack.Count >= 2)
                {
                    if (typeof(VideoModel.VideoCard) == CustomNagivationStack[CustomNagivationStack.Count - 2].GetType())
                    {
                        TabStates.Tab1.VideoCardLoader = (VideoModel.VideoCard)CustomNagivationStack[CustomNagivationStack.Count - 2];
                        CustomNagivationStack.RemoveRange(CustomNagivationStack.Count - 2, 2);
                    }
                    else if (typeof(CreatorModel.Creator) == CustomNagivationStack[CustomNagivationStack.Count - 2].GetType())
                    {
                        TabStates.Tab1.Creator = (CreatorModel.Creator)CustomNagivationStack[CustomNagivationStack.Count - 2];
                        CustomNagivationStack.RemoveRange(CustomNagivationStack.Count - 2, 2);
                    }
                    else if (typeof(string) == CustomNagivationStack[CustomNagivationStack.Count - 2].GetType())
                    {
                        //TabStates.Tab1.Creator = CustomNagivationStack.Last();
                    }
                    else if (typeof(bool) == CustomNagivationStack[CustomNagivationStack.Count - 2].GetType())
                    {
                        SubscriptionFragment.Pop2Root();
                        CustomNagivationStack.RemoveRange(CustomNagivationStack.Count - 2, 2);
                    }
                }
                else
                {
                    SubscriptionFragment.Pop2Root();
                    RemoveFromBackStack(true);
                }
                return true;
            }


            public static bool RemoveFromBackStack(bool removeAll)
            {
                if (removeAll)
                {
                    CustomNagivationStack.Clear();
                    return true;
                }

                CustomNagivationStack.RemoveAt(CustomNagivationStack.Count - 2);
                return true;
            }
        }
    }
}