using UnityEngine;

namespace TicTocGuardians.Scripts.Game.LevelObjects
{
    public class BackgroundModelLevelObject : ModelLevelObject
    {
        public override void Deserialize(LevelObjectAsset asset)
        {
            base.Deserialize(asset);
            var allChildren = modelPrefab.GetComponentsInChildren<MeshFilter>();
            foreach (var child in allChildren)
            {
                var instance = Instantiate(child, transform);
            }
        }

        public override void Initialize(GameObject modelObject)
        {
            base.Initialize(modelObject);
            var allChildren = modelPrefab.GetComponentsInChildren<MeshFilter>();
            foreach (var child in allChildren)
            {
                var instance = Instantiate(child, transform);
            }
        }
    }
}
