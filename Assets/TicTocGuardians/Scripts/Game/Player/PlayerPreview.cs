using System;
using UnityEditor.Animations;
using UnityEngine;

namespace TicTocGuardians.Scripts.Game.Player
{
    public class PlayerPreview : MonoBehaviour
    {
        public enum State
        {
            Idle,
            Pick,
            Run
        }

        private Animator _animator;
        public State defaultState;
        private static readonly int Pick1 = Animator.StringToHash("Pick");
        private static readonly int Idle = Animator.StringToHash("Idle");
        private static readonly int Run1 = Animator.StringToHash("Run");

        public void Awake()
        {
            _animator = GetComponentInChildren<Animator>(true);
        }

        public void Start()
        {
            switch (defaultState)
            {
                case State.Pick:
                    Pick();
                    break;
                case State.Idle:
                    Wait();
                    break;
                case State.Run:
                    Run();
                    break;
            }
        }

        public void Wait()
        {
            _animator.SetBool(Idle, true);
            _animator.SetBool(Pick1, false);
            _animator.SetBool(Run1, false);
        }

        public void Run()
        {
            _animator.SetBool(Idle, false);
            _animator.SetBool(Pick1, false);
            _animator.SetBool(Run1, true);
        }

        public void Pick()
        {
            _animator.SetBool(Idle, false);
            _animator.SetBool(Pick1,true);
            _animator.SetBool(Run1, false);
        }

    }
}
