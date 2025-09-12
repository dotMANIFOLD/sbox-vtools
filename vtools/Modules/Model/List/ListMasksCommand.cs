using System.Text.Json;
using CliFx.Attributes;
using CliFx.Infrastructure;
using MANIFOLD.Utility;
using ValveResourceFormat.Serialization.KeyValues;

namespace MANIFOLD.VTools.Model {
    [Command("model list mask", Description = "Lists all weight lists in the model")]
    public class ListMasksCommand : WorkCommand {
        [CommandParameter(0)]
        public required string VmdlFileRelative { get; init; }
        [CommandOption("format", 'f', IsRequired = false)]
        public string Format { get; init; } = "human";

        public string VmdlFileAbsolute { get; private set; }
        
        protected override bool Setup(IConsole console) {
            if (!base.Setup(console)) {
                return false;
            }
            
            VmdlFileAbsolute = Path.Combine(Settings.SBoxProject, VmdlFileRelative);
            console.Output.WriteLine($"Target VMDL File: {VmdlFileRelative}");
            
            if (!File.Exists(VmdlFileAbsolute)) {
                console.Error.WriteLine("No VMDL file found.");
                return false;
            }

            return true;
        }

        protected override ValueTask Run(IConsole console) {
            using var fileStream = File.OpenRead(VmdlFileAbsolute);
            var kvFile = KeyValues3.ParseKVFile(fileStream);
            var names = ListUtil.ListAllWeightLists(kvFile);

            switch (Format) {
                case "human": {
                    console.Output.WriteLine("All bone masks:");
                    foreach (var name in names) {
                        console.Output.WriteLine(" - " + name);
                    }
                    break;
                }
                case "json": {
                    console.Output.WriteLine(JsonSerializer.Serialize(names.ToArray()));
                    break;
                }
            }
            
            return default;
        }
    }
}
