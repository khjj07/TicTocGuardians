using System;
using Default.Scripts.Util;
using UniRx;
using UnityEngine;

namespace TicTocGuardians.Scripts.Game.Player
{
    [RequireComponent(typeof(Player))]
    public class PlayerController : MonoBehaviour
    {
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
            _horizontalMovementHandler= GlobalInputBinder.CreateGetAxisStreamOptimize("Horizontal").Subscribe(v => _player.Act(new Action(Action.State.MoveX, v))).AddTo(gameObject);
            _verticalMovementHandler= GlobalInputBinder.CreateGetAxisStreamOptimize("Vertical").Subscribe(v => _player.Act(new Action(Action.State.MoveZ, v))).AddTo(gameObject);
            _jumpHandler= GlobalInputBinder.CreateGetKeyDownStream(KeyCode.Space).Subscribe(_ => _player.Act(new Action(Action.State.Jump))).AddTo(gameObject);
        }

        public void DisposeMovementStream()
        {
            _horizontalMovementHandler.Dispose();
            _verticalMovementHandler.Dispose();
            _jumpHandler.Dispose();
        }
    }
}