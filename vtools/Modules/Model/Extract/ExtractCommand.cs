using System.Text.Json;
using CliFx.Attributes;
using CliFx.Infrastructure;
using ValveResourceFormat;
using ValveResourceFormat.ResourceTypes;
using ValveResourceFormat.ResourceTypes.ModelAnimation;
using MANIFOLD.Animation;
using MANIFOLD.Utility;
using ValveResourceFormat.Serialization.KeyValues;

namespace MANIFOLD.VTools.Model {
    public abstract class ExtractCommand : WorkCommand {
        [CommandParameter(0)]
        public required string VmdlFileRelative { get; init; }
        [CommandOption("output-dir", 'o', IsRequired = false)]
        public string OutputDirectory { get; init; }

        public string VmdlFileAbsolute { get; private set; }
        public string VmdlCompiledFileAbsolute { get; private set; }
        public KV3File VmdlResource { get; private set; }
        public Resource VmdlCompiledResource { get; private set; }
        public ValveResourceFormat.ResourceTypes.Model VmdlCompiledModel { get; private set; }
        
        protected override bool Setup(IConsole console) {
            if (!base.Setup(console)) {
                return false;
            }
            
            VmdlFileAbsolute = Path.Combine(Settings.SBoxProject, VmdlFileRelative);
            VmdlCompiledFileAbsolute = VmdlFileAbsolute + "_c";
            console.Output.WriteLine($"Target VMDL File: {VmdlFileRelative}");

            if (!File.Exists(VmdlFileAbsolute)) {
                console.Error.WriteLine("No VMDL file found.");
                return false;
            }
            if (!File.Exists(VmdlCompiledFileAbsolute)) {
                console.Error.WriteLine("No compiled VMDL file found.");
                return false;
            }

            using var vmdlStream = File.OpenRead(VmdlFileAbsolute);
            VmdlResource = KeyValues3.ParseKVFile(vmdlStream);
            
            using var compiledStream = File.OpenRead(VmdlCompiledFileAbsolute);
            VmdlCompiledResource = new Resource();
            VmdlCompiledResource.Read(compiledStream);

            if (VmdlCompiledResource.ResourceType != ResourceType.Model) {
                console.Error.WriteLine("File is not a model!");
                return false;
            }

            VmdlCompiledModel = (ValveResourceFormat.ResourceTypes.Model)VmdlCompiledResource.DataBlock;
            
            return true;
        }
    }
}
