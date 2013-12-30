using Orchard.ContentManagement.Records;
using System.ComponentModel.DataAnnotations;
using Orchard.Data.Conventions;

namespace Alexan.PhotoAlbums.Models
{
    public class PhotoPartRecord : ContentPartRecord
    {
        [Required]
        public virtual string FileName { get; set; }

        public virtual string FileExtension { get; set; }
    }
}