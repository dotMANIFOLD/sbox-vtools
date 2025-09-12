using System.Text.Json;
using System.Text.Json.Serialization;
using Sandbox;

namespace MANIFOLD.Utility {
    public class GameResourceConverter : JsonConverter<GameResource> {
        public override bool CanConvert(Type typeToConvert) {
            return typeToConvert.IsAssignableTo(typeof(GameResource));
        }

        public override GameResource Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, GameResource value, JsonSerializerOptions options) {
            if (value.Embedded) {
                writer.WriteStartObject();
                writer.WriteString("$compiler", "embed");
                writer.WriteNull("$source");
                writer.WritePropertyName("data");
                writer.WriteRawValue(Json.SerializeAsObject(value).ToString());
                writer.WriteEndObject();
            } else {
                writer.WriteStringValue(value.Path);
            }
        }
    }
}
