using MANIFOLD.Animation;
using ValveResourceFormat.ResourceTypes;
using ValveResourceFormat.ResourceTypes.ModelAnimation;
using ValveResourceFormat.Serialization.KeyValues;

namespace MANIFOLD.VTools.Model {
    public class ExtractUtil {
        private class TrackPair {
            public Vector3Track posTrack;
            public RotationTrack rotTrack;
        }
        
        public static AnimationClip CreateClip(ValveResourceFormat.ResourceTypes.Model model, ValveResourceFormat.ResourceTypes.ModelAnimation.Animation anim) {
            var clip = new AnimationClip() {
                Name = anim.Name,
                FrameRate = anim.Fps,
                FrameCount = anim.FrameCount,
                Generated = true,
            };
            AddBoneTracks(anim, clip, model);
            return clip;
        }
        
        // long ass class path
        public static void AddBoneTracks(ValveResourceFormat.ResourceTypes.ModelAnimation.Animation anim, AnimationClip clip, ValveResourceFormat.ResourceTypes.Model model) {
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
                clip.Tracks.Add(pair.posTrack);
                clip.Tracks.Add(pair.rotTrack);
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
        }

        public static BoneMask CreateMask(KV3File file, string name) {
            var rootNode = file.Root.GetProperty<KVObject>("rootNode");
            var typeLists = rootNode.GetArray("children");
            var list = typeLists.First(x => x.GetStringProperty("_class") == "WeightListList");
            var masks = list.GetArray("children");
            var target = masks.First(x => x.GetStringProperty("name") == name);
            
            BoneMask mask = new BoneMask();
            mask.Name = name;
            mask.Weights = new Dictionary<string, float>();
            
            foreach (var weight in target.GetArray("weights")) {
                mask.Weights.Add(weight.GetStringProperty("bone"), weight.GetFloatProperty("weight"));
            }

            return mask;
        }
    }
}
