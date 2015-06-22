using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using GeoJSON.Net.Feature;
using mapnik.vector;
using Newtonsoft.Json;
using NUnit.Framework;
using ProtoBuf;

namespace Mapbox.Vectors.Tests
{
    [TestFixture]
    public class ParseTest
    {
        /// <summary>
        /// A simple parsing test based on the js-test in: https://github.com/mapbox/vector-tile-js/blob/master/test/parse.test.js
        /// </summary>
        [Test]
        public void ParseTest1()
        {
            var pbfStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Mapbox.Vectors.Tests.14-8801-5371.vector.pbf");

            var tile = Serializer.Deserialize<VectorTile>(pbfStream);

            // something was deserialized.
            Assert.IsNotNull(tile, "Nothing was deserialized.");

            // should contain all layers.
            Assert.IsNotNull(tile.layers, "No layers.");
            Assert.AreEqual(20, tile.layers.Count, "Not the correct number of layers.");
            foreach (var layerThatShould in new [] {
                "landuse", "waterway", "water", "barrier_line", "building",
                "landuse_overlay", "tunnel", "road", "bridge", "place_label",
                "water_label", "poi_label", "road_label", "waterway_label" })
            {
                Assert.IsTrue(tile.layers.Exists(x => x.name.Equals(layerThatShould)));
            }
        }

        [Test]
        public void CompareToOriginalJson()
        {
            var jsonTile = GetJsonTile();
            var ptfTile = GetPtfTile();

            Assert.AreEqual(ptfTile.layers.Count, jsonTile.Keys.Count);
            Assert.True(Compare(ptfTile, jsonTile));
        }

        private bool Compare(VectorTile ptfTile, Dictionary<string, FeatureCollection> jsonTile)
        {
            foreach (var ptfLayer in ptfTile.layers)
            {
                if (!jsonTile.ContainsKey(ptfLayer.name)) return false;
                var jsonLayer = jsonTile[ptfLayer.name];
                foreach (var ptfFeature in ptfLayer.features)
                {
                    var ptfProperties = ptfFeature.GetProperties(ptfLayer);
                    var jsonFeature = FindMatchingFeature(jsonLayer.Features, ptfProperties);
                    if (!ArePropertiesEqual(ptfProperties, jsonFeature.Properties)) return false;
                }
            }
            return true;
        }

        private bool ArePropertiesEqual(IDictionary<string, VectorTile.Value> ptfProperties, Dictionary<string, object> jsonProperties)
        {
            if (ptfProperties.Count != jsonProperties.Count) return false;

            foreach (var ptfProperty in ptfProperties)
            {
                if (jsonProperties[ptfProperty.Key].ToString() != ptfProperty.Value.GetValueAsString()) return false;
            }

            return true;
        }

        private static Feature FindMatchingFeature(List<Feature> features, IDictionary<string, VectorTile.Value> ptfProperties)
        {
            // The "id" field happens to be present in the sample data so Iuse it here to map the features, 
            // however these need not be there in other tile sources.
            return features.Find(f => ((long)f.Properties["id"]) == ptfProperties["id"].int_value);
        }

        private static VectorTile GetPtfTile()
        {
            const string jsonResourceName = "Mapbox.Vectors.Tests.Resources.24641.mvt";
            var jsonStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(jsonResourceName);
            return Serializer.Deserialize<VectorTile>(jsonStream);
        }

        private static Dictionary<string, FeatureCollection> GetJsonTile()
        {
            const string jsonResourceName = "Mapbox.Vectors.Tests.Resources.24641.json";
            var jsonStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(jsonResourceName);
            return JsonConvert.DeserializeObject<Dictionary<string, FeatureCollection>>(ToString(jsonStream));
        }

        private static string ToString(Stream jsonStream)
        {
            string jsonString;
            using (var reader = new StreamReader(jsonStream, Encoding.UTF8))
            {
                jsonString = reader.ReadToEnd();
            }
            return jsonString;
        }
    }
}

