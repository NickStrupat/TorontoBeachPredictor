// https://www.toronto.ca/city-government/data-research-maps/open-data/open-data-catalogue/#8a62b9bb-de65-921a-fcfa-266121cdae38
// https://www.toronto.ca/ext/open_data/catalog/data_set_files/Toronto_Beaches__Water_Quality_Readme.doc

using Newtonsoft.Json;
using System;

namespace TorontoBeachPredictor
{
    class Station
    {
        public String Name;
        public String Province;
        public Int32? StationId;
        public Double? Latitude;
        public Double? Longitude;
        public Double? ElevationInMetres;
        //public Int32? FirstYear;
        //public Int32? LastYear;
        //public Int32? HourlyFirstYear;
        //public Int32? HourlyLastYear;
        //public Int32? DailyFirstYear;
        //public Int32? DailyLastYear;
        //public Int32? MonthlyFirstYear;
        //public Int32? MonthlyLastYear;
        public override String ToString() => JsonConvert.SerializeObject(this, Formatting.None);
    }
}
