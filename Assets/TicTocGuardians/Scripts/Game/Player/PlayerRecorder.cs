using System;
using System.Collections.Generic;
using Default.Scripts.Util;
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

        public void RecordStart(Player target)
        {
            _isRecord = true;
            CreateMovementRecordStream(target);
        }

        public void RecordStop()
        {
            _isRecord = false;
        }
        public List<List<Action>> GetActionLists()
        {
            return _actions;
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