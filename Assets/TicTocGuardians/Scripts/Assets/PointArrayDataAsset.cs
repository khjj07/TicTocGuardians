using UnityEditor;

namespace TicTocGuardians.Scripts.Assets
{
    public class PointArrayDataAsset : LevelDataAsset
    {
        public Vector3DataAsset[] value;

        public static LevelDataAsset Create(string name, Vector3DataAsset[] value)
        {
            var instance = CreateInstance<PointArrayDataAsset>();
#if UNITY_EDITOR
            EditorUtility.SetDirty(instance);
#endif
            instance.name = name;
            instance.value = value;
            foreach (var child in value)
            {
            }

            return instance;
        }

        public override object GetValue()
        {
            return value;
        }
    }
}