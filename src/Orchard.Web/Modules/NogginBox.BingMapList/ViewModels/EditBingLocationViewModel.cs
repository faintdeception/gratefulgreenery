using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.ComponentModel;
using System.Globalization;

namespace NogginBox.BingMapList.ViewModels
{
	public class EditBingLocationViewModel
	{
		//public int BingLocationId { get; set; }

		#region BingLocation properties

		[DisplayName("Latitude")]
		public String LatitudeStr { get; set; }
		[DisplayName("Longitude")]
		public String LongitudeStr { get; set; }
        public bool IsEnabled { get; set; }
		public int? Width { get; set; }
		public int Height { get; set; }
		public int Zoom { get; set; }

		[DisplayName("Map type")]
		public String MapType { get; set; }
		public String MapIcon { get; set; }

		public float Latitude
		{
			get
			{
				return Convert.ToSingle(LatitudeStr, CultureInfo.InvariantCulture.NumberFormat);
			}
		}

		public float Longitude
		{
			get
			{
				return Convert.ToSingle(LongitudeStr, CultureInfo.InvariantCulture.NumberFormat);
			}
		}

		#endregion
		
		public int? BingMapListId { get; set; }

		public IEnumerable<BingMapListViewModel> BingMapLists { get; set; }

		/// <summary>
		/// The types of map available
		/// </summary>
		public SelectList MapTypeList { get; set; }

		public IEnumerable<String> PossibleMapIcons;
		public String MapIconFolder { get; set; }
	}

	public class BingMapListViewModel
	{
		public int Id { get; set; }
		public String Name { get; set; }
	}
}