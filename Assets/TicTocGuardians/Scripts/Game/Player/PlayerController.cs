using System;
using System.Collections.Generic;
using Default.Scripts.Util;
using TicTocGuardians.Scripts.Game.ETC;
using UniRx;
using UnityEngine;

namespace TicTocGuardians.Scripts.Game.Player
{
    [RequireComponent(typeof(Player))]
    public class PlayerController : MonoBehaviour
    {
        public PlayerType type;
        private Player _player;
        private float _pushTime;
        private readonly List<IDisposable> streams = new();

        public void Awake()
        {
            _player = GetComponent<Player>();
        }


        public void CreateMovementStream()
        {
            var s1 = GlobalInputBinder.CreateGetAxisStreamOptimize("Horizontal")
                .Subscribe(v => _player.Act(new Action(Action.State.MoveX, v))).AddTo(gameObject);
            var s2 = GlobalInputBinder.CreateGetAxisStreamOptimize("Vertical")
                .Subscribe(v => _player.Act(new Action(Action.State.MoveZ, v))).AddTo(gameObject);
            var s3 = GlobalInputBinder.CreateGetKeyDownStream(KeyCode.Space)
                .Subscribe(_ => _player.Act(new Action(Action.State.Jump))).AddTo(gameObject);
            streams.Add(s1);
            streams.Add(s2);
            streams.Add(s3);
        }

        public void CreateBeaverStream()
        {
            var beaver = _player as Beaver;
            var s1 = GlobalInputBinder.CreateGetKeyDownStream(KeyCode.B)
                .Where(_ => { return beaver.IsBoxCreatable(); }).First()
                .Subscribe(_ => { beaver.Act(new Action(Action.State.Special)); }).AddTo(gameObject);

            var inputStream = Observable.Zip(GlobalInputBinder.CreateGetAxisStream("Horizontal"),
                GlobalInputBinder.CreateGetAxisStream("Vertical"));

            var s2 = inputStream.Subscribe(v =>
            {
                var box = beaver.GetPushTarget() as BeaverBox;
                if (box != null && Vector3.Dot(box.contactDirection, new Vector3(v[0], 0, v[1])) < -0.3)
                {
                    if (_pushTime < 1.0f)
                    {
                        _pushTime += Time.deltaTime;
                    }
                    else
                    {
                        beaver.Act(new Action(Action.State.Push));
                        _pushTime = 0;
                    }
                }
                else
                {
                    _pushTime = 0;
                }
            }, null, () => { _pushTime = 0; }).AddTo(gameObject);

            streams.Add(s1);
            streams.Add(s2);
        }

        public void DisposeAllStream()
        {
            foreach (var s in streams) s.Dispose();
        }
    }
}