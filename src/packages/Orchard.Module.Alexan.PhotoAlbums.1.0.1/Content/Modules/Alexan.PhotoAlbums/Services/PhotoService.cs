using System;
using System.Collections.Generic;
using System.Linq;
using Alexan.PhotoAlbums.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;

namespace Alexan.PhotoAlbums.Services
{
    public interface IPhotoService : IDependency
    {
        PhotoPart Create(PhotoAlbumPart album);
        ContentItem Get(int id);
        ContentItem Get(int id, VersionOptions versionOptions);
        IEnumerable<PhotoPart> Get(PhotoAlbumPart albumPart, VersionOptions versionOptions);
        IEnumerable<PhotoPart> Get(PhotoAlbumPart albumPart, int skip, int count, VersionOptions versionOptions);
        int GetCount(PhotoAlbumPart albumPart, VersionOptions versionOptions);

        void Delete(PhotoPart img);
    }

    public class PhotoService : IPhotoService
    {
        private readonly IContentManager _contentManager;

        public PhotoService(IContentManager contentManager)
        {
            _contentManager = contentManager;
        }

        public PhotoPart Create(PhotoAlbumPart album)
        {
            var photo = _contentManager.New<PhotoPart>("Photo");
            photo.AlbumPart = album;
            return photo;
        }

        public ContentItem Get(int id)
        {
            return Get(id, VersionOptions.Latest);
        }

        public ContentItem Get(int id, VersionOptions versionOptions)
        {
            return _contentManager.Get(id, versionOptions);
        }

        public IEnumerable<PhotoPart> Get(PhotoAlbumPart albumPart, VersionOptions versionOptions)
        {
            return GetQuery(albumPart, versionOptions).List().Select(ci => ci.As<PhotoPart>());
        }

        public IEnumerable<PhotoPart> Get(PhotoAlbumPart albumPart, int skip, int count, VersionOptions versionOptions)
        {
            return GetQuery(albumPart, versionOptions).Slice(skip, count).ToList().Select(ci => ci.As<PhotoPart>());
        }

        public int GetCount(PhotoAlbumPart albumPart, VersionOptions versionOptions)
        {
            return GetQuery(albumPart, versionOptions).Count();
        }

        private IContentQuery<ContentItem, CommonPartRecord> GetQuery(ContentPart<PhotoAlbumPartRecord> albumPart, VersionOptions versionOptions)
        {
            return _contentManager.Query(versionOptions, "Photo").Join<CommonPartRecord>().Where(
                    cr => cr.Container == albumPart.Record.ContentItemRecord).OrderByDescending(cr => cr.CreatedUtc);
        }

        public void Delete(PhotoPart photo)
        {
            _contentManager.Remove(photo.ContentItem);
        }
    }
}