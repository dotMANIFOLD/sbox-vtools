using CliFx.Attributes;
using CliFx.Infrastructure;
using MANIFOLD.Utility;

namespace MANIFOLD.VTools.Model {
    [Command("model extract anim", Description = "Extract a single animation.")]
    public class ExtractAnimCommand : ExtractCommand {
        [CommandParameter(1)]
        public required string AnimationName { get; init; }
        [CommandOption("ext", 'e', IsRequired = false)]
        public string FileExtension { get; init; } = "json";
        
        protected override ValueTask Run(IConsole console) {
            var anims = VmdlCompiledModel.GetEmbeddedAnimations();
            
            var targetAnim = anims.FirstOrDefault(x => x.Name == AnimationName);
            if (targetAnim == null) {
                console.Error.WriteLine($"Animation \"{AnimationName}\" not found");
                return default;
            }
            var clip = ExtractUtil.CreateClip(VmdlCompiledModel, targetAnim);

            string outputDir = OutputDirectory ?? Path.GetDirectoryName(VmdlFileAbsolute);
            string fileName = AnimationName.Replace("@", "");
            string extension = FileExtension;
            var json = Json.SerializeAsObject(clip);
            string finalPath = Path.Combine(outputDir, $"{fileName}.{extension}");
            console.Output.WriteLine($"Output path: {finalPath}");
            File.WriteAllText(finalPath, json.ToString());
            
            return default;
        }
    }
}
