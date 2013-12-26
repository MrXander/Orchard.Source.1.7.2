using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.DisplayManagement.Implementation;
using Orchard.Widgets.Models;

namespace InsertModuleInContent.Models
{
    public class WidgetParser {
        private const string GROUP_NAME = "name";
        //private static readonly string _regexPattern = ConfigurationManager.AppSettings["Regex"];
        private static readonly Regex _regex = new Regex(@"###(?<name>\w+)###", RegexOptions.IgnoreCase);

        public static string[] GetWidgetNames(string text, IContentManager contentManager) {
            var result = new List<string>();
            
            foreach (Match match in _regex.Matches(text)) {
                var name = match.Groups[GROUP_NAME];
                
                if (!string.IsNullOrEmpty(name.Value)) {
                    var widget = contentManager.Query<WidgetPart, WidgetPartRecord>()
                        .Where(x => x.Name == name.Value)
                        .List()
                        .FirstOrDefault();
                    
                    if (widget != null) {
                        var markup = contentManager.BuildDisplay(widget);
                        var bodyPart = markup.As<BodyPart>();
                        //var m = markup.Content.Items.Where(x => x.Metadata.Prefix)Text;
                        var a = 0;
                    }
                }
            }

            return null;
        }
    }
}