using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Hosting;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using Orchard;
using Orchard.ContentManagement;
using Orchard.FileSystems.Media;
using Orchard.Localization;
using Orchard.Media.Models;
using Orchard.Media.Services;
using PrettyGallery.Models;
using PrettyGallery.ViewModels;

namespace PrettyGallery.Services
{
    public interface IPrettyGalleryService : IDependency
    {
        void UpdatePrettyGalleryForContentItem(ContentItem item, EditPrettyGalleryViewModel model);

        IEnumerable<PrettyMediaFile> GetMediaFiles(string mediaPath, Status statusFilter, int thumbnailWidth, int thumbnailHeight);
        IEnumerable<MediaFolder> GetFolderTree(string path);
        IEnumerable<string> GetLayouts();
        IEnumerable<string> GetPrettyThemes();
        IEnumerable<Mode> GetModes();
        string SerializePrettyParams(PrettyParams pp);
        PrettyParams DeserializePrettyParams(string prettyParameters);

        PrettyMediaFile GetRandomMediaFile(string mediaPath, Status statusFilter, int thumbnailWidth, int thumbnailHeight);

        void UpdateFileTitle(string mediaPath, string fileName, string title);
        void UpdateFileDescription(string mediaPath, string fileName, string description);
        string GetIdFromName(string name);
        string GetNameFromId(string id);
    }

    public class PrettyGalleryService : IPrettyGalleryService
    {
        private readonly IMediaService _mediaService;
        private readonly IStorageProvider _storageProvider;
        private readonly IOrchardServices _orchardServices;

        private readonly List<string> _imageExtensions = new List<string> { ".jpg", ".jpeg", ".png", ".bmp" };
        private readonly List<string> _videoExtensions = new List<string> { ".swf", ".mov", ".avi", ".mp4" };

        private const string ThumbnailsFolder = "Thumbnails";
        private const string MediaInfoFileName = "Media.info";
        private const string ElementMedia = "Media";
        private const string ElementFile = "File";
        private const string AttributeName = "Name";
        private const string AttributeTitle = "Title";
        private const string AttributeDescription = "Description";
        private const string AttributeLastUpdated = "LastUpdated";
        private const string AttributeStatus = "Status";
        private const string AttributeSize = "Size";
        private const string AttributeType = "Type";
        private const string AttributeUser = "User";
        private const string AttributeWidth = "Width";
        private const string AttributeHeight = "Height";

        public PrettyGalleryService(IMediaService mediaService, IStorageProvider storageProvider, IOrchardServices orchardServices)
        {
            _mediaService = mediaService;
            _storageProvider = storageProvider;
            _orchardServices = orchardServices;

            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public void UpdatePrettyGalleryForContentItem(
            ContentItem item,
            EditPrettyGalleryViewModel model)
        {
            var prettyGalleryPart = item.As<PrettyGalleryPart>();

            prettyGalleryPart.Description = model.Description;
            prettyGalleryPart.MediaPath = model.MediaPath;
            prettyGalleryPart.PrettyParameters = SerializePrettyParams(model.PrettyParams);
            prettyGalleryPart.ThumbnailHeight = model.ThumbnailHeight;
            prettyGalleryPart.ThumbnailWidth = model.ThumbnailWidth;
            prettyGalleryPart.Layout = model.Layout;
            prettyGalleryPart.Mode = (Mode)model.Mode;
        }

        public string SerializePrettyParams(PrettyParams pp) {
            try
            {
                var jss = new JavaScriptSerializer();
                var prettyParameters = jss.Serialize(pp);
                //jss.Serialize(pp);
                return prettyParameters;
            }
            catch
            {
                return "{}"; // empty JSON object
            }
        }

        public PrettyParams DeserializePrettyParams(string prettyParameters)
        {
            try
            {
                var jss = new JavaScriptSerializer();
                var pp = jss.Deserialize(prettyParameters, typeof(PrettyParams)) as PrettyParams;
                return pp;
            }
            catch {
                return new PrettyParams {theme = "dark_rounded", overlay_gallery = false};
            }
        }

        public IEnumerable<PrettyMediaFile> GetMediaFiles(string mediaPath, Status statusFilter, int thumbnailWidth, int thumbnailHeight)
        {
            const bool autoPublish = true; // TODO: make parameter

            List<PrettyMediaFile> prettyMediaFiles = null;

            try
            {
                if (String.IsNullOrEmpty(mediaPath))
                    throw new ArgumentException("mediaPath should not be null or empty", "mediaPath");

                // Re-create media folder if it was deleted
                try { _storageProvider.CreateFolder(mediaPath); }
                catch {
                    ;
                }

                // Try to get XML
                XDocument xdoc = GetMediaInfo(mediaPath);
                var root = GetMediaRoot(xdoc);

                // Get list of MediaFiles from file system
                List<MediaFile> mediaFiles = _mediaService.GetMediaFiles(mediaPath)
                    .Where(f => _imageExtensions.Contains(f.Type.ToLower())).ToList();

                // Get list of PrettyMediaFiles from XML, removing deleted files
                DateTime lastUpdated;
                Status status;
                long size;
                prettyMediaFiles = root.Elements(ElementFile)
                    .Where(x => !String.IsNullOrEmpty(x.AttributeValue(AttributeName))
                                && mediaFiles.Exists(mf => string.Equals(mf.Name, x.AttributeValue(AttributeName), StringComparison.OrdinalIgnoreCase)))
                    .Select(x => new PrettyMediaFile {
                        Name = x.AttributeValue(AttributeName),
                        Title = x.AttributeValue(AttributeTitle),
                        Description = x.AttributeValue(AttributeDescription),
                        LastUpdated = DateTime.TryParse(x.AttributeValue(AttributeLastUpdated), out lastUpdated)
                                          ? lastUpdated
                                          : DateTime.MinValue,
                        Size = long.TryParse(x.AttributeValue(AttributeSize), out size)
                                          ? size
                                          : 0,
                        Status = Enum.TryParse(x.AttributeValue(AttributeStatus), true, out status)
                                          ? status
                                          : autoPublish ? Status.Published : Status.Pending
                    })
                    .ToList();

                // Update PrettyMediaFiles attributes from current MediaFiles
                prettyMediaFiles.ForEach(
                    pmf => {
                        var mf = mediaFiles.First(f => f.Name.ToLower() == pmf.Name.ToLower());
                        pmf.Changed = (pmf.LastUpdated != mf.LastUpdated || pmf.Size != mf.Size);
                        if (pmf.Changed && pmf.Status == Status.Published && !autoPublish)
                            pmf.Status = Status.Pending;
                        pmf.Name = mf.Name;
                        pmf.LastUpdated = mf.LastUpdated;
                        pmf.FolderName = mf.FolderName;
                        pmf.Size = mf.Size;
                        pmf.Type = mf.Type;
                        pmf.User = mf.User;
                    });

                // Append new PrettyMediaFiles from MediaFiles
                prettyMediaFiles.AddRange(
                    mediaFiles
                        .Where(mf => !prettyMediaFiles.Exists(pmf => pmf.Name.ToLower() == mf.Name.ToLower()))
                        .Select(mf => new PrettyMediaFile {
                            Name = mf.Name,
                            Description = "", // TODO: Take from XMP/EXIF 
                            FolderName = mf.FolderName,
                            LastUpdated = mf.LastUpdated,
                            Size = mf.Size,
                            Title = Path.GetFileNameWithoutExtension(mf.Name), // TODO: Take from XMP/EXIF
                            Type = mf.Type,
                            User = mf.User,
                            Changed = true,
                            Status = autoPublish ? Status.Published : Status.Pending,
                        }).ToList()
                    );

                // Create thumbnails dir if required
                try {
                    //TODO: Check if thumbnails dir exists
                    _storageProvider.TryCreateFolder(GetThumbnailsFolderPath(mediaPath, thumbnailWidth, thumbnailHeight));
                }
                catch {
                    ; // ignore errors - exception means directory already exists
                }

                // Generate new XDocument
                root.RemoveNodes();
                prettyMediaFiles.ForEach(
                    // Add file node to XDoc root
                    pmf => root.Add(
                        new XElement(ElementFile,
                                     new XAttribute(AttributeName, pmf.Name),
                                     new XAttribute(AttributeTitle, pmf.Title),
                                     new XAttribute(AttributeDescription, pmf.Description),
                                     new XAttribute(AttributeLastUpdated, pmf.LastUpdated),
                                     new XAttribute(AttributeStatus, pmf.Status.ToString()),
                                     new XAttribute(AttributeSize, pmf.Size)
                            //new XAttribute(AttributeType, pmf.Type),
                            //new XAttribute(AttributeUser, pmf.User)
                            ))
                    );

                // Save XDocument
                SaveMediaFilesInfo(xdoc, mediaPath);

                // Filter by status
                if (statusFilter != Status.Any) {
                    prettyMediaFiles = prettyMediaFiles.Where(pmf => pmf.Status == statusFilter).ToList();
                }

                // set id, urls & generate thumbnails
                prettyMediaFiles.ForEach(
                    pmf =>
                    {
                        // Fill media & thumbnail attributes for file
                        pmf.Id = GetIdFromName(pmf.Name);
                        pmf.Url = _mediaService.GetPublicUrl(_storageProvider.Combine(mediaPath, pmf.Name));
                        SetThumbnail(pmf, mediaPath, thumbnailWidth, thumbnailHeight);
                    }
                    );
            }
            catch (Exception ex) {
                ;
            }

            return prettyMediaFiles ?? new List<PrettyMediaFile>();
        }

        public string GetIdFromName(string name)
        {
            Byte[] stringBytes = Encoding.Unicode.GetBytes(name);
            var sbBytes = new StringBuilder(stringBytes.Length * 2);
            foreach (byte b in stringBytes)
            {
                sbBytes.AppendFormat("{0:X2}", b);
            }
            return sbBytes.ToString();
        }

        public string GetNameFromId(string id)
        {
            int numberChars = id.Length;
            var bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(id.Substring(i, 2), 16);
            }
            return Encoding.Unicode.GetString(bytes);
        }

        private string GetThumbnailsFolderPath(string mediaPath, int thumbnailWidth, int thumbnailHeight)
        {
            return _storageProvider.Combine(_storageProvider.Combine(mediaPath, ThumbnailsFolder), String.Format("{0}x{1}", thumbnailWidth, thumbnailHeight));
        }

        private void SetThumbnail(PrettyMediaFile pmf, string mediaPath, int thumbnailWidth, int thumbnailHeight)
        {
            Stream fileStream = null;
            Stream thumbnailStream = null;
            try {
                var thumbnailFolderPath = GetThumbnailsFolderPath(mediaPath, thumbnailWidth, thumbnailHeight);

                var thumbnailPath = _storageProvider.Combine(thumbnailFolderPath, Path.GetFileNameWithoutExtension(pmf.Name) + ".jpg");
                pmf.ThumbnailUrl = _mediaService.GetPublicUrl(thumbnailPath);

                IStorageFile thumbnailFile;
                bool regenerateThumbnail = true;
                try {
                    thumbnailFile = _storageProvider.GetFile(thumbnailPath);
                    regenerateThumbnail = (pmf.Changed || thumbnailFile.GetLastUpdated() < pmf.LastUpdated || thumbnailFile.GetSize() == 0);
                }
                catch (Exception ex) {
                    try {
                        thumbnailFile = _storageProvider.CreateFile(thumbnailPath);
                    }
                    catch (Exception ex2) {
                        throw ex2;
                    }
                }

                if (regenerateThumbnail)
                {
                    // Regenerate thumbnail
                    var filePath = _storageProvider.Combine(mediaPath, pmf.Name);
                    var mediaFile = _storageProvider.GetFile(filePath);
                    fileStream = mediaFile.OpenRead();
                    thumbnailStream = thumbnailFile.OpenWrite();

                    var imgedit = new ImageService(fileStream);
                    Image resultimage = imgedit.Resize(thumbnailWidth, thumbnailHeight);

                    resultimage.Save(thumbnailStream, ImageFormat.Jpeg);
                }
            }
            catch (Exception ex)
            {
                pmf.ThumbnailUrl = HostingEnvironment.MapPath("~/Modules/PrettyGallery/images/noimage.jpg");
            }
            finally
            {
                if (fileStream != null)
                    fileStream.Close();
                if (thumbnailStream != null)
                    thumbnailStream.Close();
            }
        }

        private void SaveMediaFilesInfo(XDocument xdoc, string mediaPath) {
            IStorageFile mediaInfoFile = null;
            try {
                mediaInfoFile = _storageProvider.GetFile(_storageProvider.Combine(mediaPath, MediaInfoFileName));
            }
            catch (Exception ex) {
                try {
                    mediaInfoFile = _storageProvider.CreateFile(_storageProvider.Combine(mediaPath, MediaInfoFileName));
                }
                catch (Exception ex2) {
                    return;
                }
            }

            if (mediaInfoFile != null) {
                Stream fileStream = null;
                try {
                    fileStream = mediaInfoFile.OpenWrite();
                    fileStream.SetLength(0);
                    xdoc.Save(fileStream);
                    fileStream.Flush();
                }
                catch (Exception ex) {
                    return;
                }
                finally {
                    if (fileStream != null)
                        fileStream.Close();
                }
            }
        }

        private XDocument GetMediaInfo(string mediaPath) {
            XDocument xdoc = null;
            Stream fileStream = null;
            try {
                IStorageFile mediaInfoFile = _storageProvider.GetFile(_storageProvider.Combine(mediaPath, MediaInfoFileName));
                fileStream = mediaInfoFile.OpenRead();
                xdoc = XDocument.Load(fileStream);
            }
            catch (Exception ex) {
                xdoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));
            }
            finally {
                if (fileStream != null)
                    fileStream.Close();
            }
            return xdoc;
        }

        public IEnumerable<MediaFolder> GetFolderTree(string path)
        {
            var folderTree = new List<MediaFolder>();
            foreach (var mediaFolder in _mediaService.GetMediaFolders(path))
            {
                mediaFolder.Name = new string('.', mediaFolder.MediaPath.ToCharArray().AsEnumerable().Count(c => c == '/' || c == '\\') * 2) + mediaFolder.Name;
                folderTree.Add(mediaFolder);
                folderTree.AddRange(GetFolderTree(mediaFolder.MediaPath));
            }
            return folderTree;
        }

        public IEnumerable<string> GetLayouts()
        {
            var layouts = new List<string>();
            layouts.Add("icons_no_titles");
            layouts.Add("icons_with_titles");
            layouts.Add("tiles_with_descriptions");
            return layouts;
        }

        public IEnumerable<string> GetPrettyThemes()
        {
            var themes = new List<string>();
            themes.Add("dark_rounded");
            themes.Add("dark_square");
            themes.Add("default");
            themes.Add("facebook");
            themes.Add("light_rounded");
            themes.Add("light_square");
            return themes;
        }

        public IEnumerable<Mode> GetModes()
        {
            var modes = new List<Mode>();
            modes.Add(Mode.Slideshow);
            modes.Add(Mode.Zoom);
            return modes;
        }

        public PrettyMediaFile GetRandomMediaFile(string mediaPath, Status statusFilter, int thumbnailWidth, int thumbnailHeight)
        {
            throw new NotImplementedException();
        }

        public void UpdateFileTitle(string mediaPath, string fileName, string title)
        {
            UpdateFileAttribute(mediaPath, fileName, AttributeTitle, title);
        }

        public void UpdateFileDescription(string mediaPath, string fileName, string description)
        {
            UpdateFileAttribute(mediaPath, fileName, AttributeDescription, description);
        }

        private void UpdateFileAttribute(string mediaPath, string fileName, string attribute, object value)
        {
            try
            {
                XDocument xdoc = GetMediaInfo(mediaPath);
                xdoc.Root
                    .Elements(ElementFile)
                    .Where(f => f.AttributeValue(AttributeName).ToLower() == fileName.ToLower())
                    .FirstOrDefault()
                    .Attribute(attribute)
                    .SetValue(value);

                SaveMediaFilesInfo(xdoc, mediaPath);
            }
            catch (Exception ex)
            {
            }
        }

        public static XElement GetMediaRoot(XDocument xdoc)
        {
            if (xdoc.Root == null || xdoc.Root.Name.ToString() != ElementMedia)
            {
                xdoc.ReplaceNodes(new XElement(ElementMedia));
            }
            return xdoc.Root;
        }
    }

    public static class XExtensions
    {
        public static string AttributeValue(this XElement xElement, string attributeName)
        {
            XAttribute attribute = xElement.Attribute(attributeName);
            return attribute != null ? attribute.Value : string.Empty;
        }
    }
}
