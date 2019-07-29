using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace TorontoBeachPredictor.Data
{
    public class WeatherStation
    {
        public Int32 Id { get; set; }
        [Required] public String Name { get; set; }
        public Int32 StationId { get; set; }
        public Double Latitude { get; set; }
        public Double Longitude { get; set; }
        public Double ElevationInMetres { get; set; }

        public override String ToString() => JsonConvert.SerializeObject(this, Formatting.None);
    }
}
