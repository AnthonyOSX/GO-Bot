using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Xml;

namespace GO_Bot.Internals {

	internal static class JsonToXmlGpxConverter {

		public static string Convert(string json) {
			JArray jArray = JArray.Parse(json);

			if (jArray.Count == 0) {
				throw new Exception("No lat/long coords defined in the JSON object");
			}

			string fileName = Path.GetTempFileName();

			using (XmlWriter xmlWriter = XmlWriter.Create(fileName)) {
				xmlWriter.WriteStartDocument();
				xmlWriter.WriteStartElement("gpx");
				xmlWriter.WriteStartElement("trk");
				xmlWriter.WriteStartElement("trkseg");
				
				foreach (dynamic latLonDef in jArray) {
					xmlWriter.WriteStartElement("trkpt");
					xmlWriter.WriteElementString("lat", latLonDef.lat);
					xmlWriter.WriteElementString("lon", latLonDef.lon);
					xmlWriter.WriteEndElement();
				}

				xmlWriter.WriteEndElement();
				xmlWriter.WriteEndElement();
				xmlWriter.WriteEndElement();
				xmlWriter.WriteEndDocument();
			}

			return fileName;
		}

	}

}
