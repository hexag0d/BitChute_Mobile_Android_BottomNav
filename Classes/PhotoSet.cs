using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V7.Widget;
using System.Collections.Generic;
using BottomNavigationViewPager;
using Android.Media;

namespace RecyclerViewer
{

    // Photo: contains image resource ID and caption:
    public class VideoCard
    {
        public VideoCard(int id, string caption, string caption2)
        {
            PhotoID = id;
            //title
            Caption = caption;
            //description
            Caption2 = caption2;

            

        }

        // Return the ID of the photo:
        public int PhotoID { get; }

        // Return the Caption of the photo:
        public string Caption { get; }

        public string Caption2 { get; }

        public string Link { get; }
    }

    // Photo album: holds image resource IDs and caption:
    public class PhotoSet
    {
        // Built-in photo collection - this could be replaced with
        // a photo database:

        static List<VideoCard> mBuiltInPhotos = new List<VideoCard>{
new VideoCard (Resource.Drawable._i50, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago50", "postedvideoNAMEdBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago50"),
new VideoCard (Resource.Drawable._i51, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51"),
new VideoCard (Resource.Drawable._i52, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago52", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51"),
new VideoCard (Resource.Drawable._i53, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago53", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51"),
new VideoCard (Resource.Drawable._i54, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago54", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51"),
new VideoCard (Resource.Drawable._i55, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago55", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51"),
new VideoCard (Resource.Drawable._i56, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago56", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51"),
new VideoCard (Resource.Drawable._i57, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago57", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51"),
new VideoCard (Resource.Drawable._i58, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago58", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51"),
new VideoCard (Resource.Drawable._i59, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago59", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51"),
new VideoCard (Resource.Drawable._i60, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago60", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51"),
new VideoCard (Resource.Drawable._i61, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago61", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51"),
new VideoCard (Resource.Drawable._i62, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago62", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51"),
new VideoCard (Resource.Drawable._i63, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago63", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51"),
new VideoCard (Resource.Drawable._i64, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago64", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51"),
new VideoCard (Resource.Drawable._i65, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago65", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51"),
new VideoCard (Resource.Drawable._i66, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago66", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51"),
new VideoCard (Resource.Drawable._i67, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago67", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51"),
new VideoCard (Resource.Drawable._i68, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago68", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51"),
new VideoCard (Resource.Drawable._i69, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago69", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51"),
new VideoCard (Resource.Drawable._i70, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago70", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51"),
new VideoCard (Resource.Drawable._i71, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago71", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51"),
new VideoCard (Resource.Drawable._i72, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago72", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51"),
new VideoCard (Resource.Drawable._i73, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago73", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51"),
new VideoCard (Resource.Drawable._i74, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago74", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51"),
new VideoCard (Resource.Drawable._i75, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago75", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51"),
new VideoCard (Resource.Drawable._i76, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago76", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51"),
new VideoCard (Resource.Drawable._i77, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago77", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51"),
new VideoCard (Resource.Drawable._i78, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago78", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51"),
new VideoCard (Resource.Drawable._i79, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago79", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51"),
new VideoCard (Resource.Drawable._i80, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago80", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51"),
new VideoCard (Resource.Drawable._i81, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago81", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51"),
new VideoCard (Resource.Drawable._i82, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago82", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51"),
new VideoCard (Resource.Drawable._i83, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago83", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51"),
new VideoCard (Resource.Drawable._i84, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago84", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51"),
new VideoCard (Resource.Drawable._i85, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago85", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51"),
new VideoCard (Resource.Drawable._i86, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago86", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51"),
new VideoCard (Resource.Drawable._i87, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago87", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51"),
new VideoCard (Resource.Drawable._i88, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago88", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51"),
new VideoCard (Resource.Drawable._i89, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago89", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51"),
new VideoCard (Resource.Drawable._i90, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago90", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51"),
new VideoCard (Resource.Drawable._i91, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago91", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51"),
new VideoCard (Resource.Drawable._i92, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago92", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51"),
new VideoCard (Resource.Drawable._i93, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago93", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51"),
new VideoCard (Resource.Drawable._i94, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago94", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51"),
new VideoCard (Resource.Drawable._i95, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago95", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51"),
new VideoCard (Resource.Drawable._i96, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago96", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51"),
new VideoCard (Resource.Drawable._i97, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago97", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51"),
new VideoCard (Resource.Drawable._i98, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago98", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51"),
            };




        // Array of photos that make up the album:
        private List<VideoCard> mPhotos;

        // Random number generator for shuffling the photos:
        Random mRandom;

        // Create an instance copy of the built-in photo list and
        // create the random number generator:
        public PhotoSet()
        {
            mPhotos = mBuiltInPhotos;
            mRandom = new Random();
        }

        // Return the number of photos in the photo album:
        public int NumPhotos
        {
            get { return mPhotos.Count; }
        }

        // Indexer (read only) for accessing a photo:
        public VideoCard this[int i]
        {
            get { return mPhotos[i]; }
        }

        // Pick a random photo and swap it with the top:
        public int RandomSwap()
        {
            // Save the photo at the top:
            VideoCard tmpPhoto = mPhotos[0];

            // Generate a next random index between 1 and 
            // Length (noninclusive):
            int rnd = mRandom.Next(1, mPhotos.Count);

            // Exchange top photo with randomly-chosen photo:
            mPhotos[0] = mPhotos[rnd];
            mPhotos[rnd] = tmpPhoto;

            // Return the index of which photo was swapped with the top:
            return rnd;
        }

        // Shuffle the order of the photos:
        public void Shuffle()
        {
            // Use the Fisher-Yates shuffle algorithm:
            for (int idx = 0; idx < mPhotos.Count; ++idx)
            {
                // Save the photo at idx:
                VideoCard tmpPhoto = mPhotos[idx];

                // Generate a next random index between idx (inclusive) and 
                // Length (noninclusive):
                int rnd = mRandom.Next(idx, mPhotos.Count);

                // Exchange photo at idx with randomly-chosen (later) photo:
                mPhotos[idx] = mPhotos[rnd];
                mPhotos[rnd] = tmpPhoto;
            }
        }
    }


    //----------------------------------------------------------------------
    // VIEW HOLDER

    // Implement the ViewHolder pattern: each ViewHolder holds references
    // to the UI components (ImageView and TextView) within the CardView 
    // that is displayed in a row of the RecyclerView:
    public class PhotoViewHolder : RecyclerView.ViewHolder
    {
        public ImageView Image { get; private set; }
        public TextView Caption { get; private set; }
        public TextView Caption2 { get; private set; }

        public void GetControls()
        {
            
        }

        // Get references to the views defined in the CardView layout.
        public PhotoViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            // Locate and cache view references:
            Image = itemView.FindViewById<ImageView>(Resource.Id.imageView);
            Caption = itemView.FindViewById<TextView>(Resource.Id.titleCaptionTextView);
            Caption2 = itemView.FindViewById<TextView>(Resource.Id.descriptionCaptionTextView);

            // Detect user clicks on the item view and report which item
            // was clicked (by layout position) to the listener:
            itemView.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }
}
