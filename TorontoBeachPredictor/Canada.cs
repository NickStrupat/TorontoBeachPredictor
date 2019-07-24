// https://www.toronto.ca/city-government/data-research-maps/open-data/open-data-catalogue/#8a62b9bb-de65-921a-fcfa-266121cdae38
// https://www.toronto.ca/ext/open_data/catalog/data_set_files/Toronto_Beaches__Water_Quality_Readme.doc

using Csv;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TorontoBeachPredictor
{
    enum Timeframe { Hourly = 1, Daily = 2, Monthly = 3 }
    enum Format { Csv, Xml }

    static class Canada {
        public static Station[] Stations { get; }

        public static IReadOnlyDictionary<Station, WeatherData[]> GetWeatherData(IEnumerable<Station> stations, (DateTime Start, DateTime End) dateRange)
        {
            var start = dateRange.Start.Year;
            var end = dateRange.End.Year;
            var min = Math.Min(start, end);
            var max = Math.Max(start, end);
            var years = Enumerable.Range(min, max - min + 1).ToArray();

            var cts = new CancellationTokenSource();
            using var httpClient = new HttpClient();
            var weatherDataStreamsQuery =
                from station in stations
                from year in years
                let url = GetWeatherDataUrl(Format.Xml, station.StationId.GetValueOrDefault(), new DateTime(year, DateTime.Today.Month, DateTime.Today.Day), Timeframe.Daily)
                select httpClient.GetStreamAsync(url).ContinueWith(x => (station, xElement: XElement.Load(x.Result)));
            var weatherDataStreams = weatherDataStreamsQuery.ToArray();
            Task.WaitAll(weatherDataStreams);

            var weatherDataQuery =
                from weatherDataStream in weatherDataStreams.AsParallel()
                let station = weatherDataStream.Result.station
                let xElement = weatherDataStream.Result.xElement
                from stationData in xElement.Descendants("stationdata")
                let day = stationData.Attribute("day").Value.TryParse(Int32.Parse).Value
                let month = stationData.Attribute("month").Value.TryParse(Int32.Parse).Value
                let year = stationData.Attribute("year").Value.TryParse(Int32.Parse).Value
                let weatherData = new WeatherData
                {
                    Date = new DateTime(year, month, day),
                    MaximumTemperature = stationData.Element("maxtemp")?.Value.TryParse<Double>(Double.TryParse),
                    MinimumTemperature = stationData.Element("mintemp")?.Value.TryParse<Double>(Double.TryParse),
                }
                group weatherData by station into grouping
                select grouping;

            return weatherDataQuery.ToDictionary(x => x.Key, x => x.ToArray());
        }

        private static String GetWeatherDataUrl(Format format, Int32 stationId, DateTime date, Timeframe timeframe) =>
            $"http://climate.weather.gc.ca/climate_data/bulk_data_e.html?format={format.ToString().ToLower()}&stationID={stationId}&Year={date.Year}&Month={date.Month}&Day={date.Day}&timeframe={timeframe:D}";

        static Canada()
        {
            var ftpWebRequest = (FtpWebRequest)WebRequest.Create("ftp://client_climate@ftp.tor.ec.gc.ca/Pub/Get_More_Data_Plus_de_donnees/Station%20Inventory%20EN.csv");
            ftpWebRequest.Method = WebRequestMethods.Ftp.DownloadFile;
            using var response = (FtpWebResponse)ftpWebRequest.GetResponse();
            using var responseStream = response.GetResponseStream();
            using var streamReader = new StreamReader(responseStream, Encoding.UTF8);
            var responseString = streamReader.ReadToEnd();

            var stationsQuery =
                from line in CsvReader.ReadFromText(responseString, new CsvOptions { RowsToSkip = 3 })
                select new Station
                {
                    Name = line["Name"],
                    Province = line["Province"],
                    StationId = line["Station ID"]?.TryParse<Int32>(Int32.TryParse),
                    Latitude = line["Latitude (Decimal Degrees)"].TryParse<Double>(Double.TryParse),
                    Longitude = line["Longitude (Decimal Degrees)"].TryParse<Double>(Double.TryParse),
                    ElevationInMetres = line["Elevation (m)"].TryParse<Double>(Double.TryParse)
                };

            Stations = stationsQuery.ToArray();
        }
    }
}
