using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TorontoBeachPredictor.Service;

namespace Service
{
    class Program
    {
        public static void Main(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) => services.AddHostedService<Worker>())
                .Build()
                .Run();
    }
}
