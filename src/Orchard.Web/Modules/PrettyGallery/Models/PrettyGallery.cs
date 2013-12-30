using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;

namespace PrettyGallery.Models
{
    public enum Mode
    {
        Slideshow = 0,
        Zoom = 1
    }

    public class PrettyGalleryPartRecord : ContentPartRecord
    {
        public virtual string MediaPath { get; set; }
        public virtual int ThumbnailWidth { get; set; }
        public virtual int ThumbnailHeight { get; set; }
        public virtual string PrettyParameters { get; set; }
        public virtual string Description { get; set; }
        public virtual string Layout { get; set; }
        public virtual int Mode { get; set; }
    }

    public class PrettyGalleryPart : ContentPart<PrettyGalleryPartRecord>
    {
        public string Description
        {
            get { return Record.Description; }
            set { Record.Description = value; }
        }

        [Required]
        public string MediaPath
        {
            get { return Record.MediaPath; }
            set { Record.MediaPath = value; }
        }

        [Required]
        public int ThumbnailWidth
        {
            get { return Record.ThumbnailWidth; }
            set { Record.ThumbnailWidth = value; }
        }

        [Required]
        public int ThumbnailHeight
        {
            get { return Record.ThumbnailHeight; }
            set { Record.ThumbnailHeight = value; }
        }

        public string PrettyParameters
        {
            get { return Record.PrettyParameters; }
            set { Record.PrettyParameters = value; }
        }

        public string Layout
        {
            get { return Record.Layout; }
            set { Record.Layout = value; }
        }

        public Mode Mode
        {
            get { return (Mode)Record.Mode; }
            set { Record.Mode = (int)value; }
        }
    }
}