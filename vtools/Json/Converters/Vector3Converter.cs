using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MANIFOLD.Util {
    public class Vector3Converter : JsonConverter<Vector3> {
        public override Vector3 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, Vector3 value, JsonSerializerOptions options) {
            writer.WriteStringValue(string.Join(',', value.X, value.Y, value.Z));
        }
    }
}
