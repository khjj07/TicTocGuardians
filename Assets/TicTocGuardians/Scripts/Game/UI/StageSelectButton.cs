using TicTocGuardians.Scripts.Assets;
using TicTocGuardians.Scripts.Game.Manager;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace TicTocGuardians.Scripts.Game.UI
{
    public class StageSelectButton : MonoBehaviour
    {
        public LevelPresetListAsset presetList;
        public StageExplainUI explainUI;
        public int index;
        public Button button;
        public TMP_Text text;

        public void Start()
        {
            button.onClick.AddListener(() =>
            {
                GlobalSoundManager.Instance.PlaySFX("SFX_UI_Select_1");
                GameManager.Instance.LoadLevel(presetList, index);
            });

            button.OnPointerEnterAsObservable().Subscribe(_ =>
            {
                explainUI.previewImage.sprite = presetList.presets[index].previewImage;
                explainUI.name = presetList.presets[index].name;
                explainUI.explain.SetText(presetList.presets[index].explain);
                ;
                explainUI.OnActive();
            }).AddTo(gameObject);


            button.OnPointerExitAsObservable().Subscribe(_ => { explainUI.OnInActive(); }).AddTo(gameObject);
        }
    }
}