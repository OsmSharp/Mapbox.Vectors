using NUnit.Framework;
using System;
using System.Reflection;
using ProtoBuf;
using mapnik.vector;
using System.Linq;

namespace Mapbox.Vectors.Tests
{
    [TestFixture()]
    public class ParseTest
    {
        /// <summary>
        /// A simple parsing test based on the js-test in: https://github.com/mapbox/vector-tile-js/blob/master/test/parse.test.js
        /// </summary>
        [Test()]
        public void ParseTest1()
        {
            var pbfStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Mapbox.Vectors.14-8801-5371.vector.pbf");

            var tile = Serializer.Deserialize<tile>(pbfStream);

            // something was deserialized.
            Assert.IsNotNull(tile, "Nothing was deserialized.");

            // should contain all layers.
            Assert.IsNotNull(tile.layers, "No layers.");
            Assert.AreEqual(14, tile.layers.Count, "Not the correct number of layers.");
            foreach (var layerThatShould in new string[] {
                "landuse", "waterway", "water", "barrier_line", "building",
                "landuse_overlay", "tunnel", "road", "bridge", "place_label",
                "water_label", "poi_label", "road_label", "waterway_label" })
            {
                Assert.IsTrue(tile.layers.Exists(x => x.name.Equals(layerThatShould)));
            }
//
//            it('should extract the tags of a feature', function() {
//                var tile = new VectorTile(data);
//
//                assert.equal(tile.layers.poi_label.length, 558);
//
//                var park = tile.layers.poi_label.feature(11);
//
//                assert.equal(park.name, 'Mauerpark');
//                assert.equal(park.type, 'Park');
//
//                // Check point geometry
//                assert.deepEqual(park.loadGeometry(), [ [ { x: 3898, y: 1731 } ] ]);
//
//                // Check line geometry
//                assert.deepEqual(tile.layers.road.feature(656).loadGeometry(), [ [ { x: 1988, y: 306 }, { x: 1808, y: 321 }, { x: 1506, y: 347 } ] ]);
//            });
        }
    }
}

