using System;
using UnityEditor;
using UnityEngine;

namespace TicTocGuardians.Scripts.Assets
{
    [Serializable]
    public class Vector3DataAsset : LevelDataAsset
    {
        public Vector3 value;

        public static LevelDataAsset Create(string name, Vector3 value)
        {
            var instance = CreateInstance<Vector3DataAsset>();
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