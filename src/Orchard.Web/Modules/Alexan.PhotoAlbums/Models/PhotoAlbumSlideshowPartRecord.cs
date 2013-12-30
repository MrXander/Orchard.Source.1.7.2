using Orchard.ContentManagement.Records;

namespace Alexan.PhotoAlbums.Models
{
    public class PhotoAlbumSlideshowPartRecord : ContentPartRecord
    {
        public virtual bool AutoPlay { get; set; }
        public virtual int Interval { get; set; }
    }
}