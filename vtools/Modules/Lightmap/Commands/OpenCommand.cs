using System.ComponentModel;
using System.Diagnostics;
using CliFx.Attributes;
using CliFx.Infrastructure;

namespace MANIFOLD {
    [Command("light open", Description = "Opens blender with lightmapper tools.")]
    public class OpenCommand : VCommand {
        protected override ValueTask Run(IConsole console) {
            string arguments = $"--python-expr \"{CreateLoaderString()}\"";
            
            console.Output.WriteLine($"Running blender with arguments: {arguments}");
            
            ProcessStartInfo info = new ProcessStartInfo() {
                FileName = Settings.BlenderExecutable,
                Arguments = arguments
            };
            using var process = Process.Start(info);
            return default;
        }
        
        private string CreateLoaderString() {
            string[] arr = [
                "import sys;sys.path.append",
                $"sys.path.append({Path.Combine(WorkFolder, Paths.TOOLS_FOLDER)})",
                $"sys.path.append({Path.Combine(VpkWorkFolder, Paths.INTERMEDIATE_FOLDER)})",
                $"import {Paths.PYTHON_MODULE_FOLDER}"
            ];
            return string.Join(";", arr);
        }
    }
}
