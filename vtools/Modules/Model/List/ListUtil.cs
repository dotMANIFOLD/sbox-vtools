using ValveResourceFormat.Serialization.KeyValues;

namespace MANIFOLD.VTools.Model {
    public class ListUtil {
        public static IEnumerable<string> ListAllWeightLists(KV3File file) {
            var rootNode = file.Root.GetProperty<KVObject>("rootNode");
            var typeLists = rootNode.GetArray("children");
            var list = typeLists.First(x => x.GetStringProperty("_class") == "WeightListList");
            var masks = list.GetArray("children");
            return masks.Select(x => x.GetStringProperty("name"));
        }
    }
}
