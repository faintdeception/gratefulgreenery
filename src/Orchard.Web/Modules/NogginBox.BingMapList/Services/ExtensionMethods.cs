using System;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;

namespace NogginBox.BingMapList.Services
{
	/// <summary>
	/// Useful import/export methods
	/// </summary>
	internal static class ExtensionMethods
	{
		internal static string GetAttribute(this ImportContentContext context, String attributeName, ContentPart part)
		{
			return context.Attribute(part.PartDefinition.Name, attributeName);
		}

		internal static T TryParseWithDefault<T>(this String value, T defaultValue, TryParseHandler<T> handler)
		{
			if (String.IsNullOrEmpty(value)) return defaultValue;
			T result;

			return (handler(value, out result))
			? result
			: defaultValue;
		}
		internal delegate bool TryParseHandler<T>(string value, out T result);
	}
}