using System;
using TicTocGuardians.Scripts.Game.LevelObjects;
using TicTocGuardians.Scripts.Game.Player;
using UnityEngine;

namespace TicTocGuardians.Scripts.LevelEditor
{
    public class LevelTester : MonoBehaviour
    {
        public Transform origin;
        public PlayerController playerPrefab;
        public SpawnPointLevelObject spawnPoint;

        public void Start()
        {
            var instance = Instantiate(playerPrefab, origin);
            switch (instance.type)
            {
                case PlayerType.None:
                    break;
                case PlayerType.Beaver:
                    instance.CreateMovementStream();
                    instance.CreateBeaverStream();
                    break;
                case PlayerType.Cat:
                    instance.CreateMovementStream();
                    break;
                case PlayerType.Rabbit:
                    instance.CreateMovementStream();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            instance.transform.position = spawnPoint.transform.position + Vector3.up * 3;
        }
    }
}