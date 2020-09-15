using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using NogginBox.BingMapList.Models;
using NogginBox.BingMapList.Services;

namespace NogginBox.BingMapList.Handlers
{
	public class BingMapListHandler : ContentHandler
	{
		private readonly IBingMapListService _mapService;

		public BingMapListHandler(IRepository<BingMapListRecord> repository, IBingMapListService mapService)
		{
			Filters.Add(StorageFilter.For(repository));
			_mapService = mapService;

			//OnInitializing<BingMapListPart>(propertySetHandlers);
			OnLoaded<BingMapListPart>(lazyLoadHandlers);
		}

		void lazyLoadHandlers(LoadContentContext context, BingMapListPart part)
		{
			// add handlers that will load content just-in-time
			part.BingLocationsField.Loader(() =>
				_mapService.GetBingLocations(part.Id));
		}

		/*static void propertySetHandlers(InitializingContentContext context, BingMapListPart part)
		{
			// add handlers that will update records when part properties are set
			part.BingMapListField.Setter(sponsor =>
			{
				part.Record.BingMapList = sponsor == null
					? null
					: sponsor.ContentItem.Record;
				return sponsor;
			});

			// Force call to setter if we had already set a value
			if (part.BingMapListField.Value != null)
				part.BingMapListField.Value = part.BingMapListField.Value;
		}*/
	}
}