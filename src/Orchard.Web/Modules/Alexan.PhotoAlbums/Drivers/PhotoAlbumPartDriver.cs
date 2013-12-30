using System.Linq;
using Alexan.PhotoAlbums.Models;
using Alexan.PhotoAlbums.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Media.Services;

namespace Alexan.PhotoAlbums.Drivers
{
    public class PhotoAlbumPartDriver : ContentPartDriver<PhotoAlbumPart>
    {
        private IPhotoService _photoService;
        private IContentManager _contentManager;

        public PhotoAlbumPartDriver(IPhotoService photoService, IContentManager contentManager)
        {
            _photoService = photoService;
            _contentManager = contentManager;
        }

        protected override DriverResult Display(PhotoAlbumPart part, string displayType, dynamic shapeHelper)
        {
            if(displayType == "SummaryAdmin")
            {
                ContentShape("Parts_PhotoAlbums_PhotoAlbum_Meta",
                             () =>
                             {
                                 int count = _photoService.GetCount(part, VersionOptions.Published);
                                 return shapeHelper.Parts_PhotoAlbums_PhotoAlbum_Meta(Count: count);
                             });
            }
            
            if (displayType == "Detail")
            {
                return Combined( 
                    ContentShape("Parts_PhotoAlbums_PhotoAlbum",
                                     () => shapeHelper.Parts_PhotoAlbums_PhotoAlbum(ContentPart: part)),
                    ContentShape("Parts_PhotoAlbums_PhotoAlbum_Thumbs",
                                    () => 
                                    {
                                        var list = shapeHelper.List();
                                        list.AddRange(_photoService.Get(part, VersionOptions.Published)
                                                          .Select<PhotoPart, object>(bp => _contentManager.BuildDisplay(bp, "Summary")));
                                        return shapeHelper.Parts_PhotoAlbums_PhotoAlbum_Thumbs(ContentPart: part, ContentItems: list);
                                    })
                );
            }

            return null;

        }

        //GET
        protected override DriverResult Editor(PhotoAlbumPart part, dynamic shapeHelper)
        {
            return ContentShape("Parts_PhotoAlbums_PhotoAlbum_Edit",
                () => shapeHelper.EditorTemplate(
                    TemplateName: "Parts.PhotoAlbums.PhotoAlbum",
                    Model: part,
                    Prefix: Prefix));
        }

        //POST
        protected override DriverResult Editor(PhotoAlbumPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }
    }
}