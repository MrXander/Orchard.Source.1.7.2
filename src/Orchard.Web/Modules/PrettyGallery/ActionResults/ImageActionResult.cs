using System.Web.Mvc;
using System.Drawing.Imaging;
using System.Drawing;

namespace PrettyGallery.ActionResults
{
    public class ImageActionResult : ActionResult
    {

        private readonly Image _sourceImage;

        public ImageActionResult(Image sourceImage)
        {
            _sourceImage = sourceImage;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            GenerateImage(context);
        }

        private void GenerateImage(ControllerContext context)
        {
            context.HttpContext.Response.ContentType = "image/jpeg";
            _sourceImage.Save(context.HttpContext.Response.OutputStream, ImageFormat.Jpeg);
            _sourceImage.Dispose();
        }
    }
}
