// https://www.toronto.ca/city-government/data-research-maps/open-data/open-data-catalogue/#8a62b9bb-de65-921a-fcfa-266121cdae38
// https://www.toronto.ca/ext/open_data/catalog/data_set_files/Toronto_Beaches__Water_Quality_Readme.doc

using Newtonsoft.Json;
using System;

namespace TorontoBeachPredictor
{
    struct BeachSample
    {
        public Int32? BeachId;
        public DateTime? SampleDate;
        public DateTime? PublishDate;
        public Int32? EColiCount;
        public String BeachAdvisory;
        public String BeachStatus;
        public override String ToString() => JsonConvert.SerializeObject(this, Formatting.None);
    }
}
