using UnityEditor;

namespace TicTocGuardians.Scripts.Assets
{
    public class PointArrayDataAsset : LevelDataAsset
    {
        public static LevelDataAsset Create(string name, Vector3DataAsset[] value)
        {
            var instance = CreateInstance<PointArrayDataAsset>();
            EditorUtility.SetDirty(instance);
            instance.name = name;
            instance.value = value;
            foreach (var child in value)
            {
                
            }
            return instance;
        }
        public Vector3DataAsset[] value;

        public override object GetValue()
        {
            return (object)value;
        }
    }
}
