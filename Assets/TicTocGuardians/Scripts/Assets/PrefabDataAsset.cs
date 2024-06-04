using UnityEditor;
using UnityEngine;

namespace TicTocGuardians.Scripts.Assets
{
    public class PrefabDataAsset : LevelDataAsset 
    {
        public static LevelDataAsset Create(string name, GameObject value)
        {
            var instance = CreateInstance<PrefabDataAsset>();
            EditorUtility.SetDirty(instance);
            instance.name = name;
            instance.value = value;
            return instance;
        }

        public GameObject value;

        public override object GetValue()
        {
            return (object)value;
        }
    }
}