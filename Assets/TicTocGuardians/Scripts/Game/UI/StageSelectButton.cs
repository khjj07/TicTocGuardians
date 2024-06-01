using TicTocGuardians.Scripts.Assets;
using TicTocGuardians.Scripts.Assets.LevelAsset;
using TicTocGuardians.Scripts.Game.Manager;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace TicTocGuardians.Scripts.Game.UI
{
    public class StageSelectButton : MonoBehaviour
    {
        public LevelPresetListAsset presetList;
        public int index;
        public Button button;
        public TMP_Text text;

        public void Start()
        {
            button.onClick.AddListener(() =>
            {
                GameManager.Instance.LoadLevel(presetList,index);
            });
        }
    }
}
