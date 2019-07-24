// https://www.toronto.ca/city-government/data-research-maps/open-data/open-data-catalogue/#8a62b9bb-de65-921a-fcfa-266121cdae38
// https://www.toronto.ca/ext/open_data/catalog/data_set_files/Toronto_Beaches__Water_Quality_Readme.doc

using System;

namespace TorontoBeachPredictor
{
    static class ParseExtensions {
        public static T? TryParse<T>(this String s, Func<String, T> parse) where T : struct => s == null ? (T?) null : parse(s);
        public static T? TryParse<T>(this String s, TryParseDelegate<T> tryParse) where T : struct => tryParse(s, out var result) ? result : (T?)null;
        public static String TryIntern(this String s) => s == null ? null : String.Intern(s);

        public delegate Boolean TryParseDelegate<T>(String s, out T result) where T : struct;
    }
}
