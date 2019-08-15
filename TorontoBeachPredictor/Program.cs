// https://www.toronto.ca/city-government/data-research-maps/open-data/open-data-catalogue/#8a62b9bb-de65-921a-fcfa-266121cdae38
// https://www.toronto.ca/ext/open_data/catalog/data_set_files/Toronto_Beaches__Water_Quality_Readme.doc

using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Csv;
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
			const Int32 daysBackInTime = 5;

			var ints = new[] { 1, 2, 3, 4, 5, 6, 7 };
			var ts = ints.GetTimeSeries(3).Select().Select(x => x.ToArray()).ToArray();
			var ordered = ints.OrderByDescending(x => x).ToArray();

			using var context = new Context();
			var beaches = await context.Beaches.ToArrayAsync(AsyncProgram.CancellationToken);
			var beachSamples = await context.BeachSamples.Include(x => x.Beach).ToArrayAsync(AsyncProgram.CancellationToken);
			var weatherStations = await context.WeatherStations.ToArrayAsync(AsyncProgram.CancellationToken);
			var weatherSamples = await context.WeatherSamples.ToArrayAsync(AsyncProgram.CancellationToken);

			var joined =
				from beachSample in beachSamples
				from weatherSample in weatherSamples
				where beachSample.SampleDate == weatherSample.Date
				group (weatherSample.Date, weatherSample.MaximumTemperatureInC, weatherSample.MinimumTemperatureInC, weatherSample.TotalPrecipitationInMm, beachSample.EColiCount) by beachSample.Beach.Name into grouping
				select grouping;
			var timeSeries = joined.ToDictionary(x => x.Key, x => x.OrderBy(y => y.Date).GroupBy(y => y.Date.Year).ToDictionary(y => y.Key, y => y.ToArray().GetTimeSeries(daysBackInTime).Select().Select(z => z.ToArray()).ToArray()));

			var headers =
				Enumerable.Range(0, daysBackInTime + 1)
					.Select(x =>
						new[]
						{
							nameof(BeachSample.EColiCount) + x,
							nameof(WeatherSample.MaximumTemperatureInC) + x,
							nameof(WeatherSample.MinimumTemperatureInC) + x,
							nameof(WeatherSample.TotalPrecipitationInMm) + x,
						}
					)
					.SelectMany(x => x)
					.ToArray();
			var lines =
				timeSeries
					.SelectMany(x => x.Value)
					.SelectMany(x => x.Value)
					.Select(x => x
						.SelectMany(y =>
							new[]
							{
								y.EColiCount.ToString(),
								y.MaximumTemperatureInC.ToString(CultureInfo.InvariantCulture),
								y.MinimumTemperatureInC.ToString(CultureInfo.InvariantCulture),
								y.TotalPrecipitationInMm.ToString(CultureInfo.InvariantCulture),
							}
						)
						.ToArray()
					);
			await using var textWriter = File.CreateText("beach.csv");
			CsvWriter.Write(textWriter, headers, lines);
		}
	}
}
