// https://www.toronto.ca/city-government/data-research-maps/open-data/open-data-catalogue/#8a62b9bb-de65-921a-fcfa-266121cdae38
// https://www.toronto.ca/ext/open_data/catalog/data_set_files/Toronto_Beaches__Water_Quality_Readme.doc

using System;
using System.Linq;

namespace TorontoBeachPredictor
{
    class Program
    {
        static void Main(String[] args)
        {
            var averageBeachLatitude = Toronto.Beaches.Average(x => x.Latitude).GetValueOrDefault();
            var averageBeachLongitude = Toronto.Beaches.Average(x => x.Longitude).GetValueOrDefault();
            var stations = Canada.Stations.GroupBy(x => x.Province).ToDictionary(x => x.Key);
            var nearAverageBeachQuery =
                from station in stations["ONTARIO"]
                where (station.Latitude.GetValueOrDefault(), station.Longitude.GetValueOrDefault()).DistanceTo((averageBeachLatitude, averageBeachLongitude), UnitOfLength.Kilometers) < 5
                select station;
            var nearAverageBeach = nearAverageBeachQuery.ToList();

            var urls = Canada.GetWeatherData(nearAverageBeach, (DateTime.Today, DateTime.Today));
            var bestStation = urls.OrderByDescending(x => x.Value.Count(y => y.MaximumTemperature.HasValue && y.MinimumTemperature.HasValue)).First();

            var joined =
                from beachSample in Toronto.BeachSamples
                from weatherData in bestStation.Value
                where beachSample.SampleDate == weatherData.Date
                group (weatherData.Date, weatherData.MaximumTemperature, weatherData.MinimumTemperature, beachSample.EColiCount) by beachSample.BeachId into grouping
                select grouping;
            var what = joined.ToDictionary(x => x.Key, x => x.ToArray());
        }
    }
}
