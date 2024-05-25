using System;
using UnityEditor;
using UnityEngine;

namespace TicTocGuardians.Scripts.Assets
{
    [Serializable]
    public class Vector3DataAsset : LevelDataAsset
    {
        public static LevelDataAsset Create(string name, Vector3 value)
        {
            var instance = CreateInstance<Vector3DataAsset>();
            EditorUtility.SetDirty(instance);
            instance.name = name;
            instance.value = value;
            return instance;
        }
        public Vector3 value;

        public override object GetValue()
        {
            return (object)value;
        }
    }
}
