using System;
using DG.Tweening;
using TicTocGuardians.Scripts.Game.ETC;
using TicTocGuardians.Scripts.Game.LevelObjects;
using TicTocGuardians.Scripts.Game.Manager;
using TicTocGuardians.Scripts.Interface;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace TicTocGuardians.Scripts.Game.Player
{
    public class Beaver : Player
    {
        public Transform boxCreatePoint;
        public float boxCreateCheckDistance;
        [SerializeField] private BeaverBox beaverBoxPrefab;

        [SerializeField] private ParticleSystem boxCreateParticlePrefab;
        [SerializeField] private float boxCreateUpDistance;
        [SerializeField] private float pushTime = 3.0f;
        [SerializeField] private float pushOffset = 2.0f;
        private IPushable _pushTarget;
        private StaticModelLevelObject _targetPlatform;
        private BeaverBox boxInstance;

        public override void Start()
        {
            base.Start();
            this.UpdateAsObservable().Subscribe(_ =>
            {
                if (Physics.Raycast(boxCreatePoint.position, Vector3.down, boxCreateCheckDistance))
                    Debug.DrawRay(boxCreatePoint.position, Vector3.down * boxCreateCheckDistance, Color.red);
                else
                    Debug.DrawRay(boxCreatePoint.position, Vector3.down * boxCreateCheckDistance);
            }).AddTo(gameObject);
        }

        public void OnDestroy()
        {
            if (boxInstance) Destroy(boxInstance.gameObject);
        }

        public override void CreateDefaultStream()
        {
            base.CreateDefaultStream();

            var pushPointStayStream = this.OnTriggerStayAsObservable().Where(x => x.CompareTag("BeaverBoxPushPoint"))
                .Select(x => x.GetComponent<BeaverBoxPushPoint>());
            var pushPointExitStream = this.OnTriggerExitAsObservable().Where(x => x.CompareTag("BeaverBoxPushPoint"))
                .Select(x => x.GetComponent<BeaverBoxPushPoint>());
            pushPointStayStream.Subscribe(x =>
            {
                if (Vector3.Dot(x.direction.normalized, _direction) < -0.3f)
                {
                    SetAnimationState(AnimationState.PushReady);
                    _isOperating = true;
                }
                else
                {
                    _isOperating = false;
                    SetDirection(-x.direction.normalized);
                }

                SetPushTarget(x.GetComponentInParent<BeaverBox>());
            }).AddTo(gameObject);

            pushPointExitStream.Subscribe(x =>
            {
                SetPushTarget(null);
                _isOperating = false;
            }).AddTo(gameObject);

            var baseStream = actionSubject.Where(_ => _ != null);
            baseStream.Where(action => action.state == Action.State.Special)
                .Subscribe(_ => { CreateBox(); }).AddTo(gameObject);

            baseStream.Where(action => action.state == Action.State.Push)
                .Subscribe(_ =>
                {
                    SetAnimationState(AnimationState.Push);
                    _movable = false;
                    Observable.Timer(TimeSpan.FromSeconds(1.0f)).Subscribe(_ => { _movable = true; });
                    PushTarget();
                }).AddTo(gameObject);
        }

        public IPushable GetPushTarget()
        {
            return _pushTarget;
        }

        public void SetPushTarget(IPushable box)
        {
            _pushTarget = box;
        }

        public void PushTarget()
        {
            if (_pushTarget != null) _pushTarget.OnPush();
        }


        public bool IsBoxCreatable()
        {
            return Physics.Raycast(boxCreatePoint.position, Vector3.down, boxCreateCheckDistance);
        }

        public void CreateBoxCreateParticle()
        {
            var instance = Instantiate(boxCreateParticlePrefab);
            instance.transform.position = boxCreatePoint.position;
        }

        public void CreateBox()
        {
            GlobalSoundManager.Instance.PlaySFX("SFX_CreateBox");
            RaycastHit hit;
            var boxPosition = Vector3.zero;
            boxInstance = Instantiate(beaverBoxPrefab, LevelOrigin.Instance.transform);
            Physics.Raycast(boxCreatePoint.position, Vector3.down, out hit, boxCreateCheckDistance);
            boxPosition = hit.collider.transform.position;
            boxPosition.y = boxCreatePoint.position.y;
            boxInstance.transform.position = boxPosition;
            boxInstance.transform.DOScale(new Vector3(0.9f, 1, 0.9f), 0.5f).SetDelay(0.5f).SetEase(Ease.InCubic);
            CreateBoxCreateParticle();
        }
    }
}