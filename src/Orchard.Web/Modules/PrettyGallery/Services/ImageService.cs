using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

namespace PrettyGallery.Services
{
    public class ImageService
    {
        private readonly Stream _stream;
        private readonly string _originalPath;

        /// <summary>
        /// Initializes a new instance of the ImageService class
        /// </summary>
        /// <param name="path">To store image original path</param>
        public ImageService(string path)
        {
            _originalPath = path;
        }

        public ImageService(Stream stream)
        {
            _stream = stream;
        }
        
        /// <summary>
        /// To resize images to different sizes
        /// </summary>
        /// <param name="width">image width</param>
        /// <param name="height">image height </param>
        /// <returns>resized image</returns>
        public Image Resize(int width, int height)
        {
            return Resize(width, height, GetImage());
        }

        private Image GetImage() {
            if (_stream != null)
                return Image.FromStream(_stream);
            else
                return Image.FromFile(_originalPath);
        }

        /// <summary>
        /// To resize image with respect to given ratio.
        /// </summary>
        /// <param name="ratio">ratio of the image.</param>
        /// <param name="path">source of the image.</param>
        /// <returns>resized ratio of the image</returns>
        public Image Resize(int ratio)
        {
            Image tempimage = GetImage();
            return Resize((int)(tempimage.Width * 0.01 * ratio), (int)(tempimage.Height * 0.01 * ratio), GetImage());
        }

        /// <summary>
        /// To resize images to different sizes
        /// </summary>
        /// <param name="width">Image width</param>
        /// <param name="height">image width </param>
        /// <param name="path">path(url) of the image </param>
        /// <returns>resized image</returns>
        public Image Resize(int width, int height, string path)
        {
            return Resize(width, height, Image.FromFile(path));
        }

        /// <summary>
        /// To resize images to different sizes
        /// </summary>
        /// <param name="width">image width</param>
        /// <param name="height">image height</param>
        /// <param name="imageInfo">image informations</param>
        /// <returns>resized image</returns>
        public Image Resize(int width, int height, Image imageInfo)
        {
            decimal heigthWidthRatio = (decimal)imageInfo.Height / (decimal)imageInfo.Width;

            if (width > imageInfo.Width)
            {
                width = imageInfo.Width;
            }

            if ((decimal)imageInfo.Width / width > (decimal)imageInfo.Height / height)
            {
                height = (int)((decimal)width * heigthWidthRatio);
            }
            else
            {
                width = (int)((decimal)height / heigthWidthRatio);
            }

            return GenerateResizedImage(width, height, imageInfo);
        }

        /// <summary>
        /// To generate thumbnail image
        /// </summary>
        /// <param name="width">image width</param>
        /// <param name="height"> image height</param>
        /// <param name="imageInfo"> image informations</param>
        /// <returns>thumbnail image</returns>
        private static Image GenerateResizedImage(int width, int height, Image imageInfo)
        {
            if (width <= 0 | height <= 0)
            {
                throw new Exception("Either width or heigth has invalid value");
            }

            try
            {
                Image thumbNail = new Bitmap(width, height, imageInfo.PixelFormat);
                Graphics graphic = Graphics.FromImage(thumbNail);

                graphic.CompositingQuality = CompositingQuality.HighQuality;
                graphic.SmoothingMode = SmoothingMode.HighQuality;
                graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;

                var rectangle = new Rectangle(0, 0, width, height);
                graphic.DrawImage(imageInfo, rectangle);

                return thumbNail;

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                imageInfo.Dispose();
            }
        }
    }
}
