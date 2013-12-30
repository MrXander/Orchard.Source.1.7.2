using Alexan.PhotoAlbums.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;

namespace Alexan.PhotoAlbums.Handlers
{
    public class PhotoPartHandler : ContentHandler
    {
        public PhotoPartHandler(IRepository<PhotoPartRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}