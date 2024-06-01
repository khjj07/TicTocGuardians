using System;
using Default.Scripts.Util;
using TicTocGuardians.Scripts.Assets;
using TicTocGuardians.Scripts.Assets.LevelAsset;
using UnityEngine;

namespace TicTocGuardians.Scripts.Game.Manager
{
    

    public class GameManager : Singleton<GameManager>
    {
      
        public LevelPresetListAsset normalPresetList;
        public LevelPresetListAsset hardPresetList;

        private LevelPresetListAsset _currentLevelPresetListAsset;
        private int _currentIndex;
        public void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void ActiveLevel(LevelManager manager)
        {
            manager.LoadLevel(_currentLevelPresetListAsset.GetLevel(_currentIndex).levelAsset);
        }

        public int GetCurrentIndex()
        {
            return _currentIndex;
        }

        public void LoadLevel(int index)
        {
            LoadLevel(_currentLevelPresetListAsset, index);
        }

        public void LoadLevel(LevelPresetListAsset levelPresetList, int index)
        {
            _currentLevelPresetListAsset=levelPresetList;
            _currentIndex=index;
            var levelPreset = _currentLevelPresetListAsset.GetLevel(index);
            StartCoroutine(GlobalLoadingManager.Instance.Load(levelPreset.levelAsset.scene.name, 1.0f));
        }
    }
}
