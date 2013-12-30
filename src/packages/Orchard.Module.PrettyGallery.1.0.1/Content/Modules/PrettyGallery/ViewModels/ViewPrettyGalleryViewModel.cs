using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Orchard.Media.Models;
using PrettyGallery.Models;

namespace PrettyGallery.ViewModels
{
    public class ViewPrettyGalleryViewModel
    {
        public string Description { get; set; }
        public string MediaPath { get; set; }
        public string PrettyParameters { get; set; }
        public int ThumbnailHeight { get; set; }
        public int ThumbnailWidth { get; set; }
        public string Layout { get; set; }
        public string GalleryId { get; set; }
        public IEnumerable<PrettyMediaFile> MediaFiles { get; set; }
    }
}