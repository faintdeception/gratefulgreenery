using System.ComponentModel;
using System.Web.Mvc;
using NogginBox.BingMapList.Models;
using System.Collections.Generic;
using System;
using System.Globalization;

namespace NogginBox.BingMapList.ViewModels
{
	public class EditBingMapListViewModel
	{
		#region BingMapList properties

		[DisplayName("Center latitude")]
		public String CenterLatitudeStr { get; set; }
		[DisplayName("Center longitude")]
		public String CenterLongitudeStr { get; set; }
		public int? Width { get; set; }
		public int Height { get; set; }
		public int Zoom { get; set; }

		[DisplayName("Map type")]
		public string MapType { get; set; }

		public IList<String> Titles { get; set; }
		public IEnumerable<BingLocationPart> BingLocations { get; set; }

		public float CenterLatitude
		{
			get
			{
				return Convert.ToSingle(CenterLatitudeStr, CultureInfo.InvariantCulture.NumberFormat);
			}
		}

		public float CenterLongitude
		{
			get
			{
				return Convert.ToSingle(CenterLongitudeStr, CultureInfo.InvariantCulture.NumberFormat);
			}
		}

		#endregion

		/// <summary>
		/// The types of map available
		/// </summary>
		public SelectList MapTypeList { get; set; }

		public String MapIconFolder { get; set; }
	}
}