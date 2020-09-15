using System.Data;
using NogginBox.BingMapList.Models;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using Orchard.ContentManagement.MetaData;

namespace NogginBox.BingMapList.DataMigrations // Leave namespace like this - needs to stay constant with v1 so Orchard knows how to update
{
    public class DataMigration : DataMigrationImpl
    {
        public int Create()
        {
            // BingLocation
            SchemaBuilder.CreateTable("BingLocationRecord", table => table
                .ContentPartRecord()
                .Column("Latitude", DbType.Single, column => column.NotNull())
                .Column("Longitude", DbType.Single, column => column.NotNull())
                .Column("IsEnabled", DbType.Boolean, column => column.WithDefault(true))
                .Column("Width", DbType.Int32, column => column.Nullable().WithDefault(400))
                .Column("Height", DbType.Int32, column => column.NotNull().WithDefault(400))
                .Column("Zoom", DbType.Int32, column => column.NotNull())
                .Column("MapType", DbType.String)
                .Column("MapIcon", DbType.String)
                .Column("BingMapList_Id", DbType.Int32)
            );
            ContentDefinitionManager.AlterPartDefinition(typeof(BingLocationPart).Name,
                                                         cfg => cfg.Attachable());

            // BingMapList
            SchemaBuilder.CreateTable("BingMapListRecord", table => table
                .ContentPartRecord()
                .Column("CenterLatitude", DbType.Single, column => column.NotNull())
                .Column("CenterLongitude", DbType.Single, column => column.NotNull())
                .Column("Width", DbType.Int32, column => column.Nullable().WithDefault(400))
                .Column("Height", DbType.Int32, column => column.NotNull().WithDefault(400))
                .Column("Zoom", DbType.Int32, column => column.NotNull())
                .Column("MapType", DbType.String)
            );
            ContentDefinitionManager.AlterPartDefinition(typeof(BingMapListPart).Name,
                                                         cfg => cfg.Attachable());

            // Content items

            // Content item: Bing Place
            ContentDefinitionManager.AlterTypeDefinition("BingPlace", cfg => cfg
                .WithPart("BingLocationPart")
                .WithPart("CommonPart")
                .WithPart("TitlePart")
                .WithPart("AutoroutePart", builder => builder
                        .WithSetting("AutorouteSettings.AllowCustomPattern", "true")
                        .WithSetting("AutorouteSettings.AutomaticAdjustmentOnEdit", "false")
                        .WithSetting("AutorouteSettings.PatternDefinitions", "[{Name:'Title', Pattern: '{Content.Slug}', Description: 'my-place'}]")
                        .WithSetting("AutorouteSettings.DefaultPatternIndex", "0"))
                .WithPart("BodyPart")
                .WithPart("CommentsPart")
                .WithPart("TagsPart")
                .Creatable()
            );

            // Content item: Bing Area
            ContentDefinitionManager.AlterTypeDefinition("BingArea", cfg => cfg
                .WithPart("BingMapListPart")
                .WithPart("CommonPart")
                .WithPart("TitlePart")
                .WithPart("AutoroutePart", builder => builder
                        .WithSetting("AutorouteSettings.AllowCustomPattern", "true")
                        .WithSetting("AutorouteSettings.AutomaticAdjustmentOnEdit", "false")
                        .WithSetting("AutorouteSettings.PatternDefinitions", "[{Name:'Title', Pattern: '{Content.Slug}', Description: 'my-area'}]")
                        .WithSetting("AutorouteSettings.DefaultPatternIndex", "0"))
                .WithPart("BodyPart")
                .Creatable()
            );

            // Widgets
            /*
            // Widget: Bing Place Widget
            ContentDefinitionManager.AlterTypeDefinition("BingLocationWidget", cfg => cfg
                .WithPart("BingLocationPart")
                .WithPart("WidgetPart")
                .WithPart("CommonPart")
                .WithSetting("Stereotype", "Widget")
            );

            // Widget: Bing Area Widget
            ContentDefinitionManager.AlterTypeDefinition("BingMapListWidget", cfg => cfg
                .WithPart("BingMapListPart")
                .WithPart("WidgetPart")
                .WithPart("CommonPart")
                .WithSetting("Stereotype", "Widget")
            );
            */
            return 2;
        }

        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable("BingLocationRecord", table => table
                .AddColumn("IsEnabled", DbType.Boolean, column => column.WithDefault(true))
            );

            return 2;
        }
    }
}