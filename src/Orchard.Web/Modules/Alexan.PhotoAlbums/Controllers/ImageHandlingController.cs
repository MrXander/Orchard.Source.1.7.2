using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Web.Mvc;
using Alexan.PhotoAlbums.Extensions;
using Alexan.PhotoAlbums.Models;
using Alexan.PhotoAlbums.MVC;
using Alexan.PhotoAlbums.Services;
using Orchard.ContentManagement;

namespace Alexan.PhotoAlbums.Controllers
{
    public class ImageHandlingController : Controller
    {
        private IMediaExtendedService _mediaExtendedService;
        private readonly IPhotoService _imageService;
        private readonly IAlbumService _albumService;

        public ImageHandlingController(IMediaExtendedService mediaExtendedService, IPhotoService imageService, IAlbumService albumService)
        {
            _mediaExtendedService = mediaExtendedService;
            _imageService = imageService;
            _albumService = albumService;
        }

        [OutputCache(Duration = 60, VaryByParam="*")]
        public ActionResult Thumb(int photoId, int width, int height)
        {
            var imgPart = _imageService.Get(photoId, VersionOptions.Latest).As<PhotoPart>();
            if (imgPart == null)
                return HttpNotFound();

            string url = _mediaExtendedService.GetPublicUrl(imgPart.FullPath);
            string localPath = Server.MapPath(url);

            try
            {
                using (Image img = Image.FromFile(localPath))
                {
                    ImageFormat format = img.RawFormat;

                    Size thumbSize = img.GetNewSize(new Size(width, height));
                    var newImage = img.ResizeImage(thumbSize);
                    return new ImageResult(newImage, format);
                }
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }
    }
}