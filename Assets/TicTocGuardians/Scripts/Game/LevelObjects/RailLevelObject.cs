using System;
using TicTocGuardians.Scripts.Assets;
using TicTocGuardians.Scripts.Assets.LevelAsset;
using TicTocGuardians.Scripts.Interface;
using UniRx;
using UniRx.Triggers;
using Unity.VisualScripting;
using UnityEngine;

namespace TicTocGuardians.Scripts.Game.LevelObjects
{
    public class RailLevelObject : LevelObject
    {
        public float speed;

        public override LevelObjectAsset Serialize(LevelAsset parent)
        {
           var asset =  base.Serialize(parent);
           asset.AddData(parent,FloatDataAsset.Create("speed",speed));
           return asset;
        }
        public override void Deserialize(LevelObjectAsset asset)
        {
            base.Deserialize(asset);
            speed = (float)asset.GetValue("speed");
            var renderer = GetComponentInChildren<MeshRenderer>();
            renderer.sharedMaterial = new Material(renderer.sharedMaterial);
            renderer.sharedMaterial.SetFloat("RailSpeed",speed);
        }

        public void Start()
        {
            GetComponentInChildren<MeshCollider>().OnCollisionStayAsObservable()
                .Select(x => x.collider.GetComponent<IMovable>())
                .Subscribe(obj =>
                {
                    var forward = transform.forward;
                    Debug.Log(forward);
                    obj.MoveX(forward.x * speed);
                    obj.MoveZ(forward.z * speed);
                }).AddTo(gameObject);

            //Observable.Interval(TimeSpan.FromSeconds(0.1f)).Subscribe()
        }
    }
}