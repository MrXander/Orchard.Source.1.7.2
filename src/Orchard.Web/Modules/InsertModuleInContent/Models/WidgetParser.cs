using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace InsertModuleInContent.Models
{
    public class WidgetParser {        
        private static readonly string _regexPattern = ConfigurationManager.AppSettings["Regex"];
        private static readonly Regex _regex = new Regex(_regexPattern);

        public static string[] GetWidgetNames(string text) {

            foreach (Match match in _regex.Matches(text)) {
                match.
            }

            return null;
        }
    }
}