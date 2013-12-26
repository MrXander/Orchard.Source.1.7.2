using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using InsertModuleInContent.Models;
using Orchard.ContentManagement;
using Orchard.Services;
using Orchard.Widgets.Models;

namespace InsertModuleInContent.Filters
{
    public class HtmlFilter : IHtmlFilter {
        private readonly IContentManager _contentManager;

        public HtmlFilter(IContentManager contentManager) {
            _contentManager = contentManager;
        }

        public string ProcessContent(string text, string flavor) {
            if (!string.IsNullOrEmpty(text)) {
                var widgetNames = WidgetParser.GetWidgetNames(text, _contentManager);
                //var stopwatch = new Stopwatch();
                //stopwatch.Start();
                var widgets = _contentManager.Query<WidgetPart, WidgetPartRecord>().Where(x => x.Name == "TestWidget").List();
                //stopwatch.Stop();
                //Debug.WriteLine(stopwatch.Elapsed.TotalSeconds);
                var widget = widgets.FirstOrDefault();
                if (widget != null)
                {

                }
            }            
            return text;
        }
    }
}