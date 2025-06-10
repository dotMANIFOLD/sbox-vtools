using System.Text.Json;
using CliFx.Attributes;
using CliFx.Infrastructure;
using ValveResourceFormat;
using ValveResourceFormat.ResourceTypes;
using ValveResourceFormat.ResourceTypes.ModelAnimation;
using MANIFOLD.Animation;
using MANIFOLD.Util;

namespace MANIFOLD.AnimExtract {
    [Command("anim extract")]
    public class ExtractCommand : WCommand {
        public class TrackPair {
            public Vector3Track posTrack;
            public RotationTrack rotTrack;
        }
        
        [CommandParameter(0)]
        public required string VmdlFileRelative { get; init; }
        [CommandOption("output-dir", 'o', IsRequired = false)]
        public string OutputDirectory { get; init; }
        [CommandOption("extension", 'e', IsRequired = false)]
        public string FileExtension { get; init; }
        
        public string VmdlFileAbsolute { get; set; }
        
        protected override bool Setup(IConsole console) {
            if (!base.Setup(console)) {
                return false;
            }
            
            VmdlFileAbsolute = Path.Combine(Settings.sboxProject, VmdlFileRelative);
            console.Output.WriteLine($"Target VMDL File: {VmdlFileRelative}");

            if (!File.Exists(VmdlFileAbsolute)) {
                console.Output.WriteLine("No VMDL file found.");
                return false;
            }
            
            return true;
        }

        protected override ValueTask Run(IConsole console) {
            using var stream = File.OpenRead(VmdlFileAbsolute);
            var resource = new Resource();
            resource.Read(stream);

            if (resource.ResourceType != ResourceType.Model) {
                console.Output.WriteLine("File is not a model");
                return default;
            }

            var model = (Model)resource.DataBlock;
            var anims = model.GetEmbeddedAnimations();
            var collection = new AnimationCollection();

            foreach (var anim in anims) {
                var mAnim = new AnimationClip() {
                    Name = anim.Name,
                    FrameRate = anim.Fps,
                    FrameCount = anim.FrameCount,
                };
                var pairs = new TrackPair[model.Skeleton.Bones.Length];
                for (int i = 0; i < pairs.Length; i++) {
                    var bone = model.Skeleton.Bones[i];
                    var pair = new TrackPair();
                    pairs[i] = pair;
                    
                    pair.posTrack = new Vector3Track() {
                        Name = "LocalPosition",
                        TargetBone = bone.Name
                    };
                    pair.rotTrack = new RotationTrack() {
                        Name = "LocalRotation",
                        TargetBone = bone.Name
                    };
                    mAnim.Tracks.Add(pair.posTrack);
                    mAnim.Tracks.Add(pair.rotTrack);
                }
                
                // dont be fooled, GetAnimationMatrices looks correct but it gives us model space matrices (i think)
                var cache = new AnimationFrameCache(model.Skeleton, model.FlexControllers);

                for (int i = 0; i < anim.FrameCount; i++) {
                    var frame = cache.GetFrame(anim, i);
                    
                    for (int boneIndex = 0; boneIndex < frame.Bones.Length; boneIndex++) {
                        var transform = frame.Bones[boneIndex];
                        pairs[boneIndex].posTrack.KeyFrames.Add(i, transform.Position);
                        pairs[boneIndex].rotTrack.KeyFrames.Add(i, transform.Angle);
                    }
                }
                
                collection.Animations.Add(mAnim);
            }

            string outputDir = OutputDirectory ?? Path.GetDirectoryName(VmdlFileAbsolute);
            string fileName = Path.GetFileNameWithoutExtension(VmdlFileAbsolute);
            string extension = FileExtension ?? AnimationCollection.EXTENSION;
            string json = Json.Serialize(collection);
            string finalPath = Path.Combine(outputDir, $"{fileName}.{extension}");
            console.Output.WriteLine($"Output path: {finalPath}");
            File.WriteAllText(finalPath, json);
            
            return default;
        }
    }
}
