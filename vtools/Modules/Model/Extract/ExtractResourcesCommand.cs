using CliFx.Attributes;
using CliFx.Infrastructure;
using MANIFOLD.AnimGraph;
using MANIFOLD.Utility;

namespace MANIFOLD.VTools.Model {
    [Command("model extract resources", Description = "Extracts everything to make graph resources")]
    public class ExtractResourcesCommand : ExtractCommand {
        [CommandOption("ext", 'e', IsRequired = false)]
        public string FileExtension { get; set; } = "json";

        protected override ValueTask Run(IConsole console) {
            AnimGraphResources resources = new AnimGraphResources();
            resources.Model = new Sandbox.Model() { Path = VmdlFileRelative };

            foreach (var anim in VmdlCompiledModel.GetEmbeddedAnimations()) {
                var clip = ExtractUtil.CreateClip(VmdlCompiledModel, anim);
                clip.Embedded = true;
                
                resources.Animations.Add(clip);
            }

            foreach (var name in ListUtil.ListAllWeightLists(VmdlResource)) {
                var mask = ExtractUtil.CreateMask(VmdlResource, name);
                mask.Model = new Sandbox.Model() { Path = VmdlFileRelative };
                mask.Embedded = true;
                
                resources.BoneMasks.Add(mask);
            }
            
            string outputDir = OutputDirectory ?? Path.GetDirectoryName(VmdlFileAbsolute);
            string fileName = Path.GetFileNameWithoutExtension(VmdlFileAbsolute);
            string extension = FileExtension;
            var json = Json.SerializeAsObject(resources);
            string finalPath = Path.Combine(outputDir, $"{fileName}.{extension}");
            console.Output.WriteLine($"Output path: {finalPath}");
            File.WriteAllText(finalPath, json.ToString());
            return default;
        }
    }
}
