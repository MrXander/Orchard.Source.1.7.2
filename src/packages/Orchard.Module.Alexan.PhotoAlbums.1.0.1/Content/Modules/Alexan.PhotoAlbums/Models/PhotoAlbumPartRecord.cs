using Orchard.ContentManagement.Records;
using Orchard.Data.Conventions;
using System.ComponentModel.DataAnnotations;

namespace Alexan.PhotoAlbums.Models
{
    public class PhotoAlbumPartRecord : ContentPartRecord
    {
        [StringLengthMax]
        public virtual string Description { get; set; }

        [Range(1, 2000)]
        public virtual int MaxWidth { get; set; }
        [Range(1, 2000)]
        public virtual int MaxHeight { get; set; }

        [Range(1, 2000)]
        public virtual int ThumbWidth { get; set; }
        [Range(1, 2000)]
        public virtual int ThumbHeight { get; set; }

        public virtual bool Resize { get; set; }

        public virtual int AnimationSpeed { get; set; }
    }
}