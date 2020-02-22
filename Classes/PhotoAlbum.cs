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
        public class Photo
        {
        public Photo(int id, string caption)
        {
            PhotoID = id;
            Caption = caption;
        }

        // Return the ID of the photo:
        public int PhotoID { get; }

        // Return the Caption of the photo:
        public string Caption { get; }
        }

        // Photo album: holds image resource IDs and caption:
        public class PhotoAlbum
        {
            // Built-in photo collection - this could be replaced with
            // a photo database:

            static Photo[] mBuiltInPhotos = {
new Photo (Resource.Drawable._i50, "image50"),
new Photo (Resource.Drawable._i51, "image51"),
new Photo (Resource.Drawable._i52, "image52"),
new Photo (Resource.Drawable._i53, "image53"),
new Photo (Resource.Drawable._i54, "image54"),
new Photo (Resource.Drawable._i55, "image55"),
new Photo (Resource.Drawable._i56, "image56"),
new Photo (Resource.Drawable._i57, "image57"),
new Photo (Resource.Drawable._i58, "image58"),
new Photo (Resource.Drawable._i59, "image59"),
new Photo (Resource.Drawable._i60, "image60"),
new Photo (Resource.Drawable._i61, "image61"),
new Photo (Resource.Drawable._i62, "image62"),
new Photo (Resource.Drawable._i63, "image63"),
new Photo (Resource.Drawable._i64, "image64"),
new Photo (Resource.Drawable._i65, "image65"),
new Photo (Resource.Drawable._i66, "image66"),
new Photo (Resource.Drawable._i67, "image67"),
new Photo (Resource.Drawable._i68, "image68"),
new Photo (Resource.Drawable._i69, "image69"),
new Photo (Resource.Drawable._i70, "image70"),
new Photo (Resource.Drawable._i71, "image71"),
new Photo (Resource.Drawable._i72, "image72"),
new Photo (Resource.Drawable._i73, "image73"),
new Photo (Resource.Drawable._i74, "image74"),
new Photo (Resource.Drawable._i75, "image75"),
new Photo (Resource.Drawable._i76, "image76"),
new Photo (Resource.Drawable._i77, "image77"),
new Photo (Resource.Drawable._i78, "image78"),
new Photo (Resource.Drawable._i79, "image79"),
new Photo (Resource.Drawable._i80, "image80"),
new Photo (Resource.Drawable._i81, "image81"),
new Photo (Resource.Drawable._i82, "image82"),
new Photo (Resource.Drawable._i83, "image83"),
new Photo (Resource.Drawable._i84, "image84"),
new Photo (Resource.Drawable._i85, "image85"),
new Photo (Resource.Drawable._i86, "image86"),
new Photo (Resource.Drawable._i87, "image87"),
new Photo (Resource.Drawable._i88, "image88"),
new Photo (Resource.Drawable._i89, "image89"),
new Photo (Resource.Drawable._i90, "image90"),
new Photo (Resource.Drawable._i91, "image91"),
new Photo (Resource.Drawable._i92, "image92"),
new Photo (Resource.Drawable._i93, "image93"),
new Photo (Resource.Drawable._i94, "image94"),
new Photo (Resource.Drawable._i95, "image95"),
new Photo (Resource.Drawable._i96, "image96"),
new Photo (Resource.Drawable._i97, "image97"),
new Photo (Resource.Drawable._i98, "image98"),


            };
            


            // Array of photos that make up the album:
        private Photo[] mPhotos;

            // Random number generator for shuffling the photos:
            Random mRandom;

            // Create an instance copy of the built-in photo list and
            // create the random number generator:
            public PhotoAlbum()
            {
                mPhotos = mBuiltInPhotos;
                mRandom = new Random();
            }

            // Return the number of photos in the photo album:
            public int NumPhotos
            {
                get { return mPhotos.Length; }
            }

            // Indexer (read only) for accessing a photo:
            public Photo this[int i]
            {
                get { return mPhotos[i]; }
            }

            // Pick a random photo and swap it with the top:
            public int RandomSwap()
            {
                // Save the photo at the top:
                Photo tmpPhoto = mPhotos[0];

                // Generate a next random index between 1 and 
                // Length (noninclusive):
                int rnd = mRandom.Next(1, mPhotos.Length);

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
                for (int idx = 0; idx < mPhotos.Length; ++idx)
                {
                    // Save the photo at idx:
                    Photo tmpPhoto = mPhotos[idx];

                    // Generate a next random index between idx (inclusive) and 
                    // Length (noninclusive):
                    int rnd = mRandom.Next(idx, mPhotos.Length);

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

        // Get references to the views defined in the CardView layout.
        public PhotoViewHolder(View itemView, Action<int> listener)
            : base(itemView)
        {
            // Locate and cache view references:
            Image = itemView.FindViewById<ImageView>(Resource.Id.imageView);
            Caption = itemView.FindViewById<TextView>(Resource.Id.textView);

            // Detect user clicks on the item view and report which item
            // was clicked (by layout position) to the listener:
            itemView.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }
}
