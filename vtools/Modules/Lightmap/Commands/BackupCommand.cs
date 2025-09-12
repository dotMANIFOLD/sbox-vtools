using System.Security.Cryptography;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

namespace MANIFOLD {
    [Command("light backup", Description = "Backup a VPK file. Required before anything else can execute.")]
    public class BackupCommand : VCommand {
        protected override ValueTask Run(IConsole console) {
            string destination = Path.Combine(VpkWorkFolder, "backup.vpk");
            
            Directory.CreateDirectory(VpkWorkFolder);
            File.Copy(VpkFileAbsolute, Path.Combine(VpkWorkFolder, "backup.vpk"), true);
            console.Output.WriteLine($"VPK copied.");
            
            console.Output.WriteLine("Calculating hashes");
            using var stream = File.Open(destination, FileMode.Open);
            
            FileHashes hashes = new FileHashes(Path.Combine(VpkWorkFolder, "hash.json"), false);
            hashes.CreateOriginalHash(stream);
            hashes.Save();
            console.Output.WriteLine($"Hash saved.");
            
            return default;
        }
    }
}
