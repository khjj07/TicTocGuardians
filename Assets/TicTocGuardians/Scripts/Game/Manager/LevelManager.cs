using System.Collections.Generic;
using TicTocGuardians.Scripts.Assets;
using TicTocGuardians.Scripts.Assets.LevelAsset;
using TicTocGuardians.Scripts.Game.LevelObjects;
using TicTocGuardians.Scripts.Game.Player;
using TMPro;
using UnityEngine;

namespace TicTocGuardians.Scripts.Game.Manager
{
    public class LevelManager : MonoBehaviour
    {
        public LevelSpawnPoint spawnPoint;
        public Transform world;
        public Transform origin;
        public LevelCamera levelCamera;
        public LevelLight levelLight;
        public TMP_Text timerText;
        public float timeLimit;
        public double currentTime;

        [HideInInspector]
        public double timeStep = 0.01d;

        public Player.Player[] playerPrefabs = new Player.Player[3];
        public List<Player.Player> playerInstances = new List<Player.Player>();

        public virtual void Start()
        {
            GlobalLoadingManager.Instance.ActiveScene();
        }

        public void LoadLevel(LevelAsset asset)
        {
            levelLight.Deserialize(asset.levelLight);
            levelCamera.Deserialize(asset.levelCamera);
            spawnPoint.Deserialize(asset.spawnPoint);
            timeLimit = asset.timeLimit;
            foreach (var obj in asset.objects)
            {
                var instance = Instantiate(obj.prefab, origin);
                instance.Deserialize(obj);
            }
        }

        public Player.Player SpawnPlayer(PlayerType type)
        {
           var instance = Instantiate(playerPrefabs[(int)type-1],origin);
           instance.transform.position = spawnPoint.transform.position;
           playerInstances.Add(instance);
           return instance;
        }
    }
}
