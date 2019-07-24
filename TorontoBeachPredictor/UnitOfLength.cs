// https://www.toronto.ca/city-government/data-research-maps/open-data/open-data-catalogue/#8a62b9bb-de65-921a-fcfa-266121cdae38
// https://www.toronto.ca/ext/open_data/catalog/data_set_files/Toronto_Beaches__Water_Quality_Readme.doc

using System;

namespace TorontoBeachPredictor
{
    public class UnitOfLength
    {
        public static UnitOfLength Kilometers = new UnitOfLength(1.609344);
        public static UnitOfLength NauticalMiles = new UnitOfLength(0.8684);
        public static UnitOfLength Miles = new UnitOfLength(1);

        private readonly Double fromMilesFactor;

        private UnitOfLength(Double fromMilesFactor) => this.fromMilesFactor = fromMilesFactor;

        public Double ConvertFromMiles(Double input) => input * fromMilesFactor;
    }
}
