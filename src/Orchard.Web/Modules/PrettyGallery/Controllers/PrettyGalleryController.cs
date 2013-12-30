using System;
using System.Net;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.UI.Admin;
using PrettyGallery.Services;

namespace PrettyGallery.Controllers
{
    //[Admin]
    public class PrettyGalleryController : Controller
    {
        private readonly IPrettyGalleryService _prettyGalleryService;

        public PrettyGalleryController(IOrchardServices services, IPrettyGalleryService prettyGalleryService) {
            Services = services;
            _prettyGalleryService = prettyGalleryService;
            T = NullLocalizer.Instance;
        }

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        [HttpPost]
        public ActionResult UpdateFileInfo(string id, string value, string mediaPath)
        {
            try
            {
                // Check permissions
                if (!Services.Authorizer.Authorize(Permissions.ManagePrettyGalleries, T("Not allowed to edit media information")))
                    return new HttpUnauthorizedResult();

                string[] parameters = id.Split("_".ToCharArray());
                if (parameters.Length != 2)
                    throw new ArgumentException("Format of id is invalid");

                var fileAttribute = parameters[0];
                var fileId = parameters[1];
                var fileName = _prettyGalleryService.GetNameFromId(fileId);
                
                if (String.Compare(fileAttribute, "title", true) == 0)
                    _prettyGalleryService.UpdateFileTitle(mediaPath, fileName, value);
                else if (String.Compare(fileAttribute, "description", true) == 0)
                    _prettyGalleryService.UpdateFileDescription(mediaPath, fileName, value);
                else 
                    throw new ArgumentException("Unknown fileAttribute to update");

                return new ContentResult {Content = value};
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return new ContentResult { Content = e.Message };
            }
        }
    }
}