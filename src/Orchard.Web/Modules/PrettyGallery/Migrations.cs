using System.Data;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using Orchard.Indexing;

namespace PrettyGallery
{
    public class Migrations : DataMigrationImpl {

        public int Create() {

            // Creating table PrettyGalleryPartRecord
            SchemaBuilder.CreateTable("PrettyGalleryPartRecord", table => table
                .ContentPartRecord()
                .Column<string>("MediaPath", c=>c.Unlimited())
                .Column<int>("ThumbnailWidth")
                .Column<int>("ThumbnailHeight")
                .Column<string>("PrettyParameters", c => c.Unlimited())
            );

            // Creating table PrettyGalleryPartRecord
            ContentDefinitionManager.AlterPartDefinition("PrettyGalleryPart",
                builder => builder.Attachable());

            ContentDefinitionManager.AlterTypeDefinition("PrettyGallery", cfg => cfg
                .WithPart("CommonPart")
                .WithPart("RoutePart")
                //.WithPart("BodyPart")
                .WithPart("MenuPart")
                .WithPart("PrettyGalleryPart")
                //.WithPart("CommentsPart")
                //.WithPart("TagsPart")
                .Creatable()
                .Indexed());

            return 1;
        }

        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable("PrettyGalleryPartRecord", table => table
                .AddColumn<string>("Description", c => c.Unlimited())
            );

            return 2;
        }

        public int UpdateFrom2()
        {
            SchemaBuilder.AlterTable("PrettyGalleryPartRecord", table => table
                .AddColumn<string>("Layout")
            );

            SchemaBuilder.AlterTable("PrettyGalleryPartRecord", table => table
                .AddColumn<int>("Mode")
            );

            return 3;
        }
    }
}