﻿// https://www.toronto.ca/city-government/data-research-maps/open-data/open-data-catalogue/#8a62b9bb-de65-921a-fcfa-266121cdae38
// https://www.toronto.ca/ext/open_data/catalog/data_set_files/Toronto_Beaches__Water_Quality_Readme.doc

using System;
using System.Collections.Generic;

namespace TorontoBeachPredictor
{
	public static class Extensions
	{
		public static IEnumerable<IEnumerable<T>> Select<T>(this T[,] array)
		{
			if (array == null)
				throw new ArgumentNullException(nameof(array));
			for (var i = 0; i != array.GetLength(0); i++)
				yield return GetRow(i);

			IEnumerable<T> GetRow(Int32 i)
			{
				for (var j = 0; j != array.GetLength(1); j++)
					yield return array[i, j];
			}
		}

		public static T[,] GetTimeSeries<T>(this T[] all, UInt32 daysBackInTime)
		{
			if (all == null)
				throw new ArgumentNullException(nameof(all));
			if (all.Length <= daysBackInTime)
				return new T[0, 0];
			var timeSeries = new T[all.Length - daysBackInTime, 1 + daysBackInTime];
			for (var i = 0; i != timeSeries.GetLength(0); i++)
				for (var j = 0; j != timeSeries.GetLength(1); j++)
					timeSeries[i, j] = all[daysBackInTime + i - j];
			return timeSeries;
		}
	}
}