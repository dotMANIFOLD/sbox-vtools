using CliFx.Attributes;
using CliFx.Infrastructure;
using MANIFOLD.Animation;
using MANIFOLD.Utility;
using Sandbox;
using ValveResourceFormat.Serialization.KeyValues;

namespace MANIFOLD.VTools.Model {
    [Command("model extract mask", Description = "Extract a single bone mask.")]
    public class ExtractMaskCommand : ExtractCommand {
        [CommandParameter(1)]
        public required string MaskName { get; init; }
        [CommandOption("ext", 'e', IsRequired = false)]
        public string FileExtension { get; init; } = "json";
        
        protected override ValueTask Run(IConsole console) {
            BoneMask mask = ExtractUtil.CreateMask(VmdlResource, MaskName);
            mask.Model = new Sandbox.Model() { Path = VmdlFileRelative };
            
            string outputDir = OutputDirectory ?? Path.GetDirectoryName(VmdlFileAbsolute);
            string fileName = MaskName;
            string extension = FileExtension;
            var json = Json.SerializeAsObject(mask);
            string finalPath = Path.Combine(outputDir, $"{fileName}.{extension}");
            console.Output.WriteLine($"Output path: {finalPath}");
            File.WriteAllText(finalPath, json.ToString());
            
            return default;
        }
    }
}
