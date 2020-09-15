using System;
using System.Collections.Generic;
using System.Linq;
using NogginBox.BingMapList.Models;
using NogginBox.BingMapList.Services;
using NogginBox.BingMapList.ViewModels;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Core.Title.Models;
using Orchard.DisplayManagement;
using Orchard.MediaLibrary.Services;

namespace NogginBox.BingMapList.Drivers
{
	public class BingMapListDriver : ContentPartDriver<BingMapListPart>
	{
		private readonly IBingMapListService _mapListService;
		private readonly IContentManager _content;
		private readonly IMediaLibraryService _mediaLibraryService;
		private readonly dynamic _shape;

		public BingMapListDriver(IBingMapListService mapListService, IContentManager content, IMediaLibraryService mediaService, IShapeFactory shapeFactory)
		{
			_mapListService = mapListService;
			_content = content;
			_mediaLibraryService = mediaService;
			_shape = shapeFactory;
		}

		protected override DriverResult Display(BingMapListPart part, string displayType, dynamic shapeHelper)
		{
			// Create list of content items
			var locationContent = _shape.List();
			var locationContentItems = part.BingLocations.Select(t => t.As<ContentItem>())
				.OrderBy(t => t.As<TitlePart>().Title); // Todo: Test this orderby clause is okay if the part has no TitlePart
			locationContent.AddRange(locationContentItems.Select(b => _content.BuildDisplay(b, "Summary")));

			return ContentShape("Parts_BingMapList",
								() => shapeHelper.Parts_BingMapList(
									CenterLongitude: part.CenterLongitude,
									CenterLatitude: part.CenterLatitude,
									Width: part.Width,
									Height: part.Height,
									Zoom: part.Zoom,
									MapType: part.MapType,
									MapIconFolder: GetIconFolder(),
									BingLocations: part.BingLocations,
									LocationsCount: part.BingLocations.Count(),
									LocationContentItems: locationContent,
									Titles: GetTitles(part)));
		}

		//GET
		protected override DriverResult Editor(BingMapListPart part, dynamic shapeHelper)
		{
			return ContentShape("Parts_BingMapList_Edit",
								() => shapeHelper.EditorTemplate(
												TemplateName: "Parts/BingMapList",
												Model: BuildEditorViewModel(part),
												Prefix: Prefix));
		}
		//POST
		protected override DriverResult Editor(BingMapListPart part, IUpdateModel updater, dynamic shapeHelper)
		{
			updater.TryUpdateModel(part, Prefix, null, null);

			// Get Center Lat, long from model (required to work in cultures that use , in decimal numbers)
			var model = new EditBingMapListViewModel();
			updater.TryUpdateModel(model, Prefix, new [] { "CenterLatitudeStr", "CenterLongitudeStr" }, null);
			part.CenterLatitude = model.CenterLatitude;
			part.CenterLongitude = model.CenterLongitude;

			return Editor(part, shapeHelper);
		}

		private EditBingMapListViewModel BuildEditorViewModel(BingMapListPart part)
		{
			var mapType = part.MapType ?? _mapListService.DefaultMapType;

			if (part.BingLocations == null) part.BingLocations = new List<BingLocationPart>();

			var editModel = new EditBingMapListViewModel
			{
				CenterLatitudeStr = part.CenterLatitude.toInvariantCultureString(),
				CenterLongitudeStr = part.CenterLongitude.toInvariantCultureString(),
				Width = part.Width,
				Height = (part.Height == 0) ? 300 : part.Height, // Default height
				Zoom = part.Zoom,
				MapType = mapType,
				MapTypeList = _mapListService.CreateMapTypeList(mapType),
				BingLocations = part.BingLocations,
				Titles = GetTitles(part),
				MapIconFolder = GetIconFolder()
			};

			return editModel;
		}

		private String GetIconFolder()
		{
			return _mediaLibraryService.GetMediaPublicUrl(BingMapListService.MAPICONS_MEDIA_FOLDER + "/", "");
		}

		private List<String> GetTitles(BingMapListPart part)
		{
			// Get titles, would be nice to include these in the location object
			var titles = new List<String>();

			if(part.BingLocations != null)
			{
				foreach (var location in part.BingLocations)
				{
					titles.Add(_mapListService.GetTitle(location, "Map {0}"));
				}
			}
			return titles;
		}


		#region Import / Export

		protected override void Exporting(BingMapListPart part, ExportContentContext context)
		{
			context.Element(part.PartDefinition.Name).SetAttributeValue("CenterLatitude", part.CenterLatitude);
			context.Element(part.PartDefinition.Name).SetAttributeValue("CenterLongitude", part.CenterLongitude);
			context.Element(part.PartDefinition.Name).SetAttributeValue("Width", part.Width);
			context.Element(part.PartDefinition.Name).SetAttributeValue("Height", part.Height);
			context.Element(part.PartDefinition.Name).SetAttributeValue("Zoom", part.Zoom);
			context.Element(part.PartDefinition.Name).SetAttributeValue("MapType", part.MapType);
		}
	 
		protected override void Importing(BingMapListPart part, ImportContentContext context)
		{
			var centerLatitudeString = context.GetAttribute("CenterLatitude", part);
			part.CenterLatitude = centerLatitudeString.TryParseWithDefault(0f, float.TryParse);

			var centerLongitudeString = context.GetAttribute("CenterLongitude", part);
			part.CenterLongitude = centerLongitudeString.TryParseWithDefault(0f, float.TryParse);
		
			var widthString = context.GetAttribute("Width", part);
			if (!String.IsNullOrWhiteSpace(widthString))
			{
				int width;
				if (int.TryParse(widthString, out width)) {
					part.Width = width;
				}
			}

			var heightString = context.GetAttribute("Height", part);
			part.Height = heightString.TryParseWithDefault(0, int.TryParse);

			var zoomString = context.GetAttribute("Zoom", part);
			part.Zoom = zoomString.TryParseWithDefault(0, int.TryParse);
		
			part.MapType = context.GetAttribute("MapType", part);
		} 

		#endregion
	}
}
