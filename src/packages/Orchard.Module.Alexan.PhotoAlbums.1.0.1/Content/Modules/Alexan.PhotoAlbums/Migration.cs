using Orchard.ContentManagement.MetaData;
using Orchard.Data.Migration;

namespace Alexan.PhotoAlbums
{
    public class Migration : DataMigrationImpl
    {
        public int Create() 
        {
            //Creating table AlbumPartRecord
            SchemaBuilder.CreateTable("PhotoAlbumPartRecord",
                table => table.ContentPartRecord()
                    .Column<string>("Description", c => c.Unlimited())
                    .Column<string>("MaxWidth")
                    .Column<string>("MaxHeight")
                    .Column<string>("ThumbWidth")
                    .Column<string>("ThumbHeight")
                    .Column<bool>("Resize")
                );

            //Creating table PhotoPartRecord
            SchemaBuilder.CreateTable("PhotoPartRecord",
                table => table.ContentPartRecord()
                    .Column<string>("FileName")
                    .Column<string>("FileExtension")
                );

            ContentDefinitionManager.AlterTypeDefinition(PhotoAlbumTypes.Lightbox,
                cfg => cfg
                    .WithPart("PhotoAlbumPart")
                    .WithPart("PhotoAlbumLightboxPart")
                    .WithPart("CommonPart")
                    .WithPart("RoutePart")
                    .WithPart("MenuPart")
                );

            ContentDefinitionManager.AlterTypeDefinition(PhotoAlbumTypes.Slideshow,
                cfg => cfg
                    .WithPart("PhotoAlbumPart")
                    .WithPart("PhotoAlbumSlideshowPart")
                    .WithPart("CommonPart")
                    .WithPart("RoutePart")
                    .WithPart("MenuPart")
                );

            ContentDefinitionManager.AlterTypeDefinition("Photo",
                cfg => cfg
                    .WithPart("PhotoPart")
                    .WithPart("CommonPart")
                    .WithPart("RoutePart")
                );

            return 1;
        }

        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable("PhotoAlbumPartRecord",
                table => table.AddColumn<int>("AnimationSpeed", column => column.WithDefault(300)));

            SchemaBuilder.CreateTable("PhotoAlbumSlideshowPartRecord",
                table => table.ContentPartRecord()
                              .Column<bool>("AutoPlay")
                              .Column<int>("Interval", column => column.WithDefault(5)));

            return 2;
        }

        public int UpdateFrom2()
        {
            ContentDefinitionManager.AlterTypeDefinition("Photo", cfg => cfg.WithPart("PublishLaterPart"));
            return 3;
        }
    }
}