using System;
using System.Collections;
using Default.Scripts.Util;
using DG.Tweening;
using TicTocGuardians.Scripts.Game.ETC;
using TicTocGuardians.Scripts.Game.LevelObjects;
using TicTocGuardians.Scripts.Game.Manager;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace TicTocGuardians.Scripts.Game.Player
{
    public class Beaver : Player
    {
        [SerializeField] private BeaverBox beaverBoxPrefab;
        [SerializeField] private Transform boxPoint;
        [SerializeField] private float boxCreateCheckDistance;
        [SerializeField] private float boxCreateUpDistance;
        [SerializeField] private float pushTime = 3.0f;
        [SerializeField] private float pushOffset = 2.0f;
        [SerializeField]
        private BeaverBox _pushTarget;
        private StaticModelLevelObject _targetPlatform;
        private bool _isCreatable = true;

        public override void Start()
        {
            base.Start();
        }

        public void PushTargetBox(Vector3 direction)
        {
            _pushTarget.transform.DOMove(direction * pushOffset, 0.2f).SetRelative(true);
        }

        public void CreateBeaverBox()
        {
            var instance = Instantiate(beaverBoxPrefab);
            instance.transform.position = _targetPlatform.transform.position + Vector3.up * boxCreateUpDistance;
            _isCreatable = false;
        }
    }
}