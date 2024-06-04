using System;
using TicTocGuardians.Scripts.Assets;
using TicTocGuardians.Scripts.Assets.LevelAsset;
using Unity.VisualScripting;
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
            var allChildren = modelPrefab.GetComponentsInChildren<MeshFilter>();
            foreach (var child in allChildren)
            {
                var instance = Instantiate(child, transform);
                var col = instance.AddComponent<MeshCollider>();
                col.sharedMesh = child.sharedMesh;
            }
        }
    }
}
