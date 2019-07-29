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
            await Initialize(stoppingToken);
            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                //await Task.Delay(TimeSpan.FromHours(1), stoppingToken);

                using var context = new Context();
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

        private async Task Initialize(CancellationToken cancellationToken)
        {
            using var context = new Context();
            var databaseWasCreated = await context.Database.EnsureCreatedAsync(cancellationToken);
            if (!databaseWasCreated)
                return;

            var (beaches, beachSamples) = await Toronto.GetAll(cancellationToken);
            context.Beaches.AddRange(GetBeaches(beaches));
            context.BeachSamples.AddRange(GetBeachSamples(beachSamples));
            await context.SaveChangesAsync(cancellationToken);
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await base.StartAsync(cancellationToken);
            logger.LogInformation("Worker starting at: {time}", DateTimeOffset.Now);
            //return Initialize(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Worker stopping at: {time}", DateTimeOffset.Now);
            return base.StopAsync(cancellationToken);
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
    }
}