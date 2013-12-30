using System;
using System.Drawing;
using System.IO;
using System.Web.Mvc;
using Alexan.PhotoAlbums.Extensions;
using Alexan.PhotoAlbums.Models;
using Alexan.PhotoAlbums.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Localization;

namespace Alexan.PhotoAlbums.Drivers
{
    public class PhotoPartDriver : ContentPartDriver<PhotoPart>
    {
        private readonly IMediaExtendedService _mediaService;

        public PhotoPartDriver(IMediaExtendedService mediaService)
        {
            _mediaService = mediaService;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        protected override DriverResult Display(PhotoPart part, string displayType, dynamic shapeHelper)
        {
            return displayType == "Summary"
                ? ContentShape("Parts_PhotoAlbums_Photo_Summary", () => shapeHelper.Parts_PhotoAlbums_Photo_Summary(Name: part.Name, Part: part))
                : ContentShape("Parts_PhotoAlbums_Photo", () => shapeHelper.Parts_PhotoAlbums_Photo(Name: part.Name, Part: part));
        }

        //GET
        protected override DriverResult Editor(PhotoPart part, dynamic shapeHelper)
        {
            return ContentShape("Parts_PhotoAlbums_Photo_Edit",
                () => shapeHelper.EditorTemplate(
                    TemplateName: "Parts.PhotoAlbums.Photo",
                    Model: part,
                    Prefix: Prefix));
        }

        //POST
        protected override DriverResult Editor(PhotoPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            return Editor(part, shapeHelper);
        }
    }
}