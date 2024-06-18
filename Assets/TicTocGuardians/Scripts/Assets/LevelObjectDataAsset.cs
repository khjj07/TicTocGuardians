using TicTocGuardians.Scripts.Game.LevelObjects;
using UnityEditor;

namespace TicTocGuardians.Scripts.Assets
{
    public class LevelObjectDataAsset : LevelDataAsset
    {
        public LevelObjectAsset value;

        public static LevelDataAsset Create(string name, LevelObjectAsset value)
        {
            var instance = CreateInstance<LevelObjectDataAsset>();
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