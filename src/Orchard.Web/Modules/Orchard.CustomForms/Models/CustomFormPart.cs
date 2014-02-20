using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;

namespace Orchard.CustomForms.Models {
    public class CustomFormPart : ContentPart<CustomFormPartRecord> {
        [Required]
        public string ContentType {
            get { return Record.ContentType; }
            set { Record.ContentType = value; }
        }

        public bool SaveContentItem {
            get { return Record.SaveContentItem; }
            set { Record.SaveContentItem = value; }
        }

        public bool CustomMessage {
            get { return Record.CustomMessage; }
            set { Record.CustomMessage = value; }
        }

        public string Message {
            get { return Record.Message; }
            set { Record.Message = value; }
        }

        public bool Redirect {
            get { return Record.Redirect; }
            set { Record.Redirect = value; }
        }

        public string RedirectUrl {
            get { return Record.RedirectUrl; }
            set { Record.RedirectUrl = value; }
        }

        public string Title {
            get { return this.As<ITitleAspect>().Title;  }
        }

        public bool IsAjaxForm {
            get { return Record.IsAjaxForm; }
            set { Record.IsAjaxForm = value; }
        }
    }
}