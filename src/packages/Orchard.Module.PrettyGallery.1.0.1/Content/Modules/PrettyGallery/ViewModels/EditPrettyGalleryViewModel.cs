using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Orchard.Media.Models;
using PrettyGallery.Models;
using PrettyGallery.Services;

namespace PrettyGallery.ViewModels
{
    public class EditPrettyGalleryViewModel
    {
        public string Description { get; set; }

        [Required(ErrorMessage = "Please select a Media Folder")]
        public string MediaPath { get; set; }

        public string PrettyParameters { get; set; }

        public PrettyParams PrettyParams { get; set; }

        [Required(ErrorMessage = "Thumbnail Height is required")]
        [Display(Name = "Thumbnail Height")]
        [Range(10, 1000)]
        public int ThumbnailHeight { get; set; }

        [Required(ErrorMessage = "Thumbnail Width is required")]
        [Display(Name = "Thumbnail Width")]
        [Range(10, 1000)]
        public int ThumbnailWidth { get; set; }

        public string Layout { get; set; }

        public int Mode { get; set; }

        public string GalleryId { get; set; }

        public IEnumerable<MediaFolder> MediaFolders { get; set; }

        public IEnumerable<PrettyMediaFile> MediaFiles { get; set; }

        public IEnumerable<string> Layouts { get; set; }

        public IEnumerable<Mode> Modes { get; set; }

        public IEnumerable<string> PrettyThemes { get; set; }
    }
}