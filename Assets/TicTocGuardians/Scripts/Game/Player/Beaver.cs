using System;
using System.Collections;
using Default.Scripts.Util;
using DG.Tweening;
using TicTocGuardians.Scripts.Game.ETC;
using TicTocGuardians.Scripts.Game.LevelObjects;
using TicTocGuardians.Scripts.Game.Manager;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace TicTocGuardians.Scripts.Game.Player
{
    public class Beaver : Player
    {
        [SerializeField] private BeaverBox beaverBoxPrefab;
        [SerializeField] private Transform boxPoint;
        [SerializeField] private float boxCreateCheckDistance;
        [SerializeField] private float boxCreateUpDistance;
        [SerializeField] private float pushTime = 3.0f;
        [SerializeField] private float pushOffset = 2.0f;
        [SerializeField]
        private BeaverBox _pushTarget;
        private StaticModelLevelObject _targetPlatform;
        private bool _isCreatable = true;

        public override void Start()
        {
            CreateBeaverStream();
        }

        public void CreateBeaverStream()
        {
            this.UpdateAsObservable().Select(_ =>
                {
                    RaycastHit? result = null;
                    RaycastHit hit;
                    Debug.DrawRay(boxPoint.position, Vector3.down * boxCreateCheckDistance);
                    if (Physics.Raycast(boxPoint.position, Vector3.down * boxCreateCheckDistance, out hit))
                    {
                        result = hit;
                    }
                    return result;
                })
                .Subscribe(x =>
                {
                    if (x != null)
                    {
                        _targetPlatform = x.Value.collider.GetComponentInParent<StaticModelLevelObject>();
                    }
                    else
                    {
                        _targetPlatform = null;
                    }
                }).AddTo(gameObject);

            GlobalInputBinder.CreateGetKeyDownStream(KeyCode.B).Where(_ => _isCreatable).First().Subscribe(_ =>
            {
                CreateBeaverBox();
            }).AddTo(gameObject);


            var enterBoxStream = _capsuleCollider.OnCollisionStayAsObservable()
                .Where(collision => collision.collider.CompareTag("BeaverBox"));

            var exitBoxStream = _capsuleCollider.OnCollisionExitAsObservable()
                .Where(collision => collision.collider.CompareTag("BeaverBox"));

            enterBoxStream.Subscribe(collision => _pushTarget = collision.collider.GetComponent<BeaverBox>());
            exitBoxStream.Subscribe(collision => _pushTarget = null);


            enterBoxStream.SelectMany(_ => GlobalInputBinder.CreateGetKeyDownStream(KeyCode.B))
                .TakeUntil(exitBoxStream)
                .Repeat()
                .Where(_ => Quaternion.Angle(Quaternion.LookRotation(_pushTarget.transform.forward),
                    Quaternion.LookRotation(transform.forward)) < 10)
                .Subscribe(_ => PushTargetBox(_pushTarget.transform.forward));

            enterBoxStream.SelectMany(_ => GlobalInputBinder.CreateGetKeyDownStream(KeyCode.B))
                .TakeUntil(exitBoxStream)
                .Repeat()
                .Where(_ => Quaternion.Angle(Quaternion.LookRotation(_pushTarget.transform.right),
                    Quaternion.LookRotation(transform.forward)) < 10)
                .Subscribe(_ => PushTargetBox(_pushTarget.transform.right));

            enterBoxStream.SelectMany(_ => GlobalInputBinder.CreateGetKeyDownStream(KeyCode.B))
                .TakeUntil(exitBoxStream)
                .Repeat()
                .Where(_ => Quaternion.Angle(Quaternion.LookRotation(_pushTarget.transform.forward * -1),
                    Quaternion.LookRotation(transform.forward)) < 10)
                .Subscribe(_ => PushTargetBox(_pushTarget.transform.forward * -1));

            enterBoxStream.SelectMany(_ => GlobalInputBinder.CreateGetKeyDownStream(KeyCode.B))
                .TakeUntil(exitBoxStream)
                .Repeat()
                .Where(_ => Quaternion.Angle(Quaternion.LookRotation(_pushTarget.transform.right * -1),
                    Quaternion.LookRotation(transform.forward)) < 10)
                .Subscribe(_ => PushTargetBox(_pushTarget.transform.right*-1));



        }

        public void PushTargetBox(Vector3 direction)
        {
            _pushTarget.transform.DOMove(direction * pushOffset, 0.2f).SetRelative(true);
        }

        public void CreateBeaverBox()
        {
            var instance = Instantiate(beaverBoxPrefab);
            instance.transform.position = _targetPlatform.transform.position + Vector3.up * boxCreateUpDistance;
            _isCreatable = false;
        }
    }
}