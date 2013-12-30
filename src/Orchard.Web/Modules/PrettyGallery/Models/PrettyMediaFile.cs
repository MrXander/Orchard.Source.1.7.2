using Orchard.Media.Models;

namespace PrettyGallery.Models
{
    public enum Status
    {
        Pending = 1,
        Published = 2,
        Unpublished = 4,
        Any = 7,
    }

    public class PrettyMediaFile : MediaFile {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Url { get; set; }
        public string ThumbnailUrl { get; set; }
        public int ThumbnailWidth { get; set; }
        public int ThumbnailHeight { get; set; }
        public Status Status { get; set; }
        public bool Changed { get; set; }
    }
}