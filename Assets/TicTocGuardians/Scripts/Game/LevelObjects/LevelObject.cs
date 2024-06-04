using System;
using TicTocGuardians.Scripts.Assets;
using TicTocGuardians.Scripts.Assets.LevelAsset;
using UnityEngine;

namespace TicTocGuardians.Scripts.Game.LevelObjects
{
    [Serializable]
    public class LevelObjectAsset : BaseObjectAsset
    {
        public LevelObject prefab;
    }

    public class LevelObject : BaseObject<LevelObjectAsset>
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