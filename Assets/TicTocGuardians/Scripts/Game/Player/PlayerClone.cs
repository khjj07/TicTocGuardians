using System;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace TicTocGuardians.Scripts.Game.Player
{
    [Serializable]
    public class PlayerCloneData
    {
        public PlayerType type;
        public List<List<Action>> actions;

        public PlayerCloneData(PlayerType type, List<List<Action>> actions)
        {
            this.type = type;
            this.actions = actions;
        }
    }

    [RequireComponent(typeof(Player))]
    public class PlayerClone : MonoBehaviour
    {
        [SerializeField] private float playRate = 0.01f;

        [SerializeField] private List<List<Action>> _actions;

        private int _index;

        private Player _player;

        public void Awake()
        {
            _player = GetComponent<Player>();
        }

        public void Start()
        {
            //CreateMovementStream();
        }

        public void SetActions(List<List<Action>> actions)
        {
            _actions = actions;
        }

        public void CreateMovementStream()
        {
            this.FixedUpdateAsObservable().TakeWhile(_ => _actions.Count > _index).Subscribe(v =>
            {
                foreach (var action in _actions[_index]) _player.Act(action);
                _index++;
            }).AddTo(gameObject);
        }
    }
}