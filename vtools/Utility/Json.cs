using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Sandbox;

namespace MANIFOLD.Utility {
    /// <summary>
    /// S&box Json class copycat. Makes copying things from s&box easier.
    /// </summary>
    public static class Json {
        public static JsonSerializerOptions options;
        public static GameResourceConverter resourceConverter;

        static Json() {
            options = new JsonSerializerOptions(JsonSerializerOptions.Default);
            options.WriteIndented = true;
            options.PropertyNameCaseInsensitive = true;
            options.NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.AllowNamedFloatingPointLiterals;
            options.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
            options.ReadCommentHandling = JsonCommentHandling.Skip;
            options.MaxDepth = 512;
            options.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            
            resourceConverter = new GameResourceConverter();
            options.Converters.Add(resourceConverter);
            options.Converters.Add(new JsonStringEnumConverter());
            options.Converters.Add(new TypeConverter());
            options.Converters.Add(new Vector3Converter());
            options.Converters.Add(new RotationConverter());
        }

        public static string Serialize(object obj) {
            return JsonSerializer.Serialize(obj, options);
        }

        public static JsonObject SerializeAsObject(object target) {
            var type = target.GetType();
            var obj = new JsonObject();
            foreach (var prop in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {
                bool hasIgnoreAttribute = prop.GetCustomAttribute(typeof(JsonIgnoreAttribute)) != null;
                bool hasIncludeAttribute = prop.GetCustomAttribute(typeof(JsonIncludeAttribute)) != null;
                if (prop.CanRead && prop.GetMethod.GetParameters().Length == 0 && !hasIgnoreAttribute && (prop.GetMethod.IsPublic || hasIncludeAttribute)) {
                    JsonNode node = JsonSerializer.SerializeToNode(prop.GetValue(target), prop.PropertyType, options);
                    string name = prop.Name;

                    JsonPropertyNameAttribute nameAttribute = prop.GetCustomAttribute<JsonPropertyNameAttribute>();
                    if (nameAttribute != null) name = nameAttribute.Name;

                    obj.Add(name, node);
                }
            }
            return obj;
        }

        public static T Deserialize<T>(string json) {
            return JsonSerializer.Deserialize<T>(json, options);
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

        public static JsonNode ToNode(object obj, Type type) {
            return JsonSerializer.SerializeToNode(obj, type, options);
        }

        public static T FromNode<T>(JsonNode node) {
            if (node == null) return default;
            return node.Deserialize<T>(options);
        }

        public static object FromNode(JsonNode node, Type type) {
            if (node == null) return default;
            return node.Deserialize(type, options);
        }
    }
}
