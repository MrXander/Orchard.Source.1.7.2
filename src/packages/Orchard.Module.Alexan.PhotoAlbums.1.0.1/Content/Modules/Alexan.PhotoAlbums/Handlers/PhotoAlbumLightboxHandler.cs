﻿using Alexan.PhotoAlbums.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;

namespace Alexan.PhotoAlbums.Handlers
{
    public class PhotoAlbumLightboxHandler : ContentHandler
    {
        public PhotoAlbumLightboxHandler(IRepository<PhotoAlbumSlideshowPartRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}