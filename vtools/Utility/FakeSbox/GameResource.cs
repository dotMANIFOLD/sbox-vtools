using System.Text.Json.Serialization;

namespace Sandbox {
    public class GameResource {
        [JsonIgnore]
        public string Path { get; set; }
        [JsonIgnore]
        public bool Embedded { get; set; }
        
        protected virtual Bitmap CreateAssetTypeIcon(int width, int height) {
            return null;
        }

        protected Bitmap CreateSimpleAssetTypeIcon(string name, int width, int height, string backgroundColor = null, string foregroundColor = null) {
            return null;
        }
    }
}
