using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using SteamDatabase.ValvePak;
using ValveResourceFormat;
using ValveResourceFormat.ResourceTypes;

namespace MANIFOLD {
    [Command("light replace")]
    public class ReplaceCommand : ICommand {
        public const string COMPILED_FOLDER = "compiled";
        public const string DUMMY_ORG = "root";
        public const string BAKES_FOLDER = "SimpleBake_Bakes";
        public const string LIGHTMAP_FILE = "MergedBake_Bake1_CyclesBake_DIFFUSE";
        
        [CommandParameter(0)]
        public required string VpkFile { get; init; }
        
        [CommandOption("work-folder", 'f')]
        public string? WorkFolder { get; init; }
        
        public ValueTask ExecuteAsync(IConsole console) {
            console.Output.WriteLine($"Working on file: {VpkFile}");
            console.Output.WriteLine($"Working folder: {WorkFolder}");
            
            using var package = new Package();
            package.Read(Path.Combine(WorkFolder, Path.GetFileNameWithoutExtension(VpkFile) + "_backup.vpk"));
            
            var entry = package.Entries["vtex_c"].FirstOrDefault(x => x.FileName == "irradiance");
            if (entry is null) {
                console.Output.WriteLine("No lightmap found in VPK file");
                return default;
            }

            string compiledPath = Path.Combine(WorkFolder, COMPILED_FOLDER, DUMMY_ORG, "tex.vtex_c");
            if (!File.Exists(compiledPath)) {
                console.Error.WriteLine("No replacement found");
                return default;
            }
            
            byte[] newData = File.ReadAllBytes(compiledPath);
            
            string fullPath = entry.GetFullPath();
            console.Output.WriteLine(fullPath);
            
            package.RemoveFile(entry);
            package.AddFile(fullPath, newData);
            
            package.Write(WorkFolder + Path.GetFileNameWithoutExtension(VpkFile) + "_new.vpk");
            return default;
        }
    }
}
