using System.Web.Mvc;
using Alexan.PhotoAlbums.Models;

namespace Alexan.PhotoAlbums.Extensions
{
    public static class UrlHelperAdminExtensions
    {
        public static string AlbumSummary(this UrlHelper urlHelper)
        {
            return urlHelper.Action("Summary", "PhotoAlbumAdmin", new { area = "Alexan.PhotoAlbums" });
        }

        public static string AlbumItem(this UrlHelper urlHelper, PhotoAlbumPart albumPart)
        {
            return urlHelper.Action("Item", "PhotoAlbumAdmin", new { albumId = albumPart.Id, area = "Alexan.PhotoAlbums" });
        }

        public static string AlbumItem(this UrlHelper urlHelper, int albumId)
        {
            return urlHelper.Action("Item", "PhotoAlbumAdmin", new { albumId = albumId, area = "Alexan.PhotoAlbums" });
        }

        public static string AlbumEdit(this UrlHelper urlHelper, PhotoAlbumPart albumPart)
        {
            return urlHelper.Action("Edit", "PhotoAlbumAdmin", new { albumId = albumPart.Id, area = "Alexan.PhotoAlbums" });
        }

        public static string AlbumCreate(this UrlHelper urlHelper, string albumType)
        {
            return urlHelper.Action("Create", "PhotoAlbumAdmin", new { type = albumType, area = "Alexan.PhotoAlbums" });
        }

        public static string AlbumRemove(this UrlHelper urlHelper, PhotoAlbumPart albumPart, bool deleteFolder)
        {
            return urlHelper.Action("Remove", "PhotoAlbumAdmin", new { albumId = albumPart.Id, deleteFolder = deleteFolder, area = "Alexan.PhotoAlbums" });
        }
        //---------------
        public static string PhotoAdd(this UrlHelper urlHelper, PhotoAlbumPart albumPart)
        {
            return urlHelper.Action("Add", "PhotoAdmin", new { albumId = albumPart.Id, area = "Alexan.PhotoAlbums" });
        }

        public static string PhotoUpload(this UrlHelper urlHelper, PhotoAlbumPart albumPart)
        {
            return urlHelper.Action("UploadFile", "PhotoAdmin", new { albumId = albumPart.Id, area = "Alexan.PhotoAlbums" });
        }

        public static string PhotoEdit(this UrlHelper urlHelper, PhotoPart photoPart)
        {
            return urlHelper.Action("Edit", "PhotoAdmin", new { imageId = photoPart.Id, area = "Alexan.PhotoAlbums" });
        }

        //public static string PhotoItem(this UrlHelper urlHelper, PhotoPart photoPart)
        //{
        //    return urlHelper.Action("Item", "PhotoAdmin", new { imageId = photoPart.Id, area = "Alexan.PhotoAlbums" });
        //}

        public static string PhotoRemove(this UrlHelper urlHelper, int photoId, bool deleteFile)
        {
            return urlHelper.Action("Remove", "PhotoAdmin", new { photoId = photoId, deleteFile = deleteFile, area = "Alexan.PhotoAlbums" });
        }
    }

    public static class UrlHelperhandlingExtensions
    {
        public static string Thumb(this UrlHelper urlHelper, int photoId, int width, int height)
        {
            return urlHelper.Action("Thumb", "ImageHandling", new
                                                              {
                                                                  photoId = photoId, 
                                                                  width = width, 
                                                                  height = height, 
                                                                  area = "Alexan.PhotoAlbums"
                                                              });
        }
    }
}