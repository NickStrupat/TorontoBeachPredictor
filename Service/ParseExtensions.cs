using Csv;
using System;
using System.Xml.Linq;

namespace TorontoBeachPredictor.Service
{
	static class ParseExtensions
	{
		public static T? TryParse<T>(this String s, Func<String, T> parse) where T : struct =>
			s == null ? (T?)null : parse(s);
		public static T? TryParse<T>(this String s, TryParseDelegate<T> tryParse) where T : struct =>
			tryParse(s, out var result) ? result : (T?)null;
		public static String TryIntern(this String s) =>
			s == null ? null : String.Intern(s);

		public delegate Boolean TryParseDelegate<T>(String s, out T result) where T : struct;

		public static T? TryParseColumn<T>(this ICsvLine csvLine, String columnName, Func<String, T> parse) where T : struct =>
			!csvLine.HasColumn(columnName) ? null : csvLine[columnName]?.TryParse(parse);
		public static T? TryParseColumn<T>(this ICsvLine csvLine, String columnName, TryParseDelegate<T> tryParse) where T : struct =>
			!csvLine.HasColumn(columnName) ? null : csvLine[columnName]?.TryParse(tryParse);

		public static T? TryParseElement<T>(this XElement xElement, String elementName, Func<String, T> parse) where T : struct =>
			xElement.Element(elementName)?.Value.TryParse(parse);
		public static T? TryParseElement<T>(this XElement xElement, String elementName, TryParseDelegate<T> tryParse) where T : struct =>
			xElement.Element(elementName)?.Value.TryParse(tryParse);

		public static T? TryParseAttribute<T>(this XElement xElement, String elementName, Func<String, T> parse) where T : struct =>
			xElement.Attribute(elementName)?.Value.TryParse(parse);
		public static T? TryParseAttribute<T>(this XElement xElement, String elementName, TryParseDelegate<T> tryParse) where T : struct =>
			xElement.Attribute(elementName)?.Value.TryParse(tryParse);
	}
}
