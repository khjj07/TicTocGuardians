using System;
using TicTocGuardians.Scripts.Assets;
using TicTocGuardians.Scripts.Assets.LevelAsset;
using TicTocGuardians.Scripts.Interface;
using UniRx;
using UniRx.Triggers;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace TicTocGuardians.Scripts.Game.LevelObjects
{
    public class RailLevelObject : LevelObject
    {
        public MeshRenderer[] railDecals = new MeshRenderer[3];
        public float speed;
        private static readonly int RailSpeed = Shader.PropertyToID("_RailSpeed");

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
           
        }

        public void Start()
        {
            foreach (var rail in railDecals)
            {
                if (rail.material.HasProperty(RailSpeed))
                {
                    rail.material.SetFloat(RailSpeed, speed);
                }
                else
                {
                    Debug.LogWarning("Property not found on shader: " + RailSpeed);
                }
            }

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