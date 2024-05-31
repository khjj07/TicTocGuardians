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
        public Button button;
        public TMP_Text text;
        public LevelAsset levelAsset;

        public void Start()
        {
            button.onClick.AddListener(() =>
            {
                GameManager.Instance.LoadLevel(levelAsset);
                StartCoroutine(GlobalLoadingManager.Instance.Load(levelAsset.scene.name, 1.0f));
            });
        }
    }
}
