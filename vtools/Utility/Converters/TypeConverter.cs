using System.Text.Json;
using System.Text.Json.Serialization;

namespace MANIFOLD.Utility {
    public class TypeConverter : JsonConverter<Type> {
        public override Type Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, Type value, JsonSerializerOptions options) {
            // This only works for simple types. Might need some upgrading in the future.
            writer.WriteStringValue(value.FullName);
        }
    }
}
