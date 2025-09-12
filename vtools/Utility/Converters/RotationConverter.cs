using System.Text.Json;
using System.Text.Json.Serialization;

namespace MANIFOLD.Utility {
    public class RotationConverter : JsonConverter<Rotation> {

        public override Rotation Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, Rotation value, JsonSerializerOptions options) {
            writer.WriteStringValue(string.Join(',', value.X, value.Y, value.Z, value.W));
        }
    }
}
