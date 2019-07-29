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
