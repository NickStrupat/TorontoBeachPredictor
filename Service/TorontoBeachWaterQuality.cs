using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TorontoBeachPredictor.Service
{
	static class TorontoBeachWaterQuality
	{
		public struct Beach
		{
			public Int32? Id;
			public String Name;
			public Double? Latitude;
			public Double? Longitude;
			public override String ToString() => JsonConvert.SerializeObject(this, Formatting.None);
		}

		public struct BeachSample
		{
			public Int32? BeachId;
			public DateTime? SampleDate;
			public DateTime? PublishDate;
			public Int32? EColiCount;
			public String BeachAdvisory;
			public String BeachStatus;
			public override String ToString() => JsonConvert.SerializeObject(this, Formatting.None);
		}

		private const String AllDataRequestUri = "http://app.toronto.ca/tpha/ws/beaches/history/all.xml?v=1.0";

		private static String GetDateRangeDataRequestUri(DateTime from, DateTime to) =>
			$"http://app.toronto.ca/tpha/ws/beaches/history.xml?v=1.0&from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}";

		public static Task<(Beach[] Beaches, BeachSample[] BeachSamples)> GetRange(DateTime from, DateTime to, CancellationToken cancellationToken) =>
			GetBeachData(GetDateRangeDataRequestUri(from, to), cancellationToken);

		public static Task<(Beach[] Beaches, BeachSample[] BeachSamples)> GetAll(CancellationToken cancellationToken) =>
			GetBeachData(AllDataRequestUri, cancellationToken);

		private static async Task<(Beach[] Beaches, BeachSample[] BeachSamples)> GetBeachData(String requestUri, CancellationToken cancellationToken)
		{
			using var httpClient = new HttpClient();
			var httpResponseMessage = await httpClient.GetAsync(requestUri, cancellationToken);
			var xml = await XElement.LoadAsync(await httpResponseMessage.Content.ReadAsStreamAsync(), LoadOptions.None, cancellationToken);

			var beachesQuery =
				from x in xml.Descendants("header").Descendants("beachMeta")
				select new Beach
				{
					Id = x.Attribute("id")?.Value.TryParse(Int32.Parse),
					Name = x.Attribute("name")?.Value,
					Latitude = x.Attribute("lat")?.Value.TryParse(Double.Parse),
					Longitude = x.Attribute("long")?.Value.TryParse(Double.Parse)
				};

			var beachSampleQuery =
				from beachData in xml.Descendants("body").Descendants("beachData")
				select new BeachSample
				{
					BeachId = beachData.Attribute("beachId")?.Value.TryParse(Int32.Parse),
					SampleDate = beachData.Element("sampleDate")?.Value.TryParse(DateTime.Parse),
					PublishDate = beachData.Element("publishDate")?.Value.TryParse(DateTime.Parse),
					EColiCount = beachData.Element("eColiCount")?.Value.TryParse(x => Int32.Parse(x, NumberStyles.AllowThousands)),
					BeachAdvisory = beachData.Element("beachAdvisory")?.Value.TryIntern(),
					BeachStatus = beachData.Element("beachStatus")?.Value.TryIntern()
				};

			return (beachesQuery.ToArray(), beachSampleQuery.ToArray());
		}
	}
}
