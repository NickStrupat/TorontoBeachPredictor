// https://www.toronto.ca/city-government/data-research-maps/open-data/open-data-catalogue/#8a62b9bb-de65-921a-fcfa-266121cdae38
// https://www.toronto.ca/ext/open_data/catalog/data_set_files/Toronto_Beaches__Water_Quality_Readme.doc

using System;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace TorontoBeachPredictor
{
    static class Toronto
    {
        public static Beach[] Beaches { get; }
        public static BeachSample[] BeachSamples { get; }

        static Toronto()
        {
            var all = XElement.Load("all.xml");

            var beachesQuery =
                from x in all.Descendants("header").Descendants("beachMeta")
                select new Beach
                {
                    Id = x.Attribute("id")?.Value.TryParse(Int32.Parse),
                    Name = x.Attribute("name")?.Value,
                    Latitude = x.Attribute("lat")?.Value.TryParse(Double.Parse),
                    Longitude = x.Attribute("long")?.Value.TryParse(Double.Parse)
                };

            Beaches = beachesQuery.ToArray();

            var beachDataQuery =
                from beachData in all.Descendants("body").Descendants("beachData")
                select new BeachSample
                {
                    BeachId = beachData.Attribute("beachId")?.Value.TryParse(Int32.Parse),
                    SampleDate = beachData.Element("sampleDate")?.Value.TryParse(DateTime.Parse),
                    PublishDate = beachData.Element("publishDate")?.Value.TryParse(DateTime.Parse),
                    EColiCount = beachData.Element("eColiCount")?.Value.TryParse(x => Int32.Parse(x, NumberStyles.AllowThousands)),
                    BeachAdvisory = beachData.Element("beachAdvisory")?.Value.TryIntern(),
                    BeachStatus = beachData.Element("beachStatus")?.Value.TryIntern()
                };

            BeachSamples = beachDataQuery.ToArray();
        }
    }
}
