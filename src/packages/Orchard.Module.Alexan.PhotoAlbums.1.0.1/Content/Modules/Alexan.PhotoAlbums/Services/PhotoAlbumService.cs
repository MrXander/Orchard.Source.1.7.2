using System;
using System.Collections.Generic;
using Alexan.PhotoAlbums.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Routable.Models;
using System.Linq;

namespace Alexan.PhotoAlbums.Services
{
    public interface IAlbumService : IDependency
    {
        IContent Create(string type);
        IEnumerable<PhotoAlbumPart> Get();
        ContentItem Get(int id, VersionOptions versionOptions);

        void Delete(ContentItem album);
    }

    public class PhotoAlbumService : IAlbumService
    {
        private readonly IContentManager _contentManager;
        
        public PhotoAlbumService(IContentManager contentManager)
        {
            _contentManager = contentManager;
        }

        public IContent Create(string type)
        {
            if(!PhotoAlbumTypes.GetAll().Contains(type))
            {
                return null;
            }
            return _contentManager.New(type);
        }

        public IEnumerable<PhotoAlbumPart> Get()
        {
            return _contentManager.Query<PhotoAlbumPart>(PhotoAlbumTypes.GetAll())
                .Join<RoutePartRecord>()
                .OrderBy(br => br.Title)
                .List();
        }

        public ContentItem Get(int id, VersionOptions versionOptions)
        {
            return _contentManager.Get(id, versionOptions);
        }

        public void Delete(ContentItem album)
        {
            _contentManager.Remove(album);
        }
    }
}