using System;
using UnityEditor.Animations;
using UnityEngine;

namespace TicTocGuardians.Scripts.Game.Player
{
    public class PlayerPreview : MonoBehaviour
    {
        private Animator _animator;
        private static readonly int Pick1 = Animator.StringToHash("Pick");
        private static readonly int Wait1 = Animator.StringToHash("Wait");

        public void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
        }

        public void Start()
        {
            Wait();
        }

        public void Wait()
        {
            _animator.SetBool(Wait1, true);
            _animator.SetBool(Pick1, false);
        }


        public void Pick()
        {
            _animator.SetBool(Wait1,false);
            _animator.SetBool(Pick1,true);
        }

    }
}
