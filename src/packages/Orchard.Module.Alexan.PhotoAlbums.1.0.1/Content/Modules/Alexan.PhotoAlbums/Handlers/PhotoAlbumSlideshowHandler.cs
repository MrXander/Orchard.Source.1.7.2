using Alexan.PhotoAlbums.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;

namespace Alexan.PhotoAlbums.Handlers
{
    public class PhotoAlbumSlideshowHandler : ContentHandler
    {
        public PhotoAlbumSlideshowHandler(IRepository<PhotoAlbumSlideshowPartRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}