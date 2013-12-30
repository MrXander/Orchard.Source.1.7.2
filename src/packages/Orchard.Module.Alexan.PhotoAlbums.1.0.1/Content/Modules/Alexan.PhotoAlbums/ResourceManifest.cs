using Orchard.UI.Resources;

namespace Alexan.PhotoAlbums
{
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            builder.Add().DefineStyle("PhotoAlbumsAdmin").SetUrl("photo-albums-admin.css");
            builder.Add().DefineStyle("PhotoAlbums").SetUrl("photo-albums.css");
            builder.Add().DefineStyle("jQueryLightbox").SetUrl("photo-lightbox.css");
            builder.Add().DefineStyle("jQuerySlideshow").SetUrl("photo-slideshow.css");
            builder.Add().DefineStyle("jQueryFileUpload").SetUrl("jquery.fileupload-ui.css");
            //scripts
            builder.Add().DefineScript("jQueryLightbox").SetUrl("jquery.lightbox.js");
            builder.Add().DefineScript("jQuerySlideshow").SetUrl("jquery.slideshow.js");
            
            builder.Add().DefineScript("jQueryFileUploadUI").SetUrl("jquery.fileupload-ui.js");
            builder.Add().DefineScript("jQueryFileUpload").SetUrl("jquery.fileupload.js").SetDependencies("jQuery", "jQueryFileUploadUI");

            builder.Add().DefineScript("AdminFileupload").SetUrl("admin-fileupload.js").SetDependencies("jQueryFileUpload");
            builder.Add().DefineScript("jQueryContextmenu").SetUrl("jquery.contextmenu.js").SetDependencies("jQuery");
            
        }
    }
}