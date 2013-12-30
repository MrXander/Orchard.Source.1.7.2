using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Alexan.PhotoAlbums.Extensions
{
    public static class ImageExtensions
    {
        public static Image ResizeImage(this Image source, Size newSize)
        {
            if (source == null || newSize.Width <= 0 || newSize.Height <= 0)
            {
                return null;
            }
            
            try
            {
                PixelFormat format = source.PixelFormat;
                Bitmap bitmap = new Bitmap(newSize.Width, newSize.Height, format);
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.CompositingMode = CompositingMode.SourceOver;
                    graphics.CompositingQuality = CompositingQuality.GammaCorrected;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.DrawImage(source, 0, 0,
                        Convert.ToInt32(newSize.Width), Convert.ToInt32(newSize.Height));
                    return bitmap;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static Size GetNewSize(this Image img, Size maxSize)
        {
            Size newSize;

            //image less then maxSize. do not stretch            

            if (img.Width < maxSize.Width && img.Height < maxSize.Height)
            {
                newSize = new Size(img.Width, img.Height);
            }
            else
            {
                if ((float)maxSize.Width / img.Width < (float)maxSize.Height / img.Height)
                {
                    float ratio = (float)maxSize.Width / img.Width;
                    newSize = new Size(maxSize.Width, (int)(img.Height * ratio));
                }
                else
                {
                    float ratio = (float)maxSize.Height / img.Height;
                    newSize = new Size((int)(img.Width * ratio), maxSize.Height);
                }
            }

            return newSize;
        }

        public static byte[] ToByteArray(this Image img)
        {
            var converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }
    }
}