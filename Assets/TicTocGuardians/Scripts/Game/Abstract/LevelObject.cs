using System;
using TicTocGuardians.Scripts.Assets;
using UnityEngine;

namespace TicTocGuardians.Scripts.Game.Abstract
{
    [Serializable]
    public class LevelObjectAsset : BaseObjectAsset
    {
        public LevelObject prefab;
        public GameObject modelPrefab;
    }

    public abstract class LevelObject : BaseObject<LevelObjectAsset>
    {
       
        public GameObject modelPrefab;
        public abstract void Initialize(GameObject modelPrefab);
    }
}
