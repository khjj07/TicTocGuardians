using TicTocGuardians.Scripts.Assets;
using TicTocGuardians.Scripts.Assets.LevelAsset;
using UnityEditor;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace TicTocGuardians.Scripts.Game.LevelObjects
{
    [RequireComponent(typeof(BoxCollider))]
    public class UnPassableLevelObject : StaticLevelObject
    {
        public override LevelObjectAsset Serialize(LevelAsset parent)
        {
            var asset = new LevelObjectAsset();
            asset.name = gameObject.name;
            asset.position = transform.position;
            asset.eulerAngles = transform.eulerAngles;
            asset.scale = transform.localScale;
            return asset;
        }

        public override void Deserialize(LevelObjectAsset asset)
        {
            gameObject.name = asset.name;
            transform.position = asset.position;
            transform.eulerAngles = asset.eulerAngles;
            transform.localScale = asset.scale;
        }
    }
}
