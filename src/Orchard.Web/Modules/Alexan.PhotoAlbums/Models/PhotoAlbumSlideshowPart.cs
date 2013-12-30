using Orchard.ContentManagement;

namespace Alexan.PhotoAlbums.Models
{
    public class PhotoAlbumSlideshowPart : ContentPart<PhotoAlbumSlideshowPartRecord>, IPhotoAlbumPart
    {
        public void SetDefault()
        {
            AutoPlay = true;
            Interval = 5;
        }

        public virtual bool AutoPlay
        {
            get { return Record.AutoPlay; }
            set { Record.AutoPlay = value; }
        }

        public virtual int Interval
        {
            get { return Record.Interval; }
            set { Record.Interval = value; }
        }
    }
}