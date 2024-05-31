using DG.Tweening;
using TicTocGuardians.Scripts.Assets;
using TicTocGuardians.Scripts.Assets.LevelAsset;
using UnityEditor;
using UnityEngine;

namespace TicTocGuardians.Scripts.Game.LevelObjects
{
    [RequireComponent(typeof(Camera))]
    public class LevelCamera : BaseObject<BaseObjectAsset>
    {
        public override BaseObjectAsset Serialize(LevelAsset parent)
        {
            var asset = new BaseObjectAsset();
            asset.name = gameObject.name;
            asset.position = transform.position;
            asset.eulerAngles = transform.eulerAngles;
            asset.scale = transform.localScale;
            asset.AddData(parent,FloatDataAsset.Create("orthographicSize",GetComponent<Camera>().orthographicSize));
            return asset;
        }

        public override void Deserialize(BaseObjectAsset asset)
        {
            gameObject.name = asset.name;
            transform.position = asset.position;
            transform.eulerAngles = asset.eulerAngles;
            transform.localScale = asset.scale;
            GetComponent<Camera>().orthographicSize = (float)asset.GetValue("orthographicSize");
        }

        public void Move(Vector3 position,float duration)
        {
            transform.DOMove(position, duration);
        }
    }
}