using System;

namespace TorontoBeachPredictor.Service
{
	static class CoordinatesExtensions
	{
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