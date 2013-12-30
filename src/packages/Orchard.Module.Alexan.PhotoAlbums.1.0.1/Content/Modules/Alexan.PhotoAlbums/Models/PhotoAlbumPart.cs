using System.Drawing;
using Orchard.ContentManagement;
using Orchard.Core.Routable.Models;

namespace Alexan.PhotoAlbums.Models
{
    public class PhotoAlbumPart : ContentPart<PhotoAlbumPartRecord>, IPhotoAlbumPart
    {
        public void SetDefault()
        {
            MaxWidth = 640;
            MaxHeight = 480;
            ThumbWidth = 120;
            ThumbHeight = 120;
            Resize = true;
            AnimationSpeed = 300;
        }

        public string Name
        {
            get { return this.As<RoutePart>().Title; }
            set { this.As<RoutePart>().Title = value; }
        }
        
        public string Description
        {
            get
            {
                return Record.Description;
            }
            set
            {
                Record.Description = value;
            }
        }

        public int MaxWidth
        {
            get
            {
                return Record.MaxWidth;
            }
            set
            {
                Record.MaxWidth = value;
            }
        }

        public int MaxHeight
        {
            get
            {
                return Record.MaxHeight;
            }
            set
            {
                Record.MaxHeight = value;
            }
        }

        public Size MaxSize
        {
            get
            {
                return new Size(MaxWidth, MaxHeight);
            }
        }

        public int ThumbWidth
        {
            get
            {
                return Record.ThumbWidth;
            }
            set
            {
                Record.ThumbWidth = value;
            }
        }

        public int ThumbHeight
        {
            get
            {
                return Record.ThumbHeight;
            }
            set
            {
                Record.ThumbHeight = value;
            }
        }

        public Size ThumbSize
        {
            get
            {
                return new Size(ThumbWidth, ThumbHeight);
            }
        }

        public bool Resize
        {
            get
            {
                return Record.Resize;
            }
            set
            {
                Record.Resize = value;
            }
        }

        public DisplayTypes DisplayType
        {
            get
            {
                if( this.Is<PhotoAlbumLightboxPart>())
                {
                    return DisplayTypes.Lightbox;
                }

                if (this.Is<PhotoAlbumSlideshowPart>())
                {
                    return DisplayTypes.Slideshow;
                }
                
                return DisplayTypes.Unknown;
            }
        }

        public int AnimationSpeed
        {
            get { return Record.AnimationSpeed; }
            set { Record.AnimationSpeed = value; }
        }
    }
}