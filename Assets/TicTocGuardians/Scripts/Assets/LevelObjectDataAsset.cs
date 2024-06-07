using TicTocGuardians.Scripts.Game.LevelObjects;
using UnityEditor;
using UnityEngine;

namespace TicTocGuardians.Scripts.Assets
{
    public class LevelObjectDataAsset : LevelDataAsset
    {
        public static LevelDataAsset Create(string name, LevelObjectAsset value)
        {
            var instance = CreateInstance<LevelObjectDataAsset>();
            EditorUtility.SetDirty(instance);
            instance.name = name;
            instance.value = value;
            return instance;
        }

        public LevelObjectAsset value;

        public override object GetValue()
        {
            return (object)value;
        }
    }
}