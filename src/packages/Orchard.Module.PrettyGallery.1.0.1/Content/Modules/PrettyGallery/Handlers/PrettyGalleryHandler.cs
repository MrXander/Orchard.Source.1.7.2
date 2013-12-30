using Orchard.ContentManagement.Handlers;
using PrettyGallery.Models;
using Orchard.Data;

namespace PrettyGallery.Handlers
{
    public class PrettyGalleryHandler : ContentHandler {
        public PrettyGalleryHandler(IRepository<PrettyGalleryPartRecord> repository) {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}