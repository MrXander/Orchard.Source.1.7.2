using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web;
using System.Web.Mvc;
using Alexan.PhotoAlbums.Extensions;
using Alexan.PhotoAlbums.Models;

namespace Alexan.PhotoAlbums.MVC
{
    public class ImageResult : ActionResult
    {
        public Image Image { get; set; }
        public ImageFormat Format { get; set; }

        public ImageResult(Image image) : this(image, image.RawFormat)
        {
        }

        public ImageResult(Image image, ImageFormat format)
        {
            Image = image;
            Format = format;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            // verify properties 
            if (Image == null)
            {
                throw new ArgumentNullException("Image");
            }
            string mimetype = GetMimeType(Format);
            if (string.IsNullOrWhiteSpace(mimetype))
            {
                throw new ArgumentNullException("ImageFormat");
            }

            var imageData = Image.ToByteArray();
            // output 
            context.HttpContext.Response.Clear();

            context.HttpContext.Response.Cache.SetCacheability(HttpCacheability.Public);
            context.HttpContext.Response.Cache.SetExpires(DateTime.Now.AddYears(1));
            context.HttpContext.Response.ContentType = ImageMIMETypes.GetMIMEType(Format);
            context.HttpContext.Response.OutputStream.Write(imageData, 0, imageData.Length);
        }

        private static string GetMimeType(ImageFormat format)
        {
            foreach (ImageCodecInfo codec in ImageCodecInfo.GetImageDecoders())
            {
                if (codec.FormatID == format.Guid)
                    return codec.MimeType;
            }

            return "image/unknown";
        }
    }
}