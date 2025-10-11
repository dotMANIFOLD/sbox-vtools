using MANIFOLD.Animation;
using ValveResourceFormat.ResourceTypes;
using ValveResourceFormat.ResourceTypes.ModelAnimation;
using ValveResourceFormat.Serialization.KeyValues;
using SoundEvent = MANIFOLD.Animation.SoundEvent;

namespace MANIFOLD.VTools.Model {
    public class ExtractUtil {
        private class TrackPair {
            public CompressedVector3Track posTrack;
            public CompressedRotationTrack rotTrack;
        }
        
        public static AnimationClip CreateClip(ValveResourceFormat.ResourceTypes.Model model, ValveResourceFormat.ResourceTypes.ModelAnimation.Animation anim) {
            var clip = new AnimationClip() {
                Name = anim.Name,
                FrameRate = anim.Fps,
                FrameCount = anim.FrameCount,
                Generated = true,
            };
            AddBoneTracks(anim, clip, model);
            AddEventTracks(anim, clip);
            clip.CompressData();
            return clip;
        }
        
        // long ass class path
        public static void AddBoneTracks(ValveResourceFormat.ResourceTypes.ModelAnimation.Animation anim, AnimationClip clip, ValveResourceFormat.ResourceTypes.Model model) {
            var pairs = new TrackPair[model.Skeleton.Bones.Length];
            for (int i = 0; i < pairs.Length; i++) {
                var bone = model.Skeleton.Bones[i];
                var pair = new TrackPair();
                pairs[i] = pair;
                    
                pair.posTrack = new CompressedVector3Track() {
                    Name = "LocalPosition",
                    TargetBone = bone.Name,
                    Data = new Vector3[anim.FrameCount],
                };
                pair.rotTrack = new CompressedRotationTrack() {
                    Name = "LocalRotation",
                    TargetBone = bone.Name,
                    Data = new Rotation[anim.FrameCount],
                };
                clip.BoneTracks.Add(pair.posTrack);
                clip.BoneTracks.Add(pair.rotTrack);
            }
                
            // dont be fooled, GetAnimationMatrices looks correct but it gives us model space matrices (i think)
            var cache = new AnimationFrameCache(model.Skeleton, model.FlexControllers);

            for (int i = 0; i < anim.FrameCount; i++) {
                var frame = cache.GetFrame(anim, i);
                    
                for (int boneIndex = 0; boneIndex < frame.Bones.Length; boneIndex++) {
                    var transform = frame.Bones[boneIndex];
                    pairs[boneIndex].posTrack.Data[i] = transform.Position;
                    pairs[boneIndex].rotTrack.Data[i] = transform.Angle;
                }
            }
        }

        public static void AddEventTracks(ValveResourceFormat.ResourceTypes.ModelAnimation.Animation anim, AnimationClip clip) {
            Dictionary<string, EventTrack> tracks = new Dictionary<string, EventTrack>();

            EventTrack<T> GetTrack<T>(string id) where T : class, IEvent {
                if (!tracks.TryGetValue(id, out EventTrack track)) {
                    track = new EventTrack<T>();
                    track.Name = id;
                    tracks.Add(id, track);
                }
                return (EventTrack<T>)track;
            }
            
            foreach (var evt in anim.Events) {
                switch (evt.Name) {
                    // GENERIC
                    case "AE_GENERIC_EVENT": {
                        var track = GetTrack<GenericEvent>("Generic");
                        var vectorData = evt.EventData.GetArray<double>("Vector");
                        var vector = new Vector3((float)vectorData[0], (float)vectorData[1], (float)vectorData[2]);
                        var data = new GenericEvent() {
                            Type = evt.EventData.GetStringProperty("TypeName"),
                            Int = evt.EventData.GetInt32Property("Int"),
                            Float = evt.EventData.GetFloatProperty("Float"),
                            Vector = vector,
                            String = evt.EventData.GetStringProperty("StringData"),
                        };
                        
                        track.Events.Add(evt.Frame, data);
                        break;
                    }
                    
                    // FOOTSTEPS
                    case "AE_FOOTSTEP": {
                        var track = GetTrack<FootstepEvent>("Footstep");
                        var data = new FootstepEvent() {
                            Foot = (FootstepEvent.FootSide)evt.EventData.GetInt32Property("Foot"),
                            Attachment = evt.EventData.GetStringProperty("Attachment"),
                            Volume = evt.EventData.GetFloatProperty("Volume"),
                        };
                        
                        track.Events.Add(evt.Frame, data);
                        break;
                    }
                    
                    // BODY GROUPS
                    case "AE_CL_BODYGROUP_SET_VALUE": {
                        var track = GetTrack<BodyGroupEvent>("BodyGroup");
                        var data = new BodyGroupEvent() {
                            BodyGroup = evt.EventData.GetStringProperty("bodygroup"),
                            Value = evt.EventData.GetInt32Property("value")
                        };
                        
                        track.Events.Add(evt.Frame, data);
                        break;
                    }
                    case "AE_SV_BODYGROUP_SET_VALUE": {
                        var track = GetTrack<BodyGroupEvent>("BodyGroup");
                        var data = new BodyGroupEvent() {
                            BodyGroup = evt.EventData.GetStringProperty("bodygroup"),
                            Value = evt.EventData.GetInt32Property("value")
                        };
                        
                        track.Events.Add(evt.Frame, data);
                        break;
                    }
                    
                    // SOUND
                    case "AE_CL_PLAYSOUND": {
                        var track = GetTrack<SoundEvent>("Sound");
                        var data = new SoundEvent() {
                            Event = new Sandbox.SoundEvent() { Path = evt.EventData.GetStringProperty("name") }
                        };
                        
                        track.Events.Add(evt.Frame, data);
                        break;
                    }
                    case "AE_CL_PLAYSOUND_ATTACHMENT": {
                        var track = GetTrack<SoundEvent>("Sound");
                        var data = new SoundEvent() {
                            Event = new Sandbox.SoundEvent() { Path = evt.EventData.GetStringProperty("name") },
                            Attachment = evt.EventData.GetStringProperty("attachment")
                        };
                        
                        track.Events.Add(evt.Frame, data);
                        break;
                    }
                    case "AE_SV_PLAYSOUND": {
                        var track = GetTrack<SoundEvent>("Sound");
                        var data = new SoundEvent() {
                            Event = new Sandbox.SoundEvent() { Path = evt.EventData.GetStringProperty("name") }
                        };
                        
                        track.Events.Add(evt.Frame, data);
                        break;
                    }
                }
            }

            foreach (var track in tracks.Values) {
                clip.EventTracks.Add(track);
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
