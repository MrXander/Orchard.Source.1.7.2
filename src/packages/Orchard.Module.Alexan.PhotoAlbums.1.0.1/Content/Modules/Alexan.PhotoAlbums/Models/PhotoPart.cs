using System.Drawing;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.Core.Routable.Models;

namespace Alexan.PhotoAlbums.Models
{
    public class PhotoPart : ContentPart<PhotoPartRecord>
    {
        public string Name
        {
            get { return this.As<RoutePart>().Title; }
            set { this.As<RoutePart>().Title = value; }
        }

        public string FileName
        {
            get
            {
                return Record.FileName;
            }

            set
            {
                Record.FileName = value;
            }
        }

        public string FileExtension
        {
            get
            {
                return Record.FileExtension;
            }

            set
            {
                Record.FileExtension = value;
            }
        }

        public string FullName
        {
            get { return string.Format("{0}{1}", FileName, FileExtension); }
        }

        public string Path
        {
            get { return AlbumPart.Name; }
        }

        public string FullPath
        {
            get { return System.IO.Path.Combine(Path, FullName); }
        }

        public PhotoAlbumPart AlbumPart
        {
            get { return this.As<ICommonPart>().Container.As<PhotoAlbumPart>(); }
            set { this.As<ICommonPart>().Container = value; }
        }
        
        public Size ThumbSize
        {
            get { return AlbumPart.ThumbSize; }
        }

        public Size Size
        {
            get { return AlbumPart.MaxSize; }
        }
    }
}