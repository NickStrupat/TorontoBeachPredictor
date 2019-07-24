using Newtonsoft.Json;
using System;

namespace TorontoBeachPredictor
{
    struct WeatherData
    {
        public DateTime Date;
        public Double? MaximumTemperature;
        public Double? MinimumTemperature;
        /*
        <maxtemp description="Maximum Temperature" units="°C">6.3</maxtemp>
        <mintemp description="Minimum Temperature" units="°C">-6.6</mintemp>
        <meantemp description="Mean Temperature" units="°C">-0.1</meantemp>
        <heatdegdays description="Heating Degree Days" units="°C">18.1</heatdegdays>
        <cooldegdays description="Cooling Degree Days" units="°C">0.0</cooldegdays>
        <totalrain description="Total Rain" units="mm"></totalrain>
        <totalsnow description="Total Snow" flag="M" units="cm"></totalsnow>
        <totalprecipitation description="Total Precipitation" units="mm">0.4</totalprecipitation>
        <snowonground description="Snow on Ground" units="cm">0</snowonground>
        <dirofmaxgust description="Direction of Maximum Gust" flag="M" units="10s Deg"></dirofmaxgust>
        <speedofmaxgust description="Speed of Maximum Gust" flag="M" units="km/h"></speedofmaxgust>
         * */
        public override String ToString() => JsonConvert.SerializeObject(this, Formatting.None);
    }
}
