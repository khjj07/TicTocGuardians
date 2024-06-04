using DG.Tweening;
using TicTocGuardians.Scripts.Assets;
using TicTocGuardians.Scripts.Assets.LevelAsset;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace TicTocGuardians.Scripts.Game.LevelObjects
{
    [RequireComponent(typeof(Camera))]
    public class CameraLevelObject : SingleLevelObject<CameraLevelObject>
    {
        public override LevelObjectAsset Serialize(LevelAsset parent)
        {
            var asset = base.Serialize(parent);
            asset.AddData(parent,FloatDataAsset.Create("orthographicSize",GetComponent<Camera>().orthographicSize));
            return asset;
        }

        public override void Deserialize(LevelObjectAsset asset)
        {
            base.Deserialize(asset);
            GetComponent<Camera>().orthographicSize = (float)asset.GetValue("orthographicSize");
        }

        public void Move(Vector3 position,float duration)
        {
            transform.DOMove(position, duration);
        }
    }
}