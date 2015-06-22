using System.Collections.Generic;
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

            var tile = Serializer.Deserialize<tile>(pbfStream);

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
            var jsonTile = GetJsonVectorTile();
            var protobufTile = GetProtobufVectorTile();

            Assert.AreEqual(protobufTile.layers.Count, jsonTile.Keys.Count);
            Assert.True(CompareLayers(protobufTile, jsonTile));
        }

        private bool CompareLayers(tile protobufTile, Dictionary<string, FeatureCollection> jsonTile)
        {
            foreach (var layer in protobufTile.layers)
            {
                if (!jsonTile.ContainsKey(layer.name)) return false;
            }
            return true;
        }

        private static tile GetProtobufVectorTile()
        {
            const string jsonResourceName = "Mapbox.Vectors.Tests.Resources.24641.mvt";
            var jsonStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(jsonResourceName);
            return Serializer.Deserialize<tile>(jsonStream);
        }

        private static Dictionary<string, FeatureCollection> GetJsonVectorTile()
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

