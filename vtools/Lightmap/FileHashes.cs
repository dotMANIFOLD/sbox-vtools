using System.Security.Cryptography;
using Newtonsoft.Json;

namespace MANIFOLD {
    public class FileHashes {
        public class JSONData {
            public string original;
            public string modified;
        }

        private readonly string filePath;
        
        public string Original { get; set; }
        public string Modified { get; set; }

        public FileHashes(string file, bool read = true) {
            if (read && File.Exists(file)) {
                string json = File.ReadAllText(file);
                JSONData data = JsonConvert.DeserializeObject<JSONData>(json);
                Original = data.original;
                Modified = data.modified;
            }
            filePath = file;
        }

        public void CreateOriginalHash(Stream stream) {
            Original = CreateHash(stream);
        }

        public void CreateModifiedHash(Stream stream) {
            Modified = CreateHash(stream);
        }
        
        public string CreateHash(Stream stream) {
            using var hasher = MD5.Create();
            var hash = hasher.ComputeHash(stream);
            return Convert.ToHexString(hash);
        }

        public bool MatchesAny(string hash) {
            return hash == Original || hash == Modified;
        }

        public bool MatchesAny(Stream stream) {
            var hash = CreateHash(stream);
            return MatchesAny(hash);
        }
        
        public void Save() {
            JSONData data = new JSONData();
            data.original = Original;
            data.modified = Modified;
            
            File.WriteAllText(filePath, JsonConvert.SerializeObject(data));
        }
    }
}
