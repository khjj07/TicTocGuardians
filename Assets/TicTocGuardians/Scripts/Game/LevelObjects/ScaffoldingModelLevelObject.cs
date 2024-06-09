using System;
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
