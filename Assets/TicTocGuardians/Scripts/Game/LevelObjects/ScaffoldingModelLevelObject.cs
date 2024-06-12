using System;
using TicTocGuardians.Scripts.Assets;
using TicTocGuardians.Scripts.Assets.LevelAsset;
using TicTocGuardians.Scripts.Interface;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Events;

namespace TicTocGuardians.Scripts.Game.LevelObjects
{
    public class ScaffoldingModelLevelObject : StaticModelLevelObject
    {
        public LevelObject reactableObject;
        public override LevelObjectAsset Serialize(LevelAsset parent)
        {
            var instance = base.Serialize(parent);
            instance.AddData(parent,StringDataAsset.Create("reactableObject", reactableObject.name));
            return instance;
        }

        public override void Deserialize(LevelObjectAsset asset)
        {
            base.Deserialize(asset);
            reactableObject = GameObject.Find(asset.GetValue("reactableObject") as string).GetComponent<LevelObject>();
        }

        public void Start()
        {
            GetComponentInChildren<MeshCollider>().OnCollisionEnterAsObservable()
                .Where(collision => collision.contacts[0].normal.y < -0.7)
                .Select(_=> reactableObject as IReactable)
                .Subscribe(x=> x.React());

            GetComponentInChildren<MeshCollider>().OnCollisionExitAsObservable()
                .Select(_ => reactableObject as IReactable)
                .Subscribe(x => x.React());
        }

        public void OnDrawGizmos()
        {
            var obj = reactableObject as LevelObject;
            if (obj != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, obj.transform.position);
            }
        }
    }
}
