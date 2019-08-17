using Csv;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TorontoBeachPredictor.Service
{
	static class EnvironmentCanada
	{
		public struct WeatherStation
		{
			public String Name;
			public String Province;
			public Int32? StationId;
			public Double? Latitude;
			public Double? Longitude;
			public Double? ElevationInMetres;

			public override String ToString() => JsonConvert.SerializeObject(this, Formatting.None);
		}

		public struct WeatherSample
		{
			public DateTime Date;
			public Double? MaximumTemperatureInC;
			public Double? MinimumTemperatureInC;
			public Double? MeanTemperatureInC;
			public Double? HeatingDegreeDaysInC;
			public Double? CoolingDegreeDaysInC;
			public Double? TotalPrecipitationInMm;

			public override String ToString() => JsonConvert.SerializeObject(this, Formatting.None);
		}

		public enum Timeframe { Hourly = 1, Daily = 2, Monthly = 3 }

		public enum Format { Csv, Xml }


		private const String WeatherStationsRequestUri =
			"ftp://client_climate@ftp.tor.ec.gc.ca/Pub/Get_More_Data_Plus_de_donnees/Station%20Inventory%20EN.csv";

		private static String GetWeatherDataUrl(Format format, Int32 stationId, DateTime date, Timeframe timeframe) =>
			$"http://climate.weather.gc.ca/climate_data/bulk_data_e.html?format={format.ToString().ToLower()}&stationID={stationId}&Year={date.Year}&Month={date.Month}&Day={date.Day}&timeframe={timeframe:D}";

		public static async Task<WeatherStation[]> GetAllStations()
		{
			var ftpWebRequest = (FtpWebRequest) WebRequest.Create(WeatherStationsRequestUri);
			ftpWebRequest.Method = WebRequestMethods.Ftp.DownloadFile;
			using var response = await ftpWebRequest.GetResponseAsync();
			await using var responseStream = response.GetResponseStream();

			var stationsQuery =
				from line in CsvReader.ReadFromStream(responseStream, new CsvOptions {RowsToSkip = 3})
				select new WeatherStation
				{
					Name = line["Name"],
					Province = line["Province"],
					StationId = line["Station ID"]?.TryParse<Int32>(Int32.TryParse),
					Latitude = line["Latitude (Decimal Degrees)"].TryParse<Double>(Double.TryParse),
					Longitude = line["Longitude (Decimal Degrees)"].TryParse<Double>(Double.TryParse),
					ElevationInMetres = line["Elevation (m)"].TryParse<Double>(Double.TryParse)
				};
			return stationsQuery.ToArray();
		}

		public static async Task<WeatherSample[]> GetWeatherSamples(Int32 weatherStationId, DateTime from, DateTime to, CancellationToken cancellationToken)
		{
			var min = Math.Min(from.Year, to.Year);
			var max = Math.Max(from.Year, to.Year);
			var years = Enumerable.Range(min, max - min + 1).ToArray();

			using var httpClient = new HttpClient();
			var weatherDataResponsesQuery =
				from year in years
				let url = GetWeatherDataUrl(Format.Xml, weatherStationId, new DateTime(year, DateTime.Today.Month, DateTime.Today.Day), Timeframe.Daily)
				select httpClient.GetAsync(url, cancellationToken);
			var weatherDataResponses = weatherDataResponsesQuery.ToArray();
			await Task.WhenAll(weatherDataResponses);

			var weatherDataQuery =
				from weatherDataResponse in weatherDataResponses
				let stream = weatherDataResponse.Result.Content.ReadAsStreamAsync().Result
				let xElement = XElement.Load(stream)
				from stationData in xElement.Descendants("stationdata")
				let day = stationData.Attribute("day")?.Value.TryParse(Int32.Parse)
				let month = stationData.Attribute("month")?.Value.TryParse(Int32.Parse)
				let year = stationData.Attribute("year")?.Value.TryParse(Int32.Parse)
				where new[] {day, month, year}.All(x => x.HasValue)
				select new WeatherSample
				{
					Date = new DateTime(year.Value, month.Value, day.Value),
					MaximumTemperatureInC = stationData.Element("maxtemp")?.Value.TryParse<Double>(Double.TryParse),
					MinimumTemperatureInC = stationData.Element("mintemp")?.Value.TryParse<Double>(Double.TryParse),
					MeanTemperatureInC = stationData.Element("meantemp")?.Value.TryParse<Double>(Double.TryParse),
					HeatingDegreeDaysInC = stationData.Element("heatdegdays")?.Value.TryParse<Double>(Double.TryParse),
					CoolingDegreeDaysInC = stationData.Element("cooldegdays")?.Value.TryParse<Double>(Double.TryParse),
					TotalPrecipitationInMm = stationData.Element("totalprecipitation")?.Value.TryParse<Double>(Double.TryParse),
				};

			return weatherDataQuery.ToArray();
		}
	}
}