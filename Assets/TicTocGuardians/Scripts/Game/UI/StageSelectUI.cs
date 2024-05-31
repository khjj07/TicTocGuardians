using System;
using TicTocGuardians.Scripts.Assets;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

namespace TicTocGuardians.Scripts.Game.UI
{
    [CustomEditor(typeof(StageSelectUI))]
    public class StageSelectUIInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUI.changed)
            {
                var ui = (StageSelectUI)target;
                foreach (var button in ui.frame.GetComponentsInChildren<StageSelectButton>(true))
                {
                    DestroyImmediate(button.gameObject);
                }
                foreach (var preset in ui.asset.presets)
                {
                    var instance = PrefabUtility.InstantiatePrefab(ui.buttonPrefab, ui.frame) as StageSelectButton;
                    instance.text.SetText(preset.text);
                    instance.levelAsset = preset.levelAsset;
                }
            }

            if (GUILayout.Button("Apply Changed"))
            {
                var ui = (StageSelectUI)target;
                foreach (var button in ui.frame.GetComponentsInChildren<StageSelectButton>(true))
                {
                    DestroyImmediate(button.gameObject);
                }
                foreach (var preset in ui.asset.presets)
                {
                    var instance = PrefabUtility.InstantiatePrefab(ui.buttonPrefab, ui.frame) as StageSelectButton;
                    instance.text.SetText(preset.text);
                    instance.levelAsset = preset.levelAsset;
                }
            }
           
        }
    }

    public class StageSelectUI : MonoBehaviour
    {
        public StageSelectButton buttonPrefab;
        public RectTransform frame;
        public StageSelectUIAsset asset;

        public void Start()
        {
            foreach (var button in frame.GetComponentsInChildren<StageSelectButton>(true))
            {
                Destroy(button.gameObject);
            }
            foreach (var preset in asset.presets)
            {
                var instance = Instantiate(buttonPrefab, frame);
                instance.text.SetText(preset.text);
                instance.levelAsset = preset.levelAsset;
            }
            gameObject.SetActive(false);
        }
    }
}
