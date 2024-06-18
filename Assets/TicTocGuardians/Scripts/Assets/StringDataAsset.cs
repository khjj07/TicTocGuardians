using UnityEditor;

namespace TicTocGuardians.Scripts.Assets
{
    public class StringDataAsset : LevelDataAsset
    {
        public string value;

        public static LevelDataAsset Create(string name, string value)
        {
            var instance = CreateInstance<StringDataAsset>();
#if UNITY_EDITOR
            EditorUtility.SetDirty(instance);
#endif
            instance.name = name;
            instance.value = value;
            return instance;
        }

        public override object GetValue()
        {
            return value;
        }
    }
}