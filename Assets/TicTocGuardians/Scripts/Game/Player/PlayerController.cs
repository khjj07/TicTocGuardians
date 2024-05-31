using Default.Scripts.Util;
using UniRx;
using UnityEngine;

namespace TicTocGuardians.Scripts.Game.Player
{
    [RequireComponent(typeof(Player))]
    public class PlayerController : MonoBehaviour
    {
        private Player _player;

        public void Awake()
        {
            _player = GetComponent<Player>();
        }


        public void CreateMovementStream()
        {
            GlobalInputBinder.CreateGetAxisStreamOptimize("Horizontal").Subscribe(v => _player.Act(new Action(Action.State.MoveX, v))).AddTo(gameObject);
            GlobalInputBinder.CreateGetAxisStreamOptimize("Vertical").Subscribe(v => _player.Act(new Action(Action.State.MoveZ, v))).AddTo(gameObject);
            GlobalInputBinder.CreateGetKeyDownStream(KeyCode.Space).Subscribe(_ => _player.Act(new Action(Action.State.Jump))).AddTo(gameObject);
        }

     

    }
}