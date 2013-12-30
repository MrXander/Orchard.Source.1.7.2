using System;
using System.Linq;
using Alexan.PhotoAlbums.Models;
using Alexan.PhotoAlbums.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Media.Services;

namespace Alexan.PhotoAlbums.Drivers
{
    public class PhotoAlbumLightboxPartDriver : ContentPartDriver<PhotoAlbumLightboxPart>
    {
        protected override DriverResult Display(PhotoAlbumLightboxPart part, string displayType, dynamic shapeHelper)
        {
            PhotoAlbumPart album = part.As<PhotoAlbumPart>();

            string shapeType = "Parts_PhotoAlbums_PhotoAlbumLightbox";
            string shapeTypeLayout = "Parts_PhotoAlbums_PhotoAlbumLightbox_Layout";
            Func<dynamic> factory = () => shapeHelper.Parts_PhotoAlbums_PhotoAlbumLightbox(Part: part, ThumbSize: album.ThumbSize, AnimationSpeed: album.AnimationSpeed);
            Func<dynamic> factoryLayout = () => shapeHelper.Parts_PhotoAlbums_PhotoAlbumLightbox_Layout(Part: part, ThumbSize: album.ThumbSize);
 
            return Combined(ContentShape(shapeType, factory), 
                            ContentShape(shapeTypeLayout, factoryLayout));
        }

        //GET
        protected override DriverResult Editor(PhotoAlbumLightboxPart part, dynamic shapeHelper)
        {
            return ContentShape("Parts_PhotoAlbums_PhotoAlbumLightbox_Edit",
                () => shapeHelper.EditorTemplate(
                    TemplateName: "Parts.PhotoAlbums.PhotoAlbumLightbox",
                    Model: part,
                    Prefix: Prefix));
        }

        //POST
        protected override DriverResult Editor(PhotoAlbumLightboxPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }
    }
}