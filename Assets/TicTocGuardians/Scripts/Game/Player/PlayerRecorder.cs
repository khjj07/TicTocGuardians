using System;
using System.Collections.Generic;
using Default.Scripts.Util;
using TicTocGuardians.Scripts.Game.ETC;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace TicTocGuardians.Scripts.Game.Player
{
    public class PlayerRecorder : MonoBehaviour
    {
        private List<List<Action>> _actions = new List<List<Action>>();
        private List<Action> _currentActionBuffer;
        private bool _isRecord;
        private float _pushTime = 0;
        public void RecordStart(Player target)
        {
            _isRecord = true;
            CreateMovementRecordStream(target);
            Beaver beaver = target as Beaver;
            if (beaver != null)
            {
                CreateBeaverRecordStream(beaver);
            }
        }

        public void RecordStop()
        {
            _isRecord = false;
        }
        public List<List<Action>> GetActionLists()
        {
            return _actions;
        }

        public void CreateBeaverRecordStream(Beaver beaver)
        {
            GlobalInputBinder.CreateGetKeyDownStream(KeyCode.B)
                .Where(_ =>
                {
                    return beaver.IsBoxCreatable();
                }).First()
                .Subscribe(_ =>
                {
                    _currentActionBuffer.Add(new Action(Action.State.Special));
                }).AddTo(gameObject);
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
                        _currentActionBuffer.Add(new Action(Action.State.Push));
                        _pushTime = 0;
                    }
                }
                else
                {
                    _pushTime = 0;
                }
            }, null, () => { _pushTime = 0; }).AddTo(gameObject);
        }
        public void CreateMovementRecordStream(Player target)
        {
            _actions = new List<List<Action>>();
            _currentActionBuffer = new List<Action>();
            GlobalInputBinder.CreateGetAxisStreamOptimize("Horizontal").TakeWhile(_ => _isRecord).Subscribe(v => _currentActionBuffer.Add(new Action(Action.State.MoveX, v))).AddTo(target.gameObject);
            GlobalInputBinder.CreateGetAxisStreamOptimize("Vertical").TakeWhile(_ => _isRecord).Subscribe(v => _currentActionBuffer.Add(new Action(Action.State.MoveZ, v))).AddTo(target.gameObject);
            GlobalInputBinder.CreateGetKeyDownStream(KeyCode.Space).TakeWhile(_ => _isRecord).Subscribe(_ => _currentActionBuffer.Add(new Action(Action.State.Jump))).AddTo(target.gameObject);
     
            this.FixedUpdateAsObservable().Subscribe(_ =>
            {
                RecordAction(_currentActionBuffer);
                _currentActionBuffer = new List<Action>();
            }).AddTo(target.gameObject);
        }

        public void RecordAction(List<Action> actions)
        {
            _actions.Add(actions);
        }
    }
}