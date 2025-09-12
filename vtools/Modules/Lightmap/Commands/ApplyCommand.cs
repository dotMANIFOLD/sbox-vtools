using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

namespace MANIFOLD {
    [Command("light apply")]
    public class ApplyCommand : ICommand {
        [CommandParameter(0)]
        public required string VpkFile { get; init; }
        
        [CommandOption("work-folder", 'f', IsRequired = true)]
        public string? WorkFolder { get; init; }
        
        public ValueTask ExecuteAsync(IConsole console) {
            console.Output.WriteLine($"Working on file: {VpkFile}");
            console.Output.WriteLine($"Working folder: {WorkFolder}");
            
            File.Copy(WorkFolder + Path.GetFileNameWithoutExtension(VpkFile) + "_new.vpk", VpkFile, true);
            console.Output.WriteLine($"Apply complete");
            
            return default;
        }
    }
}
