
using NogginBox.BingMapList.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;

namespace NogginBox.BingMapList.Handlers
{
	public class BingLocationHandler : ContentHandler
	{
		private readonly IContentManager _contentManager;


		public BingLocationHandler(IRepository<BingLocationRecord> repository, IContentManager contentManager)
		{
			Filters.Add(StorageFilter.For(repository));
			_contentManager = contentManager;

			OnInitializing<BingLocationPart>(propertySetHandlers);
			OnLoaded<BingLocationPart>(lazyLoadHandlers);
		}

		void lazyLoadHandlers(LoadContentContext context, BingLocationPart part)
		{
			// add handlers that will load content just-in-time
			part.BingMapListField.Loader(() =>
				part.Record.BingMapList == null ?
				null : _contentManager.Get(part.Record.BingMapList.Id));
		}

		static void propertySetHandlers(InitializingContentContext context, BingLocationPart part)
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
		}
	}
}
