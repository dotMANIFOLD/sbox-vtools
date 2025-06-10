using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace MANIFOLD.Util {
    /// <summary>
    /// S&box Json class copycat. Makes copying things from s&box easier.
    /// </summary>
    public static class Json {
        public static JsonSerializerOptions options;

        static Json() {
            options = new JsonSerializerOptions(JsonSerializerOptions.Default);
            options.WriteIndented = true;
            options.PropertyNameCaseInsensitive = true;
            options.NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.AllowNamedFloatingPointLiterals;
            options.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
            options.ReadCommentHandling = JsonCommentHandling.Skip;
            options.MaxDepth = 512;
            options.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            
            options.Converters.Add(new JsonStringEnumConverter());
            options.Converters.Add(new TypeConverter());
            options.Converters.Add(new Vector3Converter());
            options.Converters.Add(new RotationConverter());
        }

        public static string Serialize(object obj) {
            return JsonSerializer.Serialize(obj, options);
        }

        public static object Deserialize(string source, Type type) {
            return JsonSerializer.Deserialize(source, type, options);
        }
        
        public static JsonNode ToNode(object obj) {
            if (obj is Type) { // this is a dumb workaround
                return JsonSerializer.SerializeToNode(obj, typeof(Type), options);
            }
            return JsonSerializer.SerializeToNode(obj, options);
        }

        public static T FromNode<T>(JsonNode node) {
            if (node == null) return default;
            return node.Deserialize<T>(options);
        }
    }
}
