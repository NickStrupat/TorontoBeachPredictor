// https://www.toronto.ca/city-government/data-research-maps/open-data/open-data-catalogue/#8a62b9bb-de65-921a-fcfa-266121cdae38
// https://www.toronto.ca/ext/open_data/catalog/data_set_files/Toronto_Beaches__Water_Quality_Readme.doc

using System;

namespace TorontoBeachPredictor
{
    static class CoordinatesExtensions {
        public static Double DistanceTo(this (Double Latitude, Double Longitude) baseCoordinates, (Double Latitude, Double Longitude) targetCoordinates, UnitOfLength unitOfLength)
        {
            var baseRad = Math.PI * baseCoordinates.Latitude / 180;
            var targetRad = Math.PI * targetCoordinates.Latitude / 180;
            var theta = baseCoordinates.Longitude - targetCoordinates.Longitude;
            var thetaRad = Math.PI * theta / 180;

            var distance =
                Math.Sin(baseRad) * Math.Sin(targetRad) + Math.Cos(baseRad) *
                Math.Cos(targetRad) * Math.Cos(thetaRad);
            distance = Math.Acos(distance);

            distance = distance * 180 / Math.PI;
            distance = distance * 60 * 1.1515;

            return unitOfLength.ConvertFromMiles(distance);
        }
    }
}
