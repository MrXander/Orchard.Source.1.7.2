using InsertModuleInContent.Models;
using Orchard.ContentManagement;
using Orchard.Services;

namespace InsertModuleInContent.Filters
{
    public class HtmlFilter : IHtmlFilter {
        private readonly IContentManager _contentManager;

        public HtmlFilter(IContentManager contentManager) {
            _contentManager = contentManager;
        }

        public string ProcessContent(string text, string flavor) {
            return WidgetParser.Replace(text, _contentManager);
        }
    }
}