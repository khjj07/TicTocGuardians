using System;
using System.Collections.Generic;
using TicTocGuardians.Scripts.Assets.LevelAsset;
using UnityEditor;
using UnityEngine;

namespace TicTocGuardians.Scripts.Assets
{
    [Serializable]
    public class BaseObjectAsset
    {
        public string name;
        public Vector3 position;
        public Vector3 eulerAngles;
        public Vector3 scale;
        public List<LevelDataAsset> data = new List<LevelDataAsset>();
        public object GetValue(string name)
        {
            return data.Find(x => x.name == name).GetValue();
        }

        public LevelDataAsset GetData(string name)
        {
            return data.Find(x => x.name == name);
        }

        public void AddData(ScriptableObject parent, LevelDataAsset value)
        {
            data.Add(value);
            AssetDatabase.AddObjectToAsset(value, parent);
        }
    }

    public abstract class BaseObject<T> : MonoBehaviour where T : BaseObjectAsset
    {
        public abstract T Serialize(LevelAsset.LevelAsset parent);

        public abstract void Deserialize(T asset);
    }
}
