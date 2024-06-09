using System;
using System.Collections;
using Default.Scripts.Util;
using DG.Tweening;
using TicTocGuardians.Scripts.Game.ETC;
using TicTocGuardians.Scripts.Game.LevelObjects;
using TicTocGuardians.Scripts.Game.Manager;
using TicTocGuardians.Scripts.Interface;
using UniRx;
using UniRx.Triggers;
using Unity.Burst.CompilerServices;
using UnityEngine;

namespace TicTocGuardians.Scripts.Game.Player
{
    public class Beaver : Player
    {
        public Transform boxCreatePoint;
        public float boxCreateCheckDistance;
        [SerializeField] private BeaverBox beaverBoxPrefab;
        [SerializeField] private float boxCreateUpDistance;
        [SerializeField] private float pushTime = 3.0f;
        [SerializeField] private float pushOffset = 2.0f;
        [SerializeField]
        private IPushable _pushTarget;
        private StaticModelLevelObject _targetPlatform;

        public override void Start()
        {
            base.Start();
            this.UpdateAsObservable().Subscribe(_ =>
            {
                if (Physics.Raycast(boxCreatePoint.position, Vector3.down, boxCreateCheckDistance))
                {
                    Debug.DrawRay(boxCreatePoint.position, Vector3.down * boxCreateCheckDistance,Color.red);
                }
                else
                {
                    Debug.DrawRay(boxCreatePoint.position, Vector3.down * boxCreateCheckDistance);
                }
                
            });
        }

        public void SetPushTarget(IPushable box)
        {
            _pushTarget = box;
        }

        public void PushTarget()
        {
            if (_pushTarget != null)
            {
                _pushTarget.OnPush();
            }
        }
        
        
        public bool IsBoxCreatable()
        {
            return Physics.Raycast(boxCreatePoint.position, Vector3.down, boxCreateCheckDistance);
        }

        public void CreateBox()
        {
            RaycastHit hit;
            Vector3 boxPosition = Vector3.zero;
            var instance = Instantiate(beaverBoxPrefab, LevelOrigin.Instance.transform);
            Physics.Raycast(boxCreatePoint.position, Vector3.down, out hit, boxCreateCheckDistance);
            boxPosition = hit.collider.transform.position;
            boxPosition.y = boxCreatePoint.position.y;
            instance.transform.position = boxPosition;
            instance.transform.DOScale(new Vector3(0.9f, 1, 0.9f), 0.3f);
        }
    }
}