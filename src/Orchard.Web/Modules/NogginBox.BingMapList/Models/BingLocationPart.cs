using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.Core.Common.Utilities;
using System;

namespace NogginBox.BingMapList.Models
{
	public class BingLocationRecord: ContentPartRecord
	{
		public virtual float Latitude { get; set;}
		public virtual float Longitude { get; set;}

		public virtual bool IsEnabled { get; set; }
		public virtual int? Width { get; set; }
		public virtual int Height { get; set; }
		public virtual int Zoom { get; set; }
		[StringLength(4)]
		public virtual String MapType { get; set; }
		public virtual String MapIcon { get; set; }

		// MapList associations
		public virtual ContentItemRecord BingMapList { get; set; }
	}

	public class BingLocationPart : ContentPart<BingLocationRecord>
	{
		[Required]
		public float Latitude
		{
			get { return Record.Latitude; }
			set { Record.Latitude = value; }
		}

		[Required]
		public float Longitude
		{
			get { return Record.Longitude; }
			set { Record.Longitude = value; }
		}

		public bool IsEnabled
		{
			get { return Record.IsEnabled; }
			set { Record.IsEnabled = value; }
		}

		public int? Width
		{
			get { return Record.Width; }
			set { Record.Width = value; }
		}

		[Required]
		public int Height
		{
			get { return Record.Height; }
			set { Record.Height = value; }
		}

		[Required]
		public int Zoom
		{
			get { return Record.Zoom; }
			set { Record.Zoom = value; }
		}

		[Required]
		public String MapType
		{
			get { return Record.MapType; }
			set { Record.MapType = value; }
		}

		public String MapIcon
		{
			get { return Record.MapIcon; }
			set { Record.MapIcon = value; }
		}

		#region MapList associations

		private readonly LazyField<IContent> _bingMapList = new LazyField<IContent>();

		public LazyField<IContent> BingMapListField { get { return _bingMapList; } }

		public IContent BingMapList
		{
			get { return _bingMapList.Value; }
			set { _bingMapList.Value = value; }
		}

		#endregion
	}
}