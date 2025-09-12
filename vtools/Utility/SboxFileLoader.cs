using System.Diagnostics;
using SteamDatabase.ValvePak;
using ValveResourceFormat;
using ValveResourceFormat.CompiledShader;
using ValveResourceFormat.IO;
using ValveResourceFormat.ResourceTypes;

namespace MANIFOLD {
    public class SboxFileLoader : GameFileLoader {
        public static readonly string[] compiledShaders = [
            "shaders/complex.shader",
        ];
        public static readonly string[] providedShaders = [
            "shaders/complex.shader",
            "shaders/glass.shader",
        ];

        private string sboxLocation;
        
        public SboxFileLoader(Package currentPackage, string currentFileName, string sbox, string project) : base(currentPackage, currentFileName) {
            sboxLocation = sbox;
            AddDiskPathToSearch(sboxLocation + "core/");
            AddDiskPathToSearch(sboxLocation + "addons/base/Assets/");
            AddDiskPathToSearch(project);
        }

        protected override ShaderCollection LoadShaderFromDisk(string shaderName) {
            string copy = shaderName;
            if (providedShaders.Contains(shaderName)) {
                copy += "_c";
            }
            if (providedShaders.Contains(shaderName)) {
                Console.WriteLine("Loading provided shader: " + shaderName);
                var shaderResource = LoadFile(copy);
                // Resource shaderResource = new Resource();
                // shaderResource.Read(Path.Combine(sboxLocation, shaderName + "_c"));
                
                Debug.Assert(shaderResource.ResourceType == ResourceType.Shader);
                // Console.WriteLine("Resource is a shader");
                
                var shader = (SboxShader)shaderResource.GetBlockByType(BlockType.SPRV);
                return shader.Shaders;
            }
            return base.LoadShaderFromDisk(copy);
        }
    }
}
