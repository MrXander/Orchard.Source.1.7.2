using Orchard.UI.Resources;

namespace PrettyGallery {
    public class ResourceManifest : IResourceManifestProvider {
        public void BuildManifests(ResourceManifestBuilder builder) {
            var manifest = builder.Add();
            manifest.DefineStyle("prettyPhoto").SetUrl("prettyPhoto.css");
            manifest.DefineStyle("prettyGallery").SetUrl("prettyGallery.css");

            manifest.DefineScript("prettyPhoto").SetUrl("jquery.prettyPhoto.js").SetDependencies("jQuery");
            manifest.DefineScript("jeditable").SetUrl("jquery.jeditable.js").SetDependencies("jQuery");
        }
    }
}
