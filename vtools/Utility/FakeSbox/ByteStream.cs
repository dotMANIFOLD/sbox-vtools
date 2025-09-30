using System.Runtime.InteropServices;

namespace Sandbox {
    public struct ByteStream {
        private byte[] buffer;
        private MemoryStream stream;
        
        public int Length => buffer.Length;
        
        public static ByteStream Create(int size) {
            ByteStream stream = default;
            stream.buffer = new byte[size];
            stream.stream = new MemoryStream(stream.buffer, true);
            return stream;
        }

        public static ByteStream CreateReader(ReadOnlySpan<byte> data) {
            ByteStream stream = default;
            stream.buffer = data.ToArray();
            stream.stream = new MemoryStream(stream.buffer, false);
            return stream;
        }

        public T Read<T>() where T : struct {
            int size = Marshal.SizeOf<T>();
            var span = buffer.AsSpan((int)stream.Position, size);
            T value = MemoryMarshal.Read<T>(span);

            stream.Position += size;
            return value;
        }
        
        public int Read(byte[] buffer, int offset, int count) {
            return stream.Read(buffer, offset, count);
        }

        public void Write<T>(T value) where T : struct {
            int size = Marshal.SizeOf<T>();
            var span = buffer.AsSpan((int)stream.Position, size);
            MemoryMarshal.Write(span, value);
            stream.Position += size;
        }
        
        public void Write(byte[] buffer, int offset, int count) {
            stream.Write(buffer, offset, count);
        }

        public byte[] ToArray() {
            return stream.ToArray();
        }
    }
}
