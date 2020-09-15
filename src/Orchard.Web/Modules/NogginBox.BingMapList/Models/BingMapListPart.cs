using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.Core.Common.Utilities;

namespace NogginBox.BingMapList.Models
{
	public class BingMapListRecord : ContentPartRecord
	{
		public virtual float CenterLatitude { get; set; }
		public virtual float CenterLongitude { get; set; }

		public virtual int? Width { get; set; }
		public virtual int Height { get; set; }
		public virtual int Zoom { get; set; }
		[StringLength(20)]
		public virtual string MapType { get; set; }
	}

	public class BingMapListPart : ContentPart<BingMapListRecord>
	{
		[Required]
		public float CenterLatitude
		{
			get { return Record.CenterLatitude; }
			set { Record.CenterLatitude = value; }
		}

		[Required]
		public float CenterLongitude
		{
			get { return Record.CenterLongitude; }
			set { Record.CenterLongitude = value; }
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
		public string MapType
		{
			get { return Record.MapType; }
			set { Record.MapType = value; }
		}



		#region MapLocation associations

		private readonly LazyField<IEnumerable<BingLocationPart>> _bingLocations = new LazyField<IEnumerable<BingLocationPart>>();
		public LazyField<IEnumerable<BingLocationPart>> BingLocationsField { get { return _bingLocations; } }

		public IEnumerable<BingLocationPart> BingLocations
		{
			get { return _bingLocations.Value; }
			set { _bingLocations.Value = value; }
		}

		#endregion
	}
}