using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Web.Mvc;
using Alexan.PhotoAlbums.Extensions;
using Alexan.PhotoAlbums.Models;
using Alexan.PhotoAlbums.Services;
using Alexan.PhotoAlbums.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.Core.Contents.Controllers;
using Orchard.Core.Routable.Models;
using Orchard.Core.Routable.Services;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Mvc.Html;
using Orchard.Settings;
using Orchard.UI.Admin;
using Orchard.UI.Notify;

namespace Alexan.PhotoAlbums.Controllers
{
    [ValidateInput(false), Admin]
    public class PhotoAdminController : Controller, IUpdateModel
    {
        private struct UploadResult
        {
            public string ErrorMessage;
            public bool Successfully;

            public UploadResult(string errorMessage, bool successfully)
            {
                ErrorMessage = errorMessage;
                Successfully = successfully;
            }
        }

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }
        private readonly IOrchardServices _services;

        private readonly IContentManager _contentManager;
        private readonly IMediaExtendedService _mediaService;

        private readonly IPhotoService _photoService;
        private readonly IAlbumService _albumService;
        dynamic Shape { get; set; }

        public PhotoAdminController(IOrchardServices services, IShapeFactory shapeFactory, IContentManager contentManager,
            IPhotoService photoService, IAlbumService albumService, IMediaExtendedService mediaService)
        {
            _services = services;
            Shape = shapeFactory;

            _contentManager = contentManager;
            _photoService = photoService;
            _albumService = albumService;
            _mediaService = mediaService;
        }

        public ActionResult Add(int albumId)
        {
            if (!_services.Authorizer.Authorize(Permissions.EditPhoto, T("Not allowed to create photo")))
                return new HttpUnauthorizedResult();

            var album = _albumService.Get(albumId, VersionOptions.Latest).As<PhotoAlbumPart>();
            if (album == null)
                return HttpNotFound();

             // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
            return View((object)album);
        }

        [HttpPost, ActionName("Add")]
        [FormValueRequired("action")]
        public ActionResult AddPOST(int albumId, PhotoCreateViewModel[] photos, string action)
        {
            if (!_services.Authorizer.Authorize(Permissions.EditPhoto, T("Couldn't create photo")))
                return new HttpUnauthorizedResult();

            var album = _albumService.Get(albumId, VersionOptions.Latest).As<PhotoAlbumPart>();
            if (album == null)
                return HttpNotFound();

            foreach (var photoViewModel in photos)
            {
                var photo = _photoService.Get(photoViewModel.Id);
                if(action == "Save")
                {
                    var routePart = photo.As<RoutePart>();
                    routePart.Title = photoViewModel.Name;
                    routePart.Slug = photo.As<IRoutableAspect>().GetEffectiveSlug();
                    _services.ContentManager.Publish(photo);
                }
                else //Cancel
                {
                    Remove(photo.As<PhotoPart>(), true, false);
                }
            }

            return Redirect(Url.AlbumItem(album));
        }

        public ActionResult Edit(int imageId)
        {
            var photo = _photoService.Get(imageId, VersionOptions.Latest);
            if (photo == null)
                return HttpNotFound();

            if (!_services.Authorizer.Authorize(Permissions.EditPhoto, photo.As<PhotoPart>().AlbumPart, T("Couldn't edit photo")))
                return new HttpUnauthorizedResult();

            dynamic model = _services.ContentManager.BuildEditor(photo);
            // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
            return View((object)model);
        }

        [HttpPost, ActionName("Edit")]
        public ActionResult EditPOST(int imageId)
        {
            var photo = _photoService.Get(imageId, VersionOptions.Latest);
            if (photo == null)
                return HttpNotFound();

            if (!_services.Authorizer.Authorize(Permissions.EditPhoto, photo.As<PhotoPart>().AlbumPart, T("Couldn't edit photo")))
                return new HttpUnauthorizedResult();

            dynamic model = _services.ContentManager.UpdateEditor(photo, this);
            if (!ModelState.IsValid)
            {
                _services.TransactionManager.Cancel();
                // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
                return View((object)model);
            }

            _contentManager.Publish(photo);

            _services.Notifier.Information(T("Photo updated"));

            return Redirect(Url.AlbumItem(photo.As<PhotoPart>().AlbumPart));
        }

        [HttpPost]
        public ActionResult Remove(int photoId, bool deleteFile)
        {
            if (!_services.Authorizer.Authorize(Permissions.ManageAlbums, T("Couldn't delete photo")))
                return new HttpUnauthorizedResult();

            //bool removeFile = Request.Form["deleteFile"].ToBool() ?? false;
            var contentItem = _photoService.Get(photoId, VersionOptions.Latest);
            
            if (contentItem == null)
                return HttpNotFound();
            var photo = contentItem.As<PhotoPart>();
            var album = photo.AlbumPart;
            int albumId = album.Id;

            Remove(photo, deleteFile);

            _services.Notifier.Information(T("Photo was successfully deleted"));
            return Request.IsAjaxRequest()
                ? (ActionResult)JavaScript(string.Format("window.location='{0}';", Url.AlbumItem(albumId)))
                : Redirect(Url.AlbumItem(album));
        }

        private void Remove(PhotoPart photo, bool removeFile, bool showNotify = true)
        {
            _photoService.Delete(photo);
            if (removeFile)
            {
                try
                {
                    _mediaService.DeleteFile(photo.FullName, photo.Path);
                    if (showNotify)
                    {
                        _services.Notifier.Information(T(string.Format("File {0} was successfully deleted from media.", photo.FullPath)));
                    }
                }
                catch (Exception)
                {
                    if(showNotify)
                    {
                        _services.Notifier.Error(T(string.Format("Cannot delete file {0} from media. Please remove it manually.", photo.FullPath)));
                    }
                }
            }
        }

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties)
        {
            return TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage)
        {
            ModelState.AddModelError(key, errorMessage.ToString());
        }

        [HttpPost]
        public ActionResult UploadFile(int albumId)
        {
            if (!_services.Authorizer.Authorize(Permissions.EditPhoto, T("Couldn't create photo")))
                return new HttpUnauthorizedResult();

            var album = _albumService.Get(albumId, VersionOptions.Latest).As<PhotoAlbumPart>();
            if (album == null)
                return HttpNotFound();

            var photo = _photoService.Create(album);
             _services.ContentManager.Create(photo, VersionOptions.Draft);
            
            var result = UploadFile(photo);

            if (result.Successfully)
            {
                return Json(new
                            {
                                successfully = true,
                                id = photo.Id,
                                thumb = Url.Thumb(photo.Id, 90, 90),
                                width = 90,
                                height = 90,
                                name = photo.Name,
                            }, "text/x-json", System.Text.Encoding.UTF8);
            }
            else
            {
                _photoService.Delete(photo);
                return Json(new
                            {
                                successfully = false,
                                errorMessage = result.ErrorMessage,
                            }, "text/x-json", System.Text.Encoding.UTF8);
            }
        }

        private UploadResult UploadFile(PhotoPart part)
        {
            var postedFile = Request.Files["file"];
            if (postedFile == null || postedFile.ContentLength == 0)
            {
                return new UploadResult(T("File is empty").ToString(), false);
            }

            if (!_mediaService.IsImage(postedFile))
            {
                return new UploadResult(T("Uploaded file is not image.").ToString(), false);
            }

            string mediaFolder = part.AlbumPart.Name;
            if (string.IsNullOrWhiteSpace(mediaFolder))
            {
                return new UploadResult(T("Album part name is empty").ToString(), false);
            }

            try
            {
                // try to create the folder before uploading a file into it
                _mediaService.CreateFolder(null, mediaFolder);
            }
            catch (Exception)
            {
                // the folder can't be created because it already exists, continue
            }

            var fileName = _mediaService.GetUniqueFilename(mediaFolder, postedFile.FileName);
            part.FileExtension = Path.GetExtension(fileName);
            part.FileName = Path.GetFileNameWithoutExtension(fileName);
            var routePart =  part.As<RoutePart>();
            routePart.Title = Path.GetFileNameWithoutExtension(postedFile.FileName);
            routePart.Slug = part.As<IRoutableAspect>().GetEffectiveSlug();

            try
            {
                if (part.AlbumPart.Resize)  
                {
                    Image originalImage = Image.FromStream(postedFile.InputStream);
                    Size newSize = originalImage.GetNewSize(part.AlbumPart.MaxSize);
                    Image img = originalImage.ResizeImage(newSize);
                    _mediaService.UploadMediaFile(mediaFolder, part.FullName, img.ToByteArray(), false);
                }
                else
                {
                    _mediaService.UploadMediaFile(mediaFolder, part.FullName, postedFile);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Upload image failed");
                return new UploadResult(T("Upload image failed").ToString(), false);
            }

            return new UploadResult(string.Empty, true);
        }
    }
}