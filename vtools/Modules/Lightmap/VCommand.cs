using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

namespace MANIFOLD {
    public abstract class VCommand : WorkCommand {
        
        [CommandParameter(0)]
        public required string VpkFileRelative { get; init; }
        
        // FILLED IN
        public string VpkFileAbsolute { get; set; }
        public string VpkWorkFolder { get; set; }

        protected override bool Setup(IConsole console) {
            if (!base.Setup(console)) {
                return false;
            }
            
            VpkFileAbsolute = Path.Combine(Settings.SBoxProject, VpkFileRelative);
            console.Output.WriteLine($"Target VPK File: {VpkFileRelative}");

            if (!File.Exists(VpkFileAbsolute)) {
                console.Output.WriteLine("No VPK file found.");
                return false;
            }
            
            VpkWorkFolder = Path.Combine(WorkFolder, Paths.MAPS_FOLDER, Path.GetFileNameWithoutExtension(VpkFileRelative));
            Directory.CreateDirectory(VpkWorkFolder);
            return true;
        }
    }
}
