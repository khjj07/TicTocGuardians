using System;
using TicTocGuardians.Scripts.Assets;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Serialization;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

namespace TicTocGuardians.Scripts.Game.UI
{
    
    public class StageSelectUI : MonoBehaviour
    {
        public StageSelectButton buttonPrefab;
        public RectTransform frame;
        public LevelPresetListAsset listAsset;

        public void Start()
        {
            foreach (var button in frame.GetComponentsInChildren<StageSelectButton>(true))
            {
                Destroy(button.gameObject);
            }

            int count = 0;
            foreach (var preset in listAsset.presets)
            {
                var instance = Instantiate(buttonPrefab, frame);
                instance.text.SetText(preset.name);
                instance.presetList = listAsset;
                instance.index = count;
                count++;
            }
            gameObject.SetActive(false);
        }
    }
}
