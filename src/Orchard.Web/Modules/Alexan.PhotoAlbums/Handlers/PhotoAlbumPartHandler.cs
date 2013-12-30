using Alexan.PhotoAlbums.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;

namespace Alexan.PhotoAlbums.Handlers
{
    public class PhotoAlbumPartHandler : ContentHandler
    {
        public PhotoAlbumPartHandler(IRepository<PhotoAlbumPartRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}