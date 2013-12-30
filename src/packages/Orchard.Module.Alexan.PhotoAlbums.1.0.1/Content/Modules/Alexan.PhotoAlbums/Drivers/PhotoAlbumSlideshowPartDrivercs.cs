using System;
using System.Linq;
using Alexan.PhotoAlbums.Models;
using Alexan.PhotoAlbums.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Media.Services;

namespace Alexan.PhotoAlbums.Drivers
{
    public class PhotoDisplayPartDriver : ContentPartDriver<PhotoAlbumSlideshowPart>
    {
        protected override DriverResult Display(PhotoAlbumSlideshowPart part, string displayType, dynamic shapeHelper)
        {
            string shapeType;
            Func<dynamic> factory;
            string shapeTypeLayout;
            Func<dynamic> factoryLayout;
            PhotoAlbumPart album = part.As<PhotoAlbumPart>();

            shapeType = "Parts_PhotoAlbums_PhotoAlbumSlideshow";
            shapeTypeLayout = "Parts_PhotoAlbums_PhotoAlbumSlideshow_Layout";
            factory = () => shapeHelper.Parts_PhotoAlbums_PhotoAlbumSlideshow(Part: part, ThumbSize: album.ThumbSize, MaxSize: album.MaxSize,
                AnimationSpeed: album.AnimationSpeed, AutoPlay: part.AutoPlay, Interval: part.Interval);
            factoryLayout = () => shapeHelper.Parts_PhotoAlbums_PhotoAlbumSlideshow_Layout(Part: part, ThumbSize: album.ThumbSize, MaxSize: album.MaxSize);

            return Combined(ContentShape(shapeType, factory), ContentShape(shapeTypeLayout, factoryLayout));
        }

        //GET
        protected override DriverResult Editor(PhotoAlbumSlideshowPart part, dynamic shapeHelper)
        {
            return ContentShape("Parts_PhotoAlbums_PhotoAlbumSlideshow_Edit",
                () => shapeHelper.EditorTemplate(
                    TemplateName: "Parts.PhotoAlbums.PhotoAlbumSlideshow",
                    Model: part,
                    Prefix: Prefix));
        }

        //POST
        protected override DriverResult Editor(PhotoAlbumSlideshowPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }
    }
}