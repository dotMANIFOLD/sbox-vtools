namespace Sandbox {
    public struct Vector3 {
        private System.Numerics.Vector3 real;

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

        public Vector3(float x, float y, float z) {
            real = new System.Numerics.Vector3(x, y, z);
        }

        public static implicit operator Vector3(System.Numerics.Vector3 v) => new Vector3() { real = v };

        public static Vector3 operator *(Vector3 vec, float scalar) => vec.real * scalar;
    }
}
