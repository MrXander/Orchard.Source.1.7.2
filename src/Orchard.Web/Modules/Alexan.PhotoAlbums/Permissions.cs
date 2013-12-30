using System.Collections.Generic;
using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;

namespace Alexan.PhotoAlbums
{
    public class Permissions : IPermissionProvider
    {
        public static readonly Permission ManageAlbums = new Permission { Description = "Manage albums", Name = "ManageAlbums" };

        public static readonly Permission PublishPhoto = new Permission { Description = "Publish or unpublish photo for others", Name = "PublishPhoto", ImpliedBy = new[] { ManageAlbums } };
        public static readonly Permission PublishOwnPhoto = new Permission { Description = "Publish or unpublish own photo", Name = "PublishOwnPhoto", ImpliedBy = new[] { PublishPhoto } };
        public static readonly Permission EditPhoto = new Permission { Description = "Edit any photo", Name = "EditPhoto", ImpliedBy = new[] { PublishPhoto } };
        public static readonly Permission EditOwnPhoto = new Permission { Description = "Edit own photo", Name = "EditOwnPhoto", ImpliedBy = new[] { EditPhoto, PublishOwnPhoto } };
        public static readonly Permission DeletePhoto = new Permission { Description = "Delete photo for others", Name = "DeletePhoto", ImpliedBy = new[] { ManageAlbums } };
        public static readonly Permission DeleteOwnPhoto = new Permission { Description = "Delete own photo", Name = "DeleteOwnPhoto", ImpliedBy = new[] { DeletePhoto } };

        public static readonly Permission MetaListPhoto = new Permission { ImpliedBy = new[] { EditPhoto, PublishPhoto, DeletePhoto } };
        public static readonly Permission MetaListOwnPhoto = new Permission { ImpliedBy = new[] { EditOwnPhoto, PublishOwnPhoto, DeleteOwnPhoto } };

        public virtual Feature Feature { get; set; }

        public IEnumerable<Permission> GetPermissions()
        {
            return new[] {
                ManageAlbums,
                EditOwnPhoto,
                EditPhoto,
                PublishOwnPhoto,
                PublishPhoto,
                DeleteOwnPhoto,
                DeletePhoto,
            };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return new[] {
                new PermissionStereotype {
                    Name = "Administrator",
                    Permissions = new[] {ManageAlbums}
                },
                new PermissionStereotype {
                    Name = "Editor",
                    Permissions = new[] {PublishPhoto,EditPhoto,DeletePhoto}
                },
                new PermissionStereotype {
                    Name = "Moderator",
                },
                new PermissionStereotype {
                    Name = "Author",
                    Permissions = new[] {PublishOwnPhoto,EditOwnPhoto,DeleteOwnPhoto}
                },
                new PermissionStereotype {
                    Name = "Contributor",
                    Permissions = new[] {EditOwnPhoto}
                },
            };
        }

    }
}


