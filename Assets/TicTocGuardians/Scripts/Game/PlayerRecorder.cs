using System.Collections.Generic;
using Default.Scripts.Util;
using UniRx;
using UnityEngine;

namespace TicTocGuardians.Scripts.Game
{
    public class PlayerRecorder : MonoBehaviour
    {
        private List<Action> _actions = new List<Action>();
        private bool _isRecord;

        public void RecordStart()
        {
            _isRecord = true;
            CreateMovementRecordStream();
        }
        public void RecordStop()
        {
            _isRecord = false;
        }


        public void CreateMovementRecordStream()
        {
            GlobalInputBinder.CreateGetAxisStreamOptimize("Horizontal").TakeWhile(_=>_isRecord).Subscribe(v => RecordAction(new Action(Action.State.MoveX, v))).AddTo(gameObject);
            GlobalInputBinder.CreateGetAxisStreamOptimize("Vertical").TakeWhile(_ => _isRecord).Subscribe(v => RecordAction(new Action(Action.State.MoveZ, v))).AddTo(gameObject);
            GlobalInputBinder.CreateGetKeyDownStream(KeyCode.Space).TakeWhile(_ => _isRecord).Subscribe(_ => RecordAction(new Action(Action.State.Jump))).AddTo(gameObject);
        }

        public void RecordAction(Action action)
        {
            _actions.Add(action);
        }
    }
}