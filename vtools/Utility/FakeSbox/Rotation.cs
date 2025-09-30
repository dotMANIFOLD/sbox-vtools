namespace Sandbox {
    public struct Rotation {
        public System.Numerics.Quaternion real;

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

        public Rotation(float x, float y, float z, float w) {
            real = new System.Numerics.Quaternion(x, y, z, w);
        }
        
        public static implicit operator Rotation(System.Numerics.Quaternion q) => new Rotation() { real = q };
    }
}
