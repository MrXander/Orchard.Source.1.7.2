using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Logging;
using Orchard.Widgets.Models;

namespace InsertModuleInContent.Models
{
    public class WidgetParser {
        private const string GROUP_NAME = "name";
        //private static readonly string _regexPattern = ConfigurationManager.AppSettings["Regex"];
        private static readonly Regex _regex = new Regex(@"###(?<name>\w+)###", RegexOptions.IgnoreCase);        

        public static string Replace(string text, IContentManager contentManager) {                        
            if (string.IsNullOrEmpty(text))
                return text;

            //var stopwatch = new Stopwatch();
            //stopwatch.Start();

            var strBuilder = new StringBuilder(text);
            
            foreach (Match match in _regex.Matches(text)) {
                var name = match.Groups[GROUP_NAME];
                
                if (!string.IsNullOrEmpty(name.Value)) {
                    var widget = contentManager.Query<WidgetPart, WidgetPartRecord>()
                        .Where(x => x.Name == name.Value)
                        .List()
                        .FirstOrDefault();
                    
                    if (widget != null) {
                        var w = contentManager.BuildDisplay(widget);
                        var markup = ((ContentItem)w.ContentItem).As<BodyPart>().Text;                        
                        strBuilder.Replace(match.Value, markup);
                    }
                }
            }

            //stopwatch.Stop();
            //NullLogger.Instance.Debug(stopwatch.Elapsed.TotalSeconds.ToString(CultureInfo.InvariantCulture));

            return strBuilder.ToString();
        }
    }
}