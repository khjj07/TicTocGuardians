using TicTocGuardians.Scripts.Assets;
using TicTocGuardians.Scripts.Assets.LevelAsset;
using UnityEngine;

namespace TicTocGuardians.Scripts.Game.LevelObjects
{
    public class ModelLevelObject : LevelObject
    {
        public GameObject modelPrefab;

        public override LevelObjectAsset Serialize(LevelAsset parent)
        {
            var asset = base.Serialize(parent);
            asset.AddData(parent, PrefabDataAsset.Create("modelPrefab", modelPrefab));
            return asset;
        }


        public override void Deserialize(LevelObjectAsset asset)
        {
            base.Deserialize(asset);
            modelPrefab = (GameObject)asset.GetValue("modelPrefab");
        }

        public virtual void Initialize(GameObject modelObject)
        {
            modelPrefab = modelObject;
        }
    }
}