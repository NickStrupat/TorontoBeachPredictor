// https://www.toronto.ca/city-government/data-research-maps/open-data/open-data-catalogue/#8a62b9bb-de65-921a-fcfa-266121cdae38
// https://www.toronto.ca/ext/open_data/catalog/data_set_files/Toronto_Beaches__Water_Quality_Readme.doc

using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TorontoBeachPredictor.Data;

namespace TorontoBeachPredictor
{
    public static class AsyncProgram
    {
        private static readonly CancellationTokenSource cts = new CancellationTokenSource();
        static AsyncProgram() => Console.CancelKeyPress += (s, e) => cts.Cancel();
        public static CancellationToken CancellationToken => cts.Token;
    }

    class Program
    {
        static async Task Main(String[] args)
        {
	        var ints = new[] { 1, 2, 3, 4, 5, 6, 7 };
	        var ts = GetTimeSeries(ints, 3).Select().Select(x => x.ToArray()).ToArray();

            using var context = new Context();
            var beaches = await context.Beaches.ToArrayAsync(AsyncProgram.CancellationToken);
            var beachSamples = await context.BeachSamples.ToArrayAsync(AsyncProgram.CancellationToken);
            var weatherStations = await context.WeatherStations.ToArrayAsync(AsyncProgram.CancellationToken);
            var weatherSamples = await context.WeatherSamples.ToArrayAsync(AsyncProgram.CancellationToken);

            var joined =
				from beachSample in beachSamples
				from weatherSample in weatherSamples
				where beachSample.SampleDate == weatherSample.Date
				group (weatherSample.Date, weatherSample.MaximumTemperatureInC, weatherSample.MinimumTemperatureInC, weatherSample.TotalPrecipitationInMm, beachSample.EColiCount) by beachSample.BeachId into grouping
				select grouping;
			var what = joined.ToDictionary(x => x.Key, x => x.OrderBy(y => y.Date).GroupBy(y => y.Date.Year).ToDictionary(y => y.Key, y => GetTimeSeries(y.ToArray(), 5)));
        }

		static T[,] GetTimeSeries<T>(T[] all, UInt32 daysBackInTime)
		{
			if (all.Length <= daysBackInTime)
				return null;
			var timeSeries = new T[all.Length - daysBackInTime, 1 + daysBackInTime];
			for (var i = 0; i != timeSeries.GetLength(0); i++)
			for (var j = 0; j != timeSeries.GetLength(1); j++)
				timeSeries[i, j] = all[daysBackInTime + i - j];
			return timeSeries;
		}
    }
}
