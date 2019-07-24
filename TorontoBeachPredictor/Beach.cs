// https://www.toronto.ca/city-government/data-research-maps/open-data/open-data-catalogue/#8a62b9bb-de65-921a-fcfa-266121cdae38
// https://www.toronto.ca/ext/open_data/catalog/data_set_files/Toronto_Beaches__Water_Quality_Readme.doc

using Newtonsoft.Json;
using System;

namespace TorontoBeachPredictor
{
    struct Beach
    {
        public Int32? Id;
        public String Name;
        public Double? Latitude;
        public Double? Longitude;
        public override String ToString() => JsonConvert.SerializeObject(this, Formatting.None);
    }
}
