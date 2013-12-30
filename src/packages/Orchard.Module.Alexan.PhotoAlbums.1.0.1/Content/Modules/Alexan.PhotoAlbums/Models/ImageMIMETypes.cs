using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;

namespace Alexan.PhotoAlbums.Models
{
    public static class ImageMIMETypes
    {
        private static readonly Lazy<Dictionary<ImageFormat, string>> _types 
            = new Lazy<Dictionary<ImageFormat, string>>(GetList);

        private static Dictionary<ImageFormat, string> Types
        {
            get { return _types.Value; }
        }
        
        private static Dictionary<ImageFormat, string> GetList()
        {
            return new Dictionary<ImageFormat, string>
                   {
                       {ImageFormat.Bmp, "image/bmp"},
                       {ImageFormat.Gif, "image/gif"},
                       {ImageFormat.Icon, "image/vnd.microsoft.icon"},
                       {ImageFormat.Jpeg, "image/jpeg"},
                       {ImageFormat.Png, "image/png"},
                       {ImageFormat.Tiff, "image/tiff"},
                       {ImageFormat.Wmf, "image/wmf"},
                   };
        }

        public static string GetMIMEType(ImageFormat format)
        {
            if (format == null)
            {
                return string.Empty;
            }
            ImageFormat key = Types.Keys.FirstOrDefault(k => k.Guid.Equals(format));

            return key == null ? string.Empty : Types[key];
        }
    }
}