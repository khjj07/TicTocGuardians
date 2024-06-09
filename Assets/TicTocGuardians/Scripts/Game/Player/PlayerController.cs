using System;
using Default.Scripts.Util;
using TicTocGuardians.Scripts.Game.ETC;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace TicTocGuardians.Scripts.Game.Player
{
    [RequireComponent(typeof(Player))]
    public class PlayerController : MonoBehaviour
    {
        public PlayerType type;
        private Player _player;
        private IDisposable _horizontalMovementHandler;
        private IDisposable _verticalMovementHandler;
        private IDisposable _jumpHandler;
        public void Awake()
        {
            _player = GetComponent<Player>();
        }

        public void CreateMovementStream()
        {
            _horizontalMovementHandler = GlobalInputBinder.CreateGetAxisStreamOptimize("Horizontal").Subscribe(v => _player.Act(new Action(Action.State.MoveX, v))).AddTo(gameObject);
            _verticalMovementHandler = GlobalInputBinder.CreateGetAxisStreamOptimize("Vertical").Subscribe(v => _player.Act(new Action(Action.State.MoveZ, v))).AddTo(gameObject);
            _jumpHandler = GlobalInputBinder.CreateGetKeyDownStream(KeyCode.Space).Subscribe(_ => _player.Act(new Action(Action.State.Jump))).AddTo(gameObject);
        }

        public void CreateBeaverStream()
        {
            Beaver beaver = _player as Beaver;

            GlobalInputBinder.CreateGetKeyDownStream(KeyCode.B)
                .Where(_ =>
                {
                    Debug.Log(beaver.IsBoxCreatable());
                    return beaver.IsBoxCreatable();
                }).First()
                .Subscribe(_ =>
                {
                    beaver.CreateBox();
                });


            var pushPointEnterStream = this.OnTriggerEnterAsObservable().Where(x => x.CompareTag("BeaverBoxPushPoint")).Select(x => x.GetComponentInParent<BeaverBox>());
            var pushPointExitStream = this.OnTriggerExitAsObservable().Where(x => x.CompareTag("BeaverBoxPushPoint")).Select(x => x.GetComponentInParent<BeaverBox>());

            var pushStream = Observable.Zip(GlobalInputBinder.CreateGetAxisStream("Horizontal"),
                GlobalInputBinder.CreateGetAxisStream("Vertical")).ThrottleFirst(TimeSpan.FromSeconds(1.0f)).Skip(1);

            pushPointEnterStream.Subscribe(x =>
                    {
                        Debug.Log("setTarget");
                        beaver.SetPushTarget(x);
                        pushStream.TakeUntil(pushPointExitStream)
                            .Where(v =>
                            {
                                Debug.Log(new Vector3(v[0], 0, v[1]));
                                return Vector3.Dot(x.contactDirection, new Vector3(v[0], 0, v[1])) < -0.3;
                            })
                            .Subscribe(_ => x.OnPush());
                    });
            pushPointExitStream.Subscribe(x =>
            {
                Debug.Log("null");
                beaver.SetPushTarget(null);
            });
        }



        public void DisposeMovementStream()
        {
            _horizontalMovementHandler.Dispose();
            _verticalMovementHandler.Dispose();
            _jumpHandler.Dispose();
        }
    }
}