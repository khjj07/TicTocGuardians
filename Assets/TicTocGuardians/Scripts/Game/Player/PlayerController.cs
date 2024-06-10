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
        private IDisposable _pushHandler;
        private IDisposable _createBoxHandler;
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

            _createBoxHandler = GlobalInputBinder.CreateGetKeyDownStream(KeyCode.B)
                .Where(_ =>
                {
                    Debug.Log(beaver.IsBoxCreatable());
                    return beaver.IsBoxCreatable();
                }).First()
                .Subscribe(_ =>
                {
                    beaver.CreateBox();
                });




            var inputStream = Observable.Zip(GlobalInputBinder.CreateGetAxisStream("Horizontal"),
                GlobalInputBinder.CreateGetAxisStream("Vertical"));

                _pushHandler = inputStream.Where(_=>beaver.GetPushTarget()!=null).ThrottleFirst(TimeSpan.FromSeconds(1.0f)).Skip(1).TakeWhile(_=>beaver.GetPushTarget()!=null)
                .Subscribe(v =>
                {
                    var box = beaver.GetPushTarget() as BeaverBox;
                    if (Vector3.Dot(box.contactDirection, new Vector3(v[0], 0, v[1])) < -0.3)
                    {
                        beaver.Act(new Action(Action.State.Push));
                    }
                    else
                    {
                        beaver.Act(new Action(Action.State.PushNotReady));
                    }
                });



        }

        public void DisposeAllStream()
        {
            DisposeMovementStream();
            switch (type)
            {
                case PlayerType.Beaver:
                    DisposeBeaverStream();
                    break;

            }
        }

        public void DisposeMovementStream()
        {
            _horizontalMovementHandler.Dispose();
            _verticalMovementHandler.Dispose();
            _jumpHandler.Dispose();
        }

        public void DisposeBeaverStream()
        {
            _pushHandler.Dispose();
            _createBoxHandler.Dispose();
        }
    }
}