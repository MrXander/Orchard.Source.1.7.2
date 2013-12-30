using System.Collections.Generic;
using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;

namespace PrettyGallery {
    public class Permissions : IPermissionProvider {
        public static readonly Permission ManagePrettyGalleries = new Permission { Description = "Managing Pretty Galleries", Name = "ManagePrettyGalleries" };

        public virtual Feature Feature { get; set; }

        public IEnumerable<Permission> GetPermissions() {
            return new[] {
                ManagePrettyGalleries,
            };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes() {
            return new[] {
                new PermissionStereotype {
                    Name = "Administrator",
                    Permissions = new[] {ManagePrettyGalleries}
                },
                new PermissionStereotype {
                    Name = "Editor",
                    Permissions = new[] {ManagePrettyGalleries}
                },
                new PermissionStereotype {
                    Name = "Moderator",
                },
                new PermissionStereotype {
                    Name = "Author",
                    Permissions = new[] {ManagePrettyGalleries}
                },
                new PermissionStereotype {
                    Name = "Contributor",
                },
            };
        }

    }
}