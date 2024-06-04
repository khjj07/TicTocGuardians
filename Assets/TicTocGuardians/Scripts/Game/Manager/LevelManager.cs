using System.Collections.Generic;
using Default.Scripts.Util;
using TicTocGuardians.Scripts.Assets;
using TicTocGuardians.Scripts.Assets.LevelAsset;
using TicTocGuardians.Scripts.Game.LevelObjects;
using TicTocGuardians.Scripts.Game.Player;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using static UnityEngine.UI.Image;

namespace TicTocGuardians.Scripts.Game.Manager
{
    public class LevelManager : Singleton<LevelManager>
    {
        public Transform world;
        public Transform origin;
        public TMP_Text timerText;
        public float timeLimit;
        public double currentTime;

        public Player.Player[] playerPrefabs = new Player.Player[3];
        public List<Player.Player> playerInstances = new List<Player.Player>();
        public List<DimensionLevelObject> dimensions = new List<DimensionLevelObject>();

        [HideInInspector]
        public double timeStep = 0.01d;

        public virtual void Start()
        {
            GlobalLoadingManager.Instance.ActiveScene();
        }

        public virtual void LoadLevel(LevelAsset asset)
        {
            timeLimit = asset.timeLimit;
            foreach (var obj in asset.objects)
            {
                var instance = Instantiate(obj.prefab, origin);
                instance.Deserialize(obj);
            }
            SpawnPointLevelObject.Instance.Deserialize(asset.spawnPoint);
            CameraLevelObject.Instance.Deserialize(asset.camera);
            LightLevelObject.Instance.Deserialize(asset.light);
        }

        public Player.Player SpawnPlayer(PlayerType type)
        {
            var instance = Instantiate(playerPrefabs[(int)type - 1], origin);
            instance.transform.position = SpawnPointLevelObject.Instance.transform.position;
            CreateDimensionCheckStream(instance);
            playerInstances.Add(instance);
            return instance;
        }

        public void AddRepairDimension(DimensionLevelObject repairTarget)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveRepairDimension(DimensionLevelObject repairTarget)
        {
            throw new System.NotImplementedException();
        }

        public void CreateDimensionCheckStream(Player.Player player)
        {

           //var dimensionCheck = player.UpdateAsObservable().Select(_ =>
           //{
           //    RaycastHit hit;
           //    if (Physics.Raycast(transform.position, transform.forward * player.dimensionCheckDistance, out hit))
           //    {
           //        return hit.collider.GetComponent<DimensionLevelObject>();
           //    }
           //    return null;
           //});
           //
           //dimensionCheck.Subscribe(x =>
           //{
           //    if (x != null)
           //    {
           //        player.repairTarget = x;
           //        AddRepairDimension(player.repairTarget);
           //    }
           //    else
           //    {
           //        RemoveRepairDimension(player.repairTarget);
           //        player.repairTarget = null;
           //    }
           //
           //}).AddTo(player.gameObject);
        }
    }
}
