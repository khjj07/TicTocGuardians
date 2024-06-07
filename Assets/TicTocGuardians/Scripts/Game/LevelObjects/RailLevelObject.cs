﻿using TicTocGuardians.Scripts.Interface;
using UniRx;
using UniRx.Triggers;
using Unity.VisualScripting;
using UnityEngine;

namespace TicTocGuardians.Scripts.Game.LevelObjects
{
    public class RailLevelObject : LevelObject
    {
        public float speed;
        private void Start()
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
        }
    }
}