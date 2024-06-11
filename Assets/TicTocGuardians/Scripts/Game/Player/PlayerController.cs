using System;
using System.Collections;
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

        private float _pushTime = 0;
        public void Awake()
        {
            _player = GetComponent<Player>();
        }


        public void CreateMovementStream()
        {
            GlobalInputBinder.CreateGetAxisStreamOptimize("Horizontal").Subscribe(v => _player.Act(new Action(Action.State.MoveX, v))).AddTo(this);
            GlobalInputBinder.CreateGetAxisStreamOptimize("Vertical").Subscribe(v => _player.Act(new Action(Action.State.MoveZ, v))).AddTo(this);
            GlobalInputBinder.CreateGetKeyDownStream(KeyCode.Space).Subscribe(_ => _player.Act(new Action(Action.State.Jump))).AddTo(this);
        }

        public void CreateBeaverStream()
        {
            Beaver beaver = _player as Beaver;
            GlobalInputBinder.CreateGetKeyDownStream(KeyCode.B)
                .Where(_ =>
                {
                    return beaver.IsBoxCreatable();
                }).First()
                .Subscribe(_ =>
                {
                    beaver.CreateBox();
                }).AddTo(this);




            var inputStream = Observable.Zip(GlobalInputBinder.CreateGetAxisStream("Horizontal"),
                GlobalInputBinder.CreateGetAxisStream("Vertical"));

            inputStream.Subscribe(v =>
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
                            Debug.Log("push");
                            _pushTime = 0;
                        }
                    }
                    else
                    {
                        _pushTime = 0;
                    }
                }, null, () => { _pushTime = 0; }).AddTo(this);
        }
    }
}