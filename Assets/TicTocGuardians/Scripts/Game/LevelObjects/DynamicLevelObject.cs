using TicTocGuardians.Scripts.Assets.LevelAsset;
using TicTocGuardians.Scripts.Interface;
using Unity.VisualScripting;
using UnityEngine;

namespace TicTocGuardians.Scripts.Game.LevelObjects
{
    public class DynamicLevelObject : LevelObject, IDynamic
    {
        public override LevelObjectAsset Serialize(LevelAsset parent)
        {
            var asset = new LevelObjectAsset();
            asset.name = gameObject.name;
            asset.position = transform.position;
            asset.eulerAngles = transform.eulerAngles;
            asset.scale = transform.localScale;
            asset.modelPrefab = modelPrefab;
            return asset;
        }

        public override void Deserialize(LevelObjectAsset asset)
        {
            gameObject.name = asset.name;
            transform.position = asset.position;
            transform.eulerAngles = asset.eulerAngles;
            transform.localScale = asset.scale;
            Initialize(asset.modelPrefab);
        }

        public override void Initialize(GameObject modelPrefab)
        {
            this.modelPrefab = modelPrefab;
            var allChildren = modelPrefab.GetComponentsInChildren<MeshFilter>();
            foreach (var child in allChildren)
            {
                var instance = Instantiate(child, transform);
                var col = instance.AddComponent<MeshCollider>();
                //col.convex = true;
                col.sharedMesh = child.sharedMesh;
            }
        }

    }
}
