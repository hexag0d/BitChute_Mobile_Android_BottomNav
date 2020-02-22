using Android;
using Android.Support.V7.Widget;
using Android.Views;
using System;
using static BottomNavigationViewPager.MainActivity;

namespace XamarinRecycleView.Adapter
{

    //----------------------------------------------------------------------
    // ADAPTER

    // Adapter to connect the data set (photo album) to the RecyclerView: 
    public class PhotoAlbumAdapter : RecyclerView.Adapter
    {
        // Event handler for item clicks:
        public event EventHandler<int> ItemClick;

        // Underlying data set (a photo album):
        public static RecyclerViewer.PhotoAlbum _photoAlbum;

        // Load the adapter with the data set (photo album) at construction time:
        public PhotoAlbumAdapter(RecyclerViewer.PhotoAlbum photoAlbum)
        {
            _photoAlbum = photoAlbum;
        }

        // Create a new photo CardView (invoked by the layout manager): 
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            // Inflate the CardView for the photo:
            View itemView = LayoutInflater.From(parent.Context).
                        Inflate(2130968629, parent, false);

            // Create a ViewHolder to find and hold these view references, and 
            // register OnClick with the view holder:
            RecyclerViewer.PhotoViewHolder vh = new RecyclerViewer.PhotoViewHolder(itemView, OnClick);
            return vh;
        }

        // Fill in the contents of the photo card (invoked by the layout manager):
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            RecyclerViewer.PhotoViewHolder vh = holder as RecyclerViewer.PhotoViewHolder;

            
            // Set the ImageView and TextView in this ViewHolder's CardView 
            // from this position in the photo album:
            vh.Image.SetImageResource(_photoAlbum[position].PhotoID);
            vh.Caption.Text = _photoAlbum[position].Caption;
        }

        // Return the number of photos available in the photo album:
        public override int ItemCount
        {
            get {
                if (_photoAlbum != null)
                    return _photoAlbum.NumPhotos;
                else
                    return 36;
            }
        }

        // Raise an event when the item-click takes place:
        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }
    }
}
