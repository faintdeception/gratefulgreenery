using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement;
using Orchard;
using NogginBox.BingMapList.ViewModels;
using NogginBox.BingMapList.Models;
using System.Web.Mvc;

namespace NogginBox.BingMapList.Services
{
	public interface IBingMapListService : IDependency
	{
		String DefaultMapType { get; }
		SelectList CreateMapTypeList(String mapType);
		void UpdateBingMapList(BingLocationPart part, EditBingLocationViewModel model);
		string GetTitle(IContent content, String defaultTitle);
		IEnumerable<BingMapListViewModel> GetBingMapLists();
		IEnumerable<BingLocationPart> GetBingLocations(int bingMapListId);
	}

	public class BingMapListService : IBingMapListService
	{
		public const String MAPICONS_MEDIA_FOLDER = "mapicons";

		private readonly IContentManager _contentManager;

		public BingMapListService(IContentManager contentManager)
		{
			_contentManager = contentManager;
		}

		public SelectList CreateMapTypeList(String mapType = null)
		{
			var mapDict = new Dictionary<String, String>
			{
				{"Auto", "auto"},
				{"Aerial", "a"},
				{"Birdseye", "be"},
				//{"Mercator", "m"}, // Shows blank when I use it
				{"Road", "r"},
				//{"Collins Bart", "cb"} en-gb only
				//{"Ordnance Survey", "os"} en-gb only
			};

			return new SelectList(mapDict, "Value", "Key", mapType);
		}

		public String DefaultMapType
		{
			get { return "r"; }
		}

		public void UpdateBingMapList(BingLocationPart part, EditBingLocationViewModel model)
		{
			if (model.BingMapListId != null)
			{
				part.BingMapList = _contentManager.Get((int)model.BingMapListId);
			}
			else
			{
				part.BingMapList = null;
			}
		}

		public String GetTitle(IContent content, String defaultText)
		{
			if(content == null)return null;

			var title = _contentManager.GetItemMetadata(content).DisplayText;

			return title ?? String.Format(defaultText, content.Id);

			/* Could also look for name field if there is no route
			return bingMapList.ContentItem.Parts
				.SelectMany(p => p.Fields)
				.Where(f => f.Name == "Name")
				.First()
				.Storage.Get<string>(null);*/
		}

		public IEnumerable<BingLocationPart> GetBingLocations(int bingMapListId)
		{
			var result = _contentManager
				.Query<BingLocationPart>()
				.List().ToList()
				.Where(t => t.BingMapList != null && t.BingMapList.Id == bingMapListId);
			return result;
		}

		public IEnumerable<BingMapListViewModel> GetBingMapLists()
		{
			return _contentManager
				.Query<BingMapListPart>()
				.List()
				.Select(bml => new BingMapListViewModel
				{
					Id = bml.Id,
					Name = GetTitle(bml, "Map list {0}")
				});
		}

		public static String ToInvariantCultureString(Decimal number)
		{
			return number.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		}
	}
}
