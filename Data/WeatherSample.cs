using Newtonsoft.Json;
using System;

namespace TorontoBeachPredictor.Data
{
    public class WeatherSample
    {
        public Int64 Id { get; set; }
        public DateTime Date { get; set; }
        public Double MaximumTemperatureInC { get; set; }
        public Double MinimumTemperatureInC { get; set; }
        public Double MeanTemperatureInC { get; set; }
        public Double HeatingDegreeDaysInC { get; set; }
        public Double CoolingDegreeDaysInC { get; set; }
        public Double TotalRainInMm { get; set; }

        public Int32 WeatherStationId { get; set; }
        public WeatherStation WeatherStation { get; set; }

        public override String ToString() => JsonConvert.SerializeObject(this, Formatting.None);
    }
}
