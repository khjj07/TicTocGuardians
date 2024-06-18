using TicTocGuardians.Scripts.Assets;
using UnityEngine;

namespace TicTocGuardians.Scripts.Game.UI
{
    public class StageSelectUI : MonoBehaviour
    {
        public StageSelectButton buttonPrefab;
        public RectTransform frame;
        public StageExplainUI explainUI;
        public LevelPresetListAsset listAsset;

        public void Start()
        {
            explainUI.gameObject.SetActive(false);
            foreach (var button in frame.GetComponentsInChildren<StageSelectButton>(true)) Destroy(button.gameObject);
            var count = 0;
            foreach (var preset in listAsset.presets)
            {
                var instance = Instantiate(buttonPrefab, frame);
                instance.text.SetText(preset.name);
                instance.presetList = listAsset;
                instance.explainUI = explainUI;
                instance.index = count;
                count++;
            }

            gameObject.SetActive(false);
        }
    }
}