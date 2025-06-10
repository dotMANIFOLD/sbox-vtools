using System.Numerics;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MANIFOLD {
    public enum LightType {
        Point,
        Spot,
        Sun,
        Area
    }
    
    public struct LightData {
        public LightType type;
        public Vector3 color;
        public Vector3 position;
        public Vector3 angles;
        public float power;
    }
}
