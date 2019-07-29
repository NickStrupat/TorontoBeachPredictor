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
            using var context = new BeachContext();
            var beaches = await context.Beaches.ToArrayAsync();
            var beachSamples = await context.BeachSamples.ToArrayAsync();

            //var averageBeachLatitude = beaches.Average(x => x.Latitude);
            //var averageBeachLongitude = beaches.Average(x => x.Longitude);
            //var stations = Canada.Stations.GroupBy(x => x.Province).ToDictionary(x => x.Key);
            //var nearAverageBeachQuery =
            //    from station in stations["ONTARIO"]
            //    where (station.Latitude.GetValueOrDefault(), station.Longitude.GetValueOrDefault()).DistanceTo((averageBeachLatitude, averageBeachLongitude), UnitOfLength.Kilometers) < 5
            //    select station;
            //var nearAverageBeach = nearAverageBeachQuery.ToList();

            //var urls = Canada.GetWeatherData(nearAverageBeach, (DateTime.Today, DateTime.Today));
            //var bestStation = urls.OrderByDescending(x => x.Value.Count(y => y.MaximumTemperature.HasValue && y.MinimumTemperature.HasValue)).First();

            //var joined =
            //    from beachSample in beachSamples
            //    from weatherData in bestStation.Value
            //    where beachSample.SampleDate == weatherData.Date
            //    group (weatherData.Date, weatherData.MaximumTemperature, weatherData.MinimumTemperature, beachSample.EColiCount) by beachSample.BeachId into grouping
            //    select grouping;
            //var what = joined.ToDictionary(x => x.Key, x => x.ToArray());
        }
    }
}
