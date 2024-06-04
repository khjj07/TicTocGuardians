using System;
using TicTocGuardians.Scripts.Game.LevelObjects;
using TicTocGuardians.Scripts.Game.Player;
using UnityEngine;
using static UnityEngine.UI.Image;

namespace TicTocGuardians.Scripts.LevelEditor
{
    public class LevelTester : MonoBehaviour
    {
        public Transform origin;
        public PlayerController playerPrefab;
        public void Start()
        {
            var instance = Instantiate(playerPrefab, origin);
            instance.CreateMovementStream();
            instance.transform.position = SpawnPointLevelObject.Instance.transform.position;
        }
    }
}