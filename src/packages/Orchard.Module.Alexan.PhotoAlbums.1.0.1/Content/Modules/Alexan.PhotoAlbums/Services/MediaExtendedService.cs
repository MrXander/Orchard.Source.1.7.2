using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Orchard;
using Orchard.FileSystems.Media;
using Orchard.Media.Services;

namespace Alexan.PhotoAlbums.Services
{
    public interface IMediaExtendedService : IMediaService {
        bool IsImage(HttpPostedFileBase file);

        string UploadMediaFile(string folderPath, string fileName, HttpPostedFileBase postedFile);
        string GetUniqueFilename(string folderPath, string fileName);
    }

    public class MediaExtendedService : MediaService , IMediaExtendedService {
        public MediaExtendedService(IStorageProvider storageProvider, IOrchardServices orchardServices) : base(storageProvider, orchardServices) {}

        //ToDo move to config
        private readonly string[] allowedMIME = { "image/gif", "image/jpeg", "image/pjpeg"/*for IE*/, "image/png", "image/x-png"};

        public bool IsImage(HttpPostedFileBase file) {
            return allowedMIME.Contains(file.ContentType);
        }

        public string UploadMediaFile(string folderPath, string fileName, HttpPostedFileBase postedFile)
        {
            var postedFileLength = postedFile.ContentLength;
            var postedFileStream = postedFile.InputStream;
            var postedFileData = new byte[postedFileLength];
            postedFileStream.Read(postedFileData, 0, postedFileLength);

            return UploadMediaFile(folderPath, fileName, postedFileData, false);
        }

        public string GetUniqueFilename(string folderPath, string fileName)
        {
            HashSet<string> existingFiles = new HashSet<string>(GetMediaFiles(folderPath).Select(f => f.Name));

            string name = Path.GetFileNameWithoutExtension(fileName);
            string extension = Path.GetExtension(fileName);

            if (!existingFiles.Contains(fileName))
            {
                return fileName;
            }

            int i = 1;
            bool exists;
            string newName;
            do
            {
                newName = string.Format("{0}-{1}{2}", name, i, extension);
                exists = existingFiles.Contains(newName);
                i++;
            } while (exists);

            return newName;
        }
    }
}