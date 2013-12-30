using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Alexan.PhotoAlbums
{
    public class PhotoAlbumTypes
    {
        public const string Lightbox = "Photo Album - Lightbox";
        public const string Slideshow = "Photo Album - Slideshow";

        public static string[] GetAll()
        {
            return new[]
                   {
                       Lightbox, Slideshow
                   };
        }
    }
}