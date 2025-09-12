namespace Sandbox {
    public class AssetTypeAttribute : Attribute {
        public string Name { get; set; }
        public string Category { get; set; }
        public string Extension { get; set; }
    }
}
