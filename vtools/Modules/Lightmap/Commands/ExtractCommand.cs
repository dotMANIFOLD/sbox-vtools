using System.Diagnostics;
using System.Numerics;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using MANIFOLD.Utility;
using SteamDatabase.ValvePak;
using ValveResourceFormat;
using ValveResourceFormat.IO;
using ValveResourceFormat.ResourceTypes;

namespace MANIFOLD {
    [Command("light extract", Description = "Extracts all VPK data and makes it accessible to blender.")]
    public class ExtractCommand : VCommand {
        public const float INCH_TO_METER = 39.37f;
        public const float BRIGHTNESS_TO_POWER = 200f;
        
        protected override ValueTask Run(IConsole console) {
            using var package = new Package();
            package.Read(VpkFileAbsolute);
            
            console.Output.WriteLine("Exporting map models");
            ExportMapModels(console, package);
            console.Output.WriteLine("Exporting map lights");
            ExportMapLights(console, package);
            console.Output.WriteLine("Done");

            return default;
        }

        private bool ExportMapModels(IConsole console, Package package) {
            var entry = package.Entries["vmap_c"][0];
            if (entry is null) {
                console.Output.WriteLine("No map found");
                return false;
            }
            
            package.ReadEntry(entry, out var fileData);
            using var stream = new MemoryStream(fileData);
            using var resource = new Resource();
            resource.Read(stream);

            if (resource.ResourceType != ResourceType.Map) {
                console.Output.WriteLine($"entry isnt a map??? wtf??? its a {resource.ResourceType}");
                return false;
            }

            var loader = new SboxFileLoader(package, VpkFileAbsolute, Settings.SBoxLocation, Settings.SBoxProject);
            var exporter = new GltfModelExporter(loader);

            Directory.CreateDirectory(Path.Combine(VpkWorkFolder, Paths.INTERMEDIATE_FOLDER));
            exporter.Export(resource, Path.Combine(VpkWorkFolder, Paths.INTERMEDIATE_FOLDER, Paths.INTERMEDIATE_MODELS_FILE));
            return true;
        }

        private bool ExportMapLights(IConsole console, Package package) {
            var entry = package.Entries["vents_c"][0];
            if (entry == null) {
                console.Output.WriteLine("Could not find entities");
                return false;
            }
            
            package.ReadEntry(entry, out var fileData);
            using var stream = new MemoryStream(fileData);
            using var resource = new Resource();
            resource.Read(stream);

            EntityLump lump = (EntityLump)resource.DataBlock;
            List<LightData> lights = new List<LightData>();
            
            foreach (var entity in lump.GetEntities()) {
                var className = entity.GetPropertyUnchecked<string>("classname");
                if (!className.StartsWith("light")) continue;

                LightType type = className switch {
                    "light_omni" => LightType.Point,
                    "light_spot" => LightType.Spot,
                    "light_environment" => LightType.Sun,
                    "light_area" => LightType.Area,
                    _ => LightType.Point
                };

                // var powerProp = entity.GetProperty("brightness");
                // console.Output.WriteLineAsync("brightness: " + entity.GetProperty("brightness").Type);

                Vector3 angles = entity.GetVector3Property("angles");
                float power = (float)entity.GetProperty<double>("brightness");
                if (type == LightType.Sun) {
                    angles.Y += 90;
                }
                if (type != LightType.Sun) {
                    power *= BRIGHTNESS_TO_POWER;
                }
                
                lights.Add(new LightData() {
                    type = type,
                    color = entity.GetColor32Property("color"),
                    position = Swizzle(entity.GetVector3Property("origin") / INCH_TO_METER),
                    angles = SwizzleAngle(angles),
                    power = power
                });
            }

            using var writer = File.CreateText(Path.Combine(VpkWorkFolder, Paths.INTERMEDIATE_FOLDER, Paths.INTERMEDIATE_LIGHTS_FILE));
            string json = Json.Serialize(lights);
            writer.Write(json);
            
            return true;
        }
        
        private Vector3 Swizzle(Vector3 vector) {
            return new Vector3(vector.Y, -vector.X, vector.Z);
        }

        private Vector3 SwizzleAngle(Vector3 vector) {
            return new Vector3(vector.Z, vector.X, vector.Y);
        }
    }
}
