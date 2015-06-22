using System.Collections.Generic;
using System.Globalization;

namespace mapnik.vector
{
    public partial class VectorTile
    {
        public partial class Feature
        {
            public IDictionary<string, Value> GetProperties(Layer layer)
            {
                var dictionary = new Dictionary<string, Value>();

                for (int i = 0; i < _tags.Count; i += 2)
                {
                    var keyTag = _tags[i];
                    var valueTag = _tags[i + 1];
                    dictionary[layer.keys[(int)keyTag]] = layer.values[(int)valueTag];
                }

                return dictionary;
            }
        }

        public partial class Value
        {
            public string GetValueAsString()
            {
                if (string_value != "") return string_value;
                if (float_value != default(float)) return float_value.ToString(CultureInfo.InvariantCulture);;
                if (double_value != default(double)) return double_value.ToString(CultureInfo.InvariantCulture);
                if (int_value != default(long)) return int_value.ToString((CultureInfo.InvariantCulture));
                if (uint_value != default(ulong)) return uint_value.ToString(CultureInfo.InvariantCulture);
                if (sint_value != default(long)) return sint_value.ToString(CultureInfo.InvariantCulture);
                if (bool_value != default (bool)) return bool_value.ToString();
                // The response below could be incorrect because there could be an actual zero value for an int for example
                // Unfortunately the parse result does not hold the information we need. This info is in the protobuf itself
                return ""; 
            }
        }
    }
}
