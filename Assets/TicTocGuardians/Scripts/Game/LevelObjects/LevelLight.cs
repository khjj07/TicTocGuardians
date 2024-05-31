using TicTocGuardians.Scripts.Assets.LevelAsset;
using TicTocGuardians.Scripts.Assets;
using UnityEngine;

namespace TicTocGuardians.Scripts.Game.LevelObjects
{
    public class LevelLight : BaseObject<BaseObjectAsset>
    {

        public override BaseObjectAsset Serialize(LevelAsset parent)
        {
            var asset = new BaseObjectAsset();
            asset.name = gameObject.name;
            asset.position = transform.position;
            asset.eulerAngles = transform.eulerAngles;
            asset.scale = transform.localScale;
            return asset;
        }

        public override void Deserialize(BaseObjectAsset asset)
        {
            gameObject.name = asset.name;
            transform.position = asset.position;
            transform.eulerAngles = asset.eulerAngles;
            transform.localScale = asset.scale;
        }
    }
}
