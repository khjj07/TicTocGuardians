using System;
using Default.Scripts.Util;
using TicTocGuardians.Scripts.Assets.LevelAsset;
using UnityEngine;

namespace TicTocGuardians.Scripts.Game.Manager
{
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField]
        private LevelManager currentLevelManager;

        private LevelAsset _levelAssetBuffer;
        public void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void SetLevelManager(LevelManager manager)
        {
            currentLevelManager = manager;
            currentLevelManager.LoadLevel(_levelAssetBuffer);
            _levelAssetBuffer=null;
        }

        public void LoadLevel(LevelAsset levelAsset)
        {
            _levelAssetBuffer = levelAsset;
        }
    }
}
