namespace MANIFOLD.Animation {
    public class AnimationCollection {
        public const string EXTENSION = "manm";
        
        public string Skeleton { get; set; }
        
        public List<AnimationClip> Animations { get; set; } = new List<AnimationClip>();
    }
}
