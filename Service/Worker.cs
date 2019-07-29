using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TorontoBeachPredictor.Data;

namespace TorontoBeachPredictor.Service
{

    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;

        public Worker(ILogger<Worker> logger) => this.logger = logger;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await InitializeBeaches(stoppingToken);
            await InitializeWeather(stoppingToken);
            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                //await Task.Delay(TimeSpan.FromHours(1), stoppingToken);

                using var context = new BeachContext();
                var mostRecent = context.BeachSamples.Max(x => x.PublishDate);
                var (beaches, beachSamples) = await Toronto.GetRange(mostRecent, DateTime.Today, stoppingToken);

                var existingBeaches = await context.Beaches.ToArrayAsync();
                var currentBeaches = GetBeaches(beaches);
                var newBeaches = currentBeaches.Except(existingBeaches);
                context.Beaches.AddRange(newBeaches);

                var existingBeachSamples = await context.BeachSamples.Where(x => x.PublishDate >= DateTime.Today).ToArrayAsync();
                var currentBeachSamples = GetBeachSamples(beachSamples);
                var newBeachSamples = currentBeachSamples.Except(existingBeachSamples);
                context.BeachSamples.AddRange(newBeachSamples);

                await context.SaveChangesAsync(stoppingToken);
            }
        }

        private async Task InitializeBeaches(CancellationToken cancellationToken)
        {
            using var context = new BeachContext();
            await context.Database.EnsureCreatedAsync(cancellationToken);
            var shouldNotSeed = await context.Beaches.AnyAsync() || await context.BeachSamples.AnyAsync();
            if (!shouldNotSeed)
            {
                var (beaches, beachSamples) = await Toronto.GetAll(cancellationToken);
                context.Beaches.AddRange(GetBeaches(beaches));
                context.BeachSamples.AddRange(GetBeachSamples(beachSamples));
                await context.SaveChangesAsync(cancellationToken);
            }
        }

        private async Task InitializeWeather(CancellationToken cancellationToken)
        {
            using var context = new WeatherContext();
            await context.Database.EnsureCreatedAsync(cancellationToken);
            var shouldNotSeed = await context.WeatherStations.AnyAsync() || await context.WeatherSamples.AnyAsync();
            if (!shouldNotSeed)
            {
                var weatherStations = await Canada.GetAllStations();
                var weatherSamples = await Canada.GetWeatherSamples(weatherStations.Single(x => x.Name == "TORONTO CITY").StationId.Value, new DateTime(2007, 06, 01), DateTime.Today, cancellationToken);
                context.WeatherStations.AddRange(GetWeatherStations(weatherStations));
                context.WeatherSamples.AddRange(GetWeatherSamples(weatherSamples));
                await context.SaveChangesAsync(cancellationToken);
            }
        }

        private static IEnumerable<Beach> GetBeaches(Toronto.Beach[] beaches) =>
            from beach in beaches
            where beach.Id != null
            where beach.Name != null
            where beach.Latitude != null
            where beach.Longitude != null
            select new Beach
            {
                Id = beach.Id.Value,
                Name = beach.Name,
                Latitude = beach.Latitude.Value,
                Longitude = beach.Longitude.Value
            };

        private static IEnumerable<BeachSample> GetBeachSamples(Toronto.BeachSample[] beachSamples) => 
            from beachSample in beachSamples
            where beachSample.BeachId != null
            where beachSample.SampleDate != null
            where beachSample.PublishDate != null
            where beachSample.EColiCount != null
            where Enum.TryParse<BeachStatus>(beachSample.BeachStatus, out _)
            select new BeachSample
            {
                BeachId = beachSample.BeachId.Value,
                SampleDate = beachSample.SampleDate.Value,
                PublishDate = beachSample.PublishDate.Value,
                EColiCount = beachSample.EColiCount.Value,
                BeachStatus = Enum.Parse<BeachStatus>(beachSample.BeachStatus)
            };

        private static IEnumerable<WeatherStation> GetWeatherStations(Canada.WeatherStation[] weatherStations) =>
            from weatherStation in weatherStations
            where weatherStation.StationId != null
            where weatherStation.Province != null
            where weatherStation.Name != null
            where weatherStation.Latitude != null
            where weatherStation.Longitude != null
            select new WeatherStation
            {
                Name = weatherStation.Name,
                StationId = weatherStation.StationId.Value,
                Latitude = weatherStation.Latitude.Value,
                Longitude = weatherStation.Longitude.Value,
                ElevationInMetres = weatherStation.ElevationInMetres.Value
            };

        private static IEnumerable<WeatherSample> GetWeatherSamples(Canada.WeatherSample[] weatherSamples) =>
            from weatherSample in weatherSamples
            where weatherSample.MaximumTemperatureInC != null
            where weatherSample.MinimumTemperatureInC != null
            where weatherSample.MeanTemperatureInC != null
            where weatherSample.HeatingDegreeDaysInC != null
            where weatherSample.CoolingDegreeDaysInC != null
            where weatherSample.TotalRainInMm != null
            select new WeatherSample
            {
                Date = weatherSample.Date,
                MaximumTemperatureInC = weatherSample.MaximumTemperatureInC.Value,
                MinimumTemperatureInC = weatherSample.MinimumTemperatureInC.Value,
                MeanTemperatureInC = weatherSample.MeanTemperatureInC.Value,
                HeatingDegreeDaysInC = weatherSample.HeatingDegreeDaysInC.Value,
                CoolingDegreeDaysInC = weatherSample.CoolingDegreeDaysInC.Value,
                TotalRainInMm = weatherSample.TotalRainInMm.Value,
            };
    }
}