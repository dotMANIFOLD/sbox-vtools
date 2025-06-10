using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MANIFOLD.Util {
    public class RotationConverter : JsonConverter<Quaternion> {

        public override Quaternion Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, Quaternion value, JsonSerializerOptions options) {
            writer.WriteStringValue(string.Join(',', value.X, value.Y, value.Z, value.W));
        }
    }
}
