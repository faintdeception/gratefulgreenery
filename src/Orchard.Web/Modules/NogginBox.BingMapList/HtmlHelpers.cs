using System;
using System.Web;
using System.Web.Mvc;

namespace NogginBox.BingMapList
{
	public static class HtmlHelpers
	{
		public static HtmlString toInvariantCultureString(this HtmlHelper html, float number)
		{
			return new HtmlString(number.toInvariantCultureString());
		}

		public static String toInvariantCultureString(this float number)
		{
			return number.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		}
	}
}