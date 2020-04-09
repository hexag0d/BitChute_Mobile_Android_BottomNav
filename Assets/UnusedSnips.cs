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








///// <summary>
///// class that contains video information for the user to select
///// should not contain video data, this class is for the metadata of the video
///// </summary>
//public class VideoCardNoCreator
//{
//    public static List<VideoCardNoCreator> GetVideoCardNoCreatorList()
//    {
//        return VideoCardNoCreatorList;
//    }

//    static List<VideoCardNoCreator> VideoCardNoCreatorList = new List<VideoCardNoCreator>{
//new VideoCardNoCreator (Resource.Drawable._testThumb360_0, "Video 1", "Creator 1","videoID1", Resource.Drawable._testThumb360_0, null, null ),
//new VideoCardNoCreator (Resource.Drawable._testThumb360_1, "Video 2", "Creator 2","videoID2", Resource.Drawable._testThumb360_1, null, null ),
//new VideoCardNoCreator (Resource.Drawable._testThumb360_2, "Video 3", "Creator 3","videoID3", Resource.Drawable._testThumb360_2, null, null ),
//new VideoCardNoCreator (Resource.Drawable._testThumb360_3, "Video 4", "Creator 4","videoID4", Resource.Drawable._testThumb360_3, null, null ),
//new VideoCardNoCreator (Resource.Drawable._testThumb360_4, "Video 5", "Creator 5","videoID5", Resource.Drawable._testThumb360_4, null, null ),
//new VideoCardNoCreator (Resource.Drawable._testThumb360_0, "Video 1", "Creator 1","videoID1", Resource.Drawable._testThumb360_0, null, null ),
//new VideoCardNoCreator (Resource.Drawable._testThumb360_1, "Video 2", "Creator 2","videoID2", Resource.Drawable._testThumb360_1, null, null ),
//new VideoCardNoCreator (Resource.Drawable._testThumb360_2, "Video 3", "Creator 3","videoID3", Resource.Drawable._testThumb360_2, null, null ),
//new VideoCardNoCreator (Resource.Drawable._testThumb360_3, "Video 4", "Creator 4","videoID4", Resource.Drawable._testThumb360_3, null, null ),
//new VideoCardNoCreator (Resource.Drawable._testThumb360_4, "Video 5", "Creator 5","videoID5", Resource.Drawable._testThumb360_4, null, null ),
//new VideoCardNoCreator (Resource.Drawable._testThumb360_0, "Video 1", "Creator 1","videoID1", Resource.Drawable._testThumb360_0, null, null ),
//new VideoCardNoCreator (Resource.Drawable._testThumb360_1, "Video 2", "Creator 2","videoID2", Resource.Drawable._testThumb360_1, null, null ),
//new VideoCardNoCreator (Resource.Drawable._testThumb360_2, "Video 3", "Creator 3","videoID3", Resource.Drawable._testThumb360_2, null, null ),
//new VideoCardNoCreator (Resource.Drawable._testThumb360_3, "Video 4", "Creator 4","videoID4", Resource.Drawable._testThumb360_3, null, null ),
//new VideoCardNoCreator (Resource.Drawable._testThumb360_4, "Video 5", "Creator 5","videoID5", Resource.Drawable._testThumb360_4, null, null ),
//new VideoCardNoCreator (Resource.Drawable._testThumb360_0, "Video 1", "Creator 1","videoID1", Resource.Drawable._testThumb360_0, null, null ),
//new VideoCardNoCreator (Resource.Drawable._testThumb360_1, "Video 2", "Creator 2","videoID2", Resource.Drawable._testThumb360_1, null, null ),
//new VideoCardNoCreator (Resource.Drawable._testThumb360_2, "Video 3", "Creator 3","videoID3", Resource.Drawable._testThumb360_2, null, null ),
//new VideoCardNoCreator (Resource.Drawable._testThumb360_3, "Video 4", "Creator 4","videoID4", Resource.Drawable._testThumb360_3, null, null ),
//new VideoCardNoCreator (Resource.Drawable._testThumb360_4, "Video 5", "Creator 5","videoID5", Resource.Drawable._testThumb360_4, null, null ),
//  };


//    public static List<VideoCardNoCreator> GetVideoCardNoCreatorListSamePerson(Creator creator)
//    {
//        var list = new List<VideoCardNoCreator>{
//new VideoCardNoCreator (Resource.Drawable._testThumb360_0, "Video 1", "","videoID1", Resource.Drawable._testThumb360_0, null, creator.Name ),
//new VideoCardNoCreator (Resource.Drawable._testThumb360_1, "Video 2", "","videoID2", Resource.Drawable._testThumb360_1, null, creator.Name ),
//new VideoCardNoCreator (Resource.Drawable._testThumb360_2, "Video 3", "","videoID3", Resource.Drawable._testThumb360_2, null, creator.Name ),
//new VideoCardNoCreator (Resource.Drawable._testThumb360_3, "Video 4", "","videoID4", Resource.Drawable._testThumb360_3, null, creator.Name ),
//new VideoCardNoCreator (Resource.Drawable._testThumb360_4, "Video 5", "","videoID5", Resource.Drawable._testThumb360_4, null, creator.Name ),
//new VideoCardNoCreator (Resource.Drawable._testThumb360_0, "Video 1", "","videoID1", Resource.Drawable._testThumb360_0, null, creator.Name ),
//new VideoCardNoCreator (Resource.Drawable._testThumb360_1, "Video 2", "","videoID2", Resource.Drawable._testThumb360_1, null, creator.Name ),
//new VideoCardNoCreator (Resource.Drawable._testThumb360_2, "Video 3", "","videoID3", Resource.Drawable._testThumb360_2, null, creator.Name ),
//new VideoCardNoCreator (Resource.Drawable._testThumb360_3, "Video 4", "","videoID4", Resource.Drawable._testThumb360_3, null, creator.Name ),
//new VideoCardNoCreator (Resource.Drawable._testThumb360_4, "Video 5", "","videoID5", Resource.Drawable._testThumb360_4, null, creator.Name ),
//new VideoCardNoCreator (Resource.Drawable._testThumb360_0, "Video 1", "","videoID1", Resource.Drawable._testThumb360_0, null, creator.Name ),
//new VideoCardNoCreator (Resource.Drawable._testThumb360_1, "Video 2", "","videoID2", Resource.Drawable._testThumb360_1, null, creator.Name ),
//new VideoCardNoCreator (Resource.Drawable._testThumb360_2, "Video 3", "","videoID3", Resource.Drawable._testThumb360_2, null, creator.Name ),
//new VideoCardNoCreator (Resource.Drawable._testThumb360_3, "Video 4", "","videoID4", Resource.Drawable._testThumb360_3, null, creator.Name ),
//new VideoCardNoCreator (Resource.Drawable._testThumb360_4, "Video 5", "","videoID5", Resource.Drawable._testThumb360_4, null, creator.Name ),
//new VideoCardNoCreator (Resource.Drawable._testThumb360_0, "Video 1", "","videoID1", Resource.Drawable._testThumb360_0, null, creator.Name ),
//new VideoCardNoCreator (Resource.Drawable._testThumb360_1, "Video 2", "","videoID2", Resource.Drawable._testThumb360_1, null, creator.Name ),
//new VideoCardNoCreator (Resource.Drawable._testThumb360_2, "Video 3", "","videoID3", Resource.Drawable._testThumb360_2, null, creator.Name ),
//new VideoCardNoCreator (Resource.Drawable._testThumb360_3, "Video 4", "","videoID4", Resource.Drawable._testThumb360_3, null, creator.Name ),
//new VideoCardNoCreator (Resource.Drawable._testThumb360_4, "Video 5", "","videoID5", Resource.Drawable._testThumb360_4, null, creator.Name ),
//  };
//        return list;
//    }

//    public VideoCardNoCreator()
//    {
//    }

//    /// <summary>
//    /// this type of VideoCard contains no Creator object
//    /// so it's used for situations where a Creator
//    /// is the container
//    /// 
//    /// Feel free to clean up these classes or redo them
//    /// I'm still trying to work out the most efficient way to do this
//    /// 
//    /// I could use one class for all of them, maybe will eventually
//    /// </summary>
//    /// <param name="id"></param>
//    /// <param name="title"></param>
//    /// <param name="caption2"></param>
//    /// <param name="link"></param>
//    /// <param name="drawableID"></param>
//    /// <param name="thumbbmp"></param>
//    public VideoCardNoCreator(int id, string title, string caption2,
//        string link, int drawableID, Bitmap thumbbmp, string creatorName)
//    {
//        //int for the drawable but I don't know if this is going to work when the app is on an API
//        VideoThumbnail = id;
//        //title
//        Title = title;
//        //description
//        CreatorName = creatorName;
//        Link = link;

//        VideoUri = SampleUri;

//        if (drawableID != 0)
//        {
//            ThumbnailDrawable = MainActivity.UniversalGetDrawable(drawableID);
//        }
//        if (thumbbmp != null)
//        {
//            ThumbnailBitmap = thumbbmp;
//        }
//    }

//    public VideoCardNoCreator(int id, string title, string caption2,
//string link, int drawableID, Bitmap thumbbmp, string creatorName, Android.Net.Uri uri)
//    {
//        //int for the drawable but I don't know if this is going to work when the app is on an API
//        VideoThumbnail = id;
//        //title
//        Title = title;
//        //description
//        CreatorName = creatorName;
//        Link = link;
//        uri = SampleUri; //TODO : remove the sample
//        VideoUri = uri;

//        if (drawableID != 0)
//        {
//            ThumbnailDrawable = MainActivity.UniversalGetDrawable(drawableID);
//        }
//        if (thumbbmp != null)
//        {
//            ThumbnailBitmap = thumbbmp;
//        }
//    }
//    //video thumbnail resource int
//    public int VideoThumbnail { get; set; }
//    //Title of Video
//    public string Title { get; set; }
//    //Description
//    public string Caption2 { get; set; }
//    public string Link { get; set; }
//    public int Index { get; set; }
//    public string CreatorName { get; set; }

//    public Android.Net.Uri VideoUri { get; set; }

//    //I've added these two members because I don't know exactly how the JSON caching is going to work
//    public Drawable ThumbnailDrawable { get; set; }

//    public Bitmap ThumbnailBitmap { get; set; }

//    public bool Equals(VideoCardNoCreator other)
//    {
//        if (this.VideoThumbnail == other.VideoThumbnail && this.Title == other.Title
//            && this.Caption2 == other.Caption2 && this.CreatorName == other.CreatorName
//            && this.ThumbnailBitmap == other.ThumbnailBitmap
//            && this.ThumbnailDrawable == other.ThumbnailDrawable)
//        {

//            return true;
//        }
//        else
//        {
//            return false;
//        }
//    }
//}