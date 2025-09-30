namespace Sandbox {
    public struct Vector4 {
        public System.Numerics.Vector4 real;

        public float x {
            get => real.X;
            set => real.X = value;
        }

        public float y {
            get => real.Y;
            set => real.Y = value;
        }

        public float z {
            get => real.Z;
            set => real.Z = value;
        }

        public float w {
            get => real.W;
            set => real.W = value;
        }

        public Vector4(float x, float y, float z, float w) {
            real = new System.Numerics.Vector4(x, y, z, w);
        }
        
        public static implicit operator Vector4(System.Numerics.Vector4 q) => new Vector4() { real = q };
    }
}
