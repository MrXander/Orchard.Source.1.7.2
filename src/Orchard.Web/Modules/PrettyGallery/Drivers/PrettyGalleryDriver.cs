using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.Media.Models;
using PrettyGallery.Models;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement;
using PrettyGallery.Services;
using PrettyGallery.ViewModels;

namespace PrettyGallery.Drivers
{
    public class PrettyGalleryDriver : ContentPartDriver<PrettyGalleryPart>
    {
        private readonly IPrettyGalleryService _prettyGalleryService;

        private const string TemplateName = "Parts/PrettyGallery";

        public PrettyGalleryDriver(IPrettyGalleryService prettyGalleryService)
        {
            _prettyGalleryService = prettyGalleryService;
        }

        protected override string Prefix
        {
            get { return "PrettyGallery"; }
        }

        protected string DefaultPrettyParameters
        {
            get { return "{theme: 'dark_square', overlay_gallery: false}"; }
        }

        protected override DriverResult Display(
            PrettyGalleryPart part, string displayType, dynamic shapeHelper)
        {
            return ContentShape("Parts_PrettyGallery",
                () => shapeHelper.Parts_PrettyGallery(
                        VM: BuildViewModel(part)));
        }

        //GET
        protected override DriverResult Editor(PrettyGalleryPart part, dynamic shapeHelper)
        {
            return ContentShape("Parts_PrettyGallery_Edit",
                () => shapeHelper.EditorTemplate(
                    TemplateName: TemplateName,
                    Model: BuildEditorViewModel(part),
                    Prefix: Prefix));
        }

        //POST
        protected override DriverResult Editor(
            PrettyGalleryPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            var model = new EditPrettyGalleryViewModel();
            updater.TryUpdateModel(model, Prefix, null, null);

            if (String.IsNullOrEmpty(model.PrettyParameters))
                model.PrettyParameters = DefaultPrettyParameters;

            if (part.ContentItem.Id != 0)
            {
                _prettyGalleryService.UpdatePrettyGalleryForContentItem(part.ContentItem, model);
            }
            return Editor(part, shapeHelper);
        }

        private EditPrettyGalleryViewModel BuildEditorViewModel(PrettyGalleryPart part) {
            var avm = new EditPrettyGalleryViewModel {
                Description = part.Description,
                MediaPath = part.MediaPath,
                PrettyParams = _prettyGalleryService.DeserializePrettyParams(part.PrettyParameters),
                PrettyParameters = !String.IsNullOrEmpty(part.PrettyParameters)
                                       ? part.PrettyParameters
                                       : DefaultPrettyParameters,
                ThumbnailHeight = part.ThumbnailHeight != 0 ? part.ThumbnailHeight : 150,
                ThumbnailWidth = part.ThumbnailWidth != 0 ? part.ThumbnailWidth : 150,
                Layout = !String.IsNullOrEmpty(part.Layout) ? part.Layout : _prettyGalleryService.GetLayouts().First(),
                Mode = (int) part.Mode,
                Modes = _prettyGalleryService.GetModes(),
                GalleryId = part.Mode == Mode.Zoom ? string.Empty : string.Format("[pp{0}]", part.Id),
                MediaFolders = _prettyGalleryService.GetFolderTree(string.Empty),
                MediaFiles = _prettyGalleryService.GetMediaFiles(part.MediaPath, Status.Any, part.ThumbnailWidth, part.ThumbnailHeight),
                Layouts = _prettyGalleryService.GetLayouts(),
                //PrettyTheme = !String.IsNullOrEmpty(_prettyGalleryService.GetPrettyTheme(part.PrettyParameters)) ? _prettyGalleryService.GetPrettyTheme(part.PrettyParameters) : _prettyGalleryService.GetPrettyThemes().First(),
                PrettyThemes = _prettyGalleryService.GetPrettyThemes()
            };
            return avm;
        }

        private ViewPrettyGalleryViewModel BuildViewModel(PrettyGalleryPart part)
        {
            var avm = new ViewPrettyGalleryViewModel {
                Description = part.Description,
                MediaPath = part.MediaPath,
                PrettyParameters = !String.IsNullOrEmpty(part.PrettyParameters)
                                       ? part.PrettyParameters
                                       : DefaultPrettyParameters,
                ThumbnailHeight = part.ThumbnailHeight != 0 ? part.ThumbnailHeight : 150,
                ThumbnailWidth = part.ThumbnailWidth != 0 ? part.ThumbnailWidth : 150,
                Layout = "Parts/GalleryLayouts/" + (!String.IsNullOrEmpty(part.Layout) ? part.Layout : _prettyGalleryService.GetLayouts().First()),
                GalleryId = part.Mode == Mode.Zoom ? string.Empty : string.Format("[pp{0}]", part.Id),
                MediaFiles = _prettyGalleryService.GetMediaFiles(part.MediaPath, Status.Any, part.ThumbnailWidth, part.ThumbnailHeight),
           };
            return avm;
        }
    }
}