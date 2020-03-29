//            if (MainActivity._viewPager.CurrentItem == 2)
//            {
//                


//     string path = "android.resource://" + "com.xamarin.example.BitChute" + "/" + Resource.Raw.mylastvidd;

//                    uri = Android.Net.Uri.Parse("android.resource://" + "com.xamarin.example.BitChute" + "/" + Resource.Raw.mylastvidd);
//            }



//public bool OnTouch(View v, MotionEvent e)
//{
//    return false;
//}

//private void ViewOnTouch(object sender, View.TouchEventArgs touchEventArgs)
//{
//    // _wv.ComputeScroll();
//    //Globals._wvHeight = _wv.ContentHeight;

//    //string message;
//    switch (touchEventArgs.Event.Action & MotionEventActions.Mask)
//    {
//        case MotionEventActions.Down:
//        //case MotionEventActions.Move:
//        //    _main.CustomOnScroll();
//        //    break;

//        case MotionEventActions.Up:
//            var check = 0;
//            //_main.HideNavBarAfterDelay();
//            break;
//        default:
//            break;
//    }
//}


//// Pick a random photo and swap it with the top:
//public int RandomSwap()
//{
//    // Save the photo at the top:
//    VideoCard tmpPhoto = mPhotos[0];

//    // Generate a next random index between 1 and 
//    // Length (noninclusive):
//    int rnd = mRandom.Next(1, mPhotos.Count);

//    // Exchange top photo with randomly-chosen photo:
//    mPhotos[0] = mPhotos[rnd];
//    mPhotos[rnd] = tmpPhoto;

//    // Return the index of which photo was swapped with the top:
//    return rnd;
//}


//// Shuffle the order of the photos:
//public void Shuffle()
//{
//    // Use the Fisher-Yates shuffle algorithm:
//    for (int idx = 0; idx < mPhotos.Count; ++idx)
//    {
//        // Save the photo at idx:
//        VideoCard tmpPhoto = mPhotos[idx];

//        // Generate a next random index between idx (inclusive) and 
//        // Length (noninclusive):
//        int rnd = mRandom.Next(idx, mPhotos.Count);

//        // Exchange photo at idx with randomly-chosen (later) photo:
//        mPhotos[idx] = mPhotos[rnd];
//        mPhotos[rnd] = tmpPhoto;
//    }
//}