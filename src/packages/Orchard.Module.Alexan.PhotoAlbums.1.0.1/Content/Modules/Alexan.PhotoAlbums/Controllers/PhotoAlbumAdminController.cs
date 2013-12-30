using System;
using System.Linq;
using System.Web.Mvc;
using Alexan.PhotoAlbums.Extensions;
using Alexan.PhotoAlbums.Models;
using Alexan.PhotoAlbums.Services;
using Orchard;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.ContentManagement;
using Orchard.Settings;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;

namespace Alexan.PhotoAlbums.Controllers
{
    [ValidateInput(false), Admin]
    public class PhotoAlbumAdminController : Controller, IUpdateModel
    {
        public Localizer T { get; set; }
        private readonly IOrchardServices _services;

        private readonly IContentManager _contentManager;
        private readonly ITransactionManager _transactionManager;
        private readonly ISiteService _siteService;
        private readonly IAlbumService _albumService;
        private readonly IPhotoService _photoService;
        private readonly IMediaExtendedService _mediaService;
        dynamic Shape { get; set; }

        public PhotoAlbumAdminController(IOrchardServices services, IShapeFactory shapeFactory,
            IContentManager contentManager, ITransactionManager transactionManager, ISiteService siteService,
            IAlbumService albumService, IPhotoService photoService, IMediaExtendedService mediaService)
        {
            _services = services;
            Shape = shapeFactory;

            _contentManager = contentManager;
            _transactionManager = transactionManager;
            _siteService = siteService;
            _albumService = albumService;
            _photoService = photoService;
            _mediaService = mediaService;
        }

        public ActionResult Item(int albumId, PagerParameters pagerParameters)
        {
            Pager pager = new Pager(_siteService.GetSiteSettings(), pagerParameters);
            PhotoAlbumPart albumPart = _albumService.Get(albumId, VersionOptions.Latest).As<PhotoAlbumPart>();

            if (albumPart == null)
                return HttpNotFound();

            var photos = _photoService.Get(albumPart, pager.GetStartIndex(), pager.PageSize, VersionOptions.Latest)
                                      .Select<PhotoPart, object>(bp => _contentManager.BuildDisplay(bp, "SummaryAdmin"));

            dynamic album = _services.ContentManager.BuildDisplay(albumPart, "DetailAdmin");

            var list = Shape.List();
            list.AddRange(photos);
            album.Content.Add(Shape.Parts_PhotoAlbums_PhotoAlbum_ListAdmin(ContentItems: list), "5");

            var totalItemCount = _photoService.GetCount(albumPart, VersionOptions.Latest);
            album.Content.Add(Shape.Pager(pager).TotalItemCount(totalItemCount), "Content:after");

            // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
            return View((object)album);
        }

        public ActionResult Summary()
        {
            var albums = _albumService.Get().Select(a => 
                _services.New.PhotoAlbum_SummaryAdmin(
                    ContentItem: a.ContentItem, 
                    PhotoCount: _photoService.GetCount(a, VersionOptions.Published)));

            var list = Shape.List();
            list.AddRange(albums);

            dynamic viewModel = Shape.ViewModel().ContentItems(list);

            // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
            return View((object)viewModel);
        }
        
        public ActionResult Create(string type)
        {
            if (!_services.Authorizer.Authorize(Permissions.ManageAlbums, T("Not allowed to create album")))
                return new HttpUnauthorizedResult();

            IContent album = _albumService.Create(type);
            
            if (album == null)
                return HttpNotFound();
            //setDefault
            var parts = album.ContentItem.Parts.OfType<IPhotoAlbumPart>();
            foreach (var contentPart in parts)
            {
                contentPart.SetDefault();
            }
            dynamic model = _services.ContentManager.BuildEditor(album);
            // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
            return View((object)model);
        }

        [HttpPost, ActionName("Create")]
        public ActionResult CreatePOST(string type)
        {
            if (!_services.Authorizer.Authorize(Permissions.ManageAlbums, T("Couldn't create album")))
                return new HttpUnauthorizedResult();

            var album = _albumService.Create(type);

            _contentManager.Create(album, VersionOptions.Draft);
            dynamic model = _contentManager.UpdateEditor(album, this);

            if (!ModelState.IsValid)
            {
                _transactionManager.Cancel();
                // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
                return View((object)model);
            }

            _contentManager.Publish(album.ContentItem);

            return Redirect(Url.AlbumItem(album.As<PhotoAlbumPart>()));
        }

        public ActionResult Edit(int albumId)
        {
            if (!_services.Authorizer.Authorize(Permissions.ManageAlbums, T("Not allowed to edit album")))
                return new HttpUnauthorizedResult();

            var albumPart = _albumService.Get(albumId, VersionOptions.Latest);
            if (albumPart == null)
                return HttpNotFound();

            dynamic model = _services.ContentManager.BuildEditor(albumPart);
            // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
            return View((object)model);
        }

        [HttpPost, ActionName("Edit")]
        public ActionResult EditPOST(int albumId)
        {
            if (!_services.Authorizer.Authorize(Permissions.ManageAlbums, T("Couldn't edit album")))
                return new HttpUnauthorizedResult();

            var album = _albumService.Get(albumId, VersionOptions.DraftRequired);
            if (album == null)
                return HttpNotFound();

            dynamic model = _services.ContentManager.UpdateEditor(album, this);
            if (!ModelState.IsValid)
            {
                _services.TransactionManager.Cancel();
                // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
                return View((object)model);
            }

            _contentManager.Publish(album);
            
            _services.Notifier.Information(T("Album information updated"));

            return Redirect(Url.AlbumSummary());
        }

        [HttpPost]
        public ActionResult Remove(int albumId, bool deleteFolder)
        {
            if (!_services.Authorizer.Authorize(Permissions.ManageAlbums, T("Couldn't delete album")))
                return new HttpUnauthorizedResult();

            var album = _albumService.Get(albumId, VersionOptions.Latest);

            if (album == null)
                return HttpNotFound();

            _albumService.Delete(album);

            var albumPart = album.As<PhotoAlbumPart>();

            if (deleteFolder)
            {
                try
                {
                    _mediaService.DeleteFolder(albumPart.Name);
                    _services.Notifier.Information(T(string.Format("Folder {0} was successfully deleted.", albumPart.Name)));
                }
                catch (Exception)
                {
                    _services.Notifier.Error(T(string.Format("Cannot delete folder {0}. Please remove it manually.", albumPart.Name)));
                }
                
            }

            _services.Notifier.Information(T("Album was successfully deleted"));
            return Redirect(Url.AlbumSummary());
        }

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties)
        {
            return TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage)
        {
            ModelState.AddModelError(key, errorMessage.ToString());
        }
    }
}