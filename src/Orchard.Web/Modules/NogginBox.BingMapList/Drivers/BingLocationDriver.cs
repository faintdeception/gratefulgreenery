using System;
using System.Collections.Generic;
using System.Linq;
using NogginBox.BingMapList.Models;
using NogginBox.BingMapList.Services;
using NogginBox.BingMapList.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Localization;
using Orchard.MediaLibrary.Services;
using Orchard.UI.Notify;

namespace NogginBox.BingMapList.Drivers
{
	public class BingLocationDriver : ContentPartDriver<BingLocationPart>
	{
		private readonly IBingMapListService _mapListService;
		private readonly IMediaLibraryService _mediaLibraryService;
		private readonly IOrchardServices _services;

		public Localizer T { get; set; }

		public BingLocationDriver(IBingMapListService mapListService, IMediaLibraryService mediaService, IOrchardServices services)
		{
			_mapListService = mapListService;
			_mediaLibraryService = mediaService;
			_services = services;
		}

		protected override string Prefix
		{
			get { return "BingLocation"; }
		}


		protected override DriverResult Display(BingLocationPart part, string displayType, dynamic shapeHelper)
		{
			if (!part.IsEnabled) return null;

			return ContentShape("Parts_BingLocation",
								() => shapeHelper.Parts_BingLocation(
									Longitude: part.Longitude,
									Latitude: part.Latitude,
									Width: part.Width,
									Height: part.Height,
									Zoom: part.Zoom,
									MapType: part.MapType,
									MapIcon: part.MapIcon,
									MapIconFolder: GetIconFolder(),
									BingMapList: part.BingMapList,
									BingMapListName: _mapListService.GetTitle(part.BingMapList, "Map list {0}")
									));
		}

		//GET
		protected override DriverResult Editor(BingLocationPart part, dynamic shapeHelper)
		{
			WarnAboutNullBingMapList(part);

			return ContentShape("Parts_BingLocation_Edit",
								() => shapeHelper.EditorTemplate(
									TemplateName: "Parts/BingLocation",
									Model: BuildEditorViewModel(part),
									Prefix: Prefix));
		}

		//POST
		protected override DriverResult Editor(BingLocationPart part, IUpdateModel updater, dynamic shapeHelper)
		{
			// Update main bits of location
			updater.TryUpdateModel(part, Prefix, null, null);

			// Update linked maplist bit of location
			var model = new EditBingLocationViewModel();
			updater.TryUpdateModel(model, Prefix, null, null);
			
			// Gets value from String version (means it also works in French)
			part.Latitude = model.Latitude;
			part.Longitude = model.Longitude;

			if (part.ContentItem.Id != 0)
			{
				_mapListService.UpdateBingMapList(part, model);
			}

			return Editor(part, shapeHelper);
		}

		private EditBingLocationViewModel BuildEditorViewModel(BingLocationPart part)
		{
			// Setting creation defaults - MapType is null on creation
			var mapType = part.MapType ?? _mapListService.DefaultMapType;
			var isEnabled = (part.MapType != null) ? part.IsEnabled : true;

			var editModel = new EditBingLocationViewModel
			{
				LatitudeStr = part.Latitude.toInvariantCultureString(),
				LongitudeStr = part.Longitude.toInvariantCultureString(),
				Width = part.Width,
				Height = (part.Height == 0) ? 300 : part.Height, // Default height
				Zoom = part.Zoom,
				MapType = mapType,
				BingMapLists = _mapListService.GetBingMapLists(),
				MapTypeList = _mapListService.CreateMapTypeList(mapType),
				PossibleMapIcons = GetPossibleMapIcons(),
				MapIcon = part.MapIcon,
				MapIconFolder = GetIconFolder(),
				IsEnabled = isEnabled
			};

			if (part.BingMapList != null)
			{
				editModel.BingMapListId = part.BingMapList.Id;
			}

			return editModel;
		}

		private IEnumerable<String> GetPossibleMapIcons()
		{
			// Get possible icons
			List<String> possibleMapIcons;
			var rootMediaFolders = _mediaLibraryService
				.GetMediaFolders(".")
				.Where(f => f.Name.Equals(BingMapListService.MAPICONS_MEDIA_FOLDER, StringComparison.OrdinalIgnoreCase));
			if (rootMediaFolders.Any())
			{
				possibleMapIcons = new List<string>(
					_mediaLibraryService.GetMediaFiles(BingMapListService.MAPICONS_MEDIA_FOLDER)
					.Select(f => f.Name));
			}
			else
			{
				possibleMapIcons = new List<String>();
			}

			return possibleMapIcons;
		}

		private String GetIconFolder()
		{
			return _mediaLibraryService.GetMediaPublicUrl(BingMapListService.MAPICONS_MEDIA_FOLDER + "/", "");
		}

		private void WarnAboutNullBingMapList(BingLocationPart part)
		{
			// Only do this check if we're in a GET request - stops AJAX requests adding notify messages to TempData
			var method = _services.WorkContext.HttpContext.Request.RequestType;
			if (method == "GET")
			{
				// Check map list
				if (part.Id != 0 && part.BingMapList == null)
				{
					_services.Notifier.Warning(T("This location is not part of a map list. Select a map list if you want it to appear on one."));
				}
			}
		}


		#region Import / Export

		protected override void Exporting(BingLocationPart part, ExportContentContext context)
		{
			context.Element(part.PartDefinition.Name).SetAttributeValue("Latitude", part.Latitude);
			context.Element(part.PartDefinition.Name).SetAttributeValue("Longitude", part.Longitude);
			context.Element(part.PartDefinition.Name).SetAttributeValue("IsEnabled", part.IsEnabled);
			context.Element(part.PartDefinition.Name).SetAttributeValue("Width", part.Width);
			context.Element(part.PartDefinition.Name).SetAttributeValue("Height", part.Height);
			context.Element(part.PartDefinition.Name).SetAttributeValue("Zoom", part.Zoom);
			context.Element(part.PartDefinition.Name).SetAttributeValue("MapType", part.MapType);
			context.Element(part.PartDefinition.Name).SetAttributeValue("MapIcon", part.MapIcon);

			if (part.Record.BingMapList == null)
			{
				return;
			}

			var bingMapListPart = _services.ContentManager.Query<BingMapListPart, BingMapListRecord>()
				.Where(x => x.Id == part.Record.BingMapList.Id)
				.List()
				.FirstOrDefault();

			if (bingMapListPart != null)
			{
				var bingMapListIdentity = _services.ContentManager.GetItemMetadata(bingMapListPart).Identity;
				context.Element(part.PartDefinition.Name).SetAttributeValue("BingMapList", bingMapListIdentity.ToString());
			}
		}
 	 
		protected override void Importing(BingLocationPart part, ImportContentContext context)
		{
			var latitudeString = context.GetAttribute("Latitude", part);
			part.Latitude = latitudeString.TryParseWithDefault(0f, float.TryParse);

			var longitudeString = context.GetAttribute("Longitude", part);
			part.Longitude = longitudeString.TryParseWithDefault(0f, float.TryParse);

			var isEnabledString = context.GetAttribute("IsEnabled", part);
			part.IsEnabled = isEnabledString.TryParseWithDefault(false, bool.TryParse);

			var widthString = context.GetAttribute("Width", part);
			if (!String.IsNullOrWhiteSpace(widthString))
			{
				int width;
				if (int.TryParse(widthString, out width))
				{
					part.Width = width;
				}
			}

			var heightString = context.GetAttribute("Height", part);
			part.Height = heightString.TryParseWithDefault(0, int.TryParse);
			
			var zoomString = context.GetAttribute("Zoom", part);
			part.Zoom = zoomString.TryParseWithDefault(0, int.TryParse);
			
			part.MapType = context.GetAttribute("MapType", part);

			part.MapIcon = context.GetAttribute("MapIcon", part);
		}
 	 
		protected override void Imported(BingLocationPart part, ImportContentContext context)
		{
			// assign the bingMapList only when everything is imported
			var bingMapList = context.Attribute(part.PartDefinition.Name, "BingMapList");
			if (bingMapList != null)
			{
				part.Record.BingMapList = context.GetItemFromSession(bingMapList).As<BingMapListPart>().ContentItem.Record;
			}
		} 

		#endregion

	}
}
