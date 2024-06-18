using Default.Scripts.Util;
using TicTocGuardians.Scripts.Assets;
using TicTocGuardians.Scripts.Assets.LevelAsset;
using UnityEditor;

namespace TicTocGuardians.Scripts.Game.Manager
{
    public class GameManager : Singleton<GameManager>
    {
        public LevelPresetListAsset normalPresetList;
        public LevelPresetListAsset hardPresetList;
        private int _currentIndex;

        private LevelPresetListAsset _currentLevelPresetListAsset;

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
            _currentLevelPresetListAsset = levelPresetList;
            _currentIndex = index;
            var levelPreset = _currentLevelPresetListAsset.GetLevel(index);
            if (levelPreset.levelAsset as TutorialLevelAsset != null)
                StartCoroutine(GlobalLoadingManager.Instance.Load("TutorialPlayScene", 3.0f));
            else
                StartCoroutine(GlobalLoadingManager.Instance.Load("GeneralPlayScene", 3.0f));
        }
    }
}