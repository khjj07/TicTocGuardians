using Default.Scripts.Extension;
using System.Collections.Generic;
using TicTocGuardians.Scripts.Assets;
using TicTocGuardians.Scripts.Assets.LevelAsset;
using TicTocGuardians.Scripts.Game;
using TicTocGuardians.Scripts.Game.LevelObjects;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace TicTocGuardians.Scripts.LevelEditor
{
    public class LevelEditorWindow : EditorWindow
    {
        private static LevelEditorWindow _window;

        private Vector2 _scrollPosition;
        private static int _tabIndex = 0;
        private int _createGridIndex = 0;
        readonly string[] _tabSubject = { "Create", "Level" };
        private GameObject[] _objects;
        private ModelLevelObject _prefab;
        private LevelAsset _targetLevelAsset;

        public static void Open(Scripts.LevelEditor.LevelEditor tool)
        {
            if (_window == null)
            {
                _window = CreateInstance<LevelEditorWindow>();
            }

            _window._objects = LevelEditor.Instance.modelsFolder.LoadAllObjectsInFolder<GameObject>().ToArray();
            Debug.Log(LevelEditor.Instance.modelsFolder.GetLocalPath());
            _window.Show();
        }


        public void OnGUI()
        {
            _tabIndex = GUILayout.Toolbar(_tabIndex, _tabSubject);
            switch (_tabIndex)
            {
                case 0:
                    OnGUI_Create();
                    break;
                case 1:
                    OnGUI_Level();
                    break;
            }
        }

        private void OnGUI_Create()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            {

                _prefab = (ModelLevelObject)EditorGUILayout.ObjectField("Level Object", _prefab, typeof(ModelLevelObject), true);

                if (GUILayout.Button("Create Level Object", EditorStyles.miniButton) && _prefab)
                {
                    var instance = PrefabUtility.InstantiatePrefab(_prefab, Scripts.LevelEditor.LevelEditor.Instance.origin) as ModelLevelObject;

                    instance.Initialize(_objects[_createGridIndex] as GameObject);

                    Selection.activeObject = instance;
                }
            }
            EditorGUILayout.EndVertical();




            EditorGUILayout.BeginVertical();
            {
                _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                if (_objects != null)
                {
                    List<GUIContent> contents = new List<GUIContent>();
                    foreach (var obj in _objects)
                    {
                        var content = new GUIContent(obj.name, (Texture)AssetPreview.GetAssetPreview(obj)); // file name in the resources folder without the (.png) extension
                        contents.Add(content);
                    }

                    _createGridIndex = GUILayout.SelectionGrid(_createGridIndex, contents.ToArray(), 1, EditorStyles.objectFieldThumb, GUILayout.Width(300));
                }

                GUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();


            // file name in the resources folder without the (.png) extension

            //if (EditorGUILayout.DropdownButton(assetContents))
        }

        private void OnGUI_Level()
        {
            _targetLevelAsset =
                (LevelAsset)EditorGUILayout.ObjectField("Level Object", _targetLevelAsset, typeof(LevelAsset), true);


            if (GUILayout.Button("Save Level") && _targetLevelAsset)
            {
                LevelAsset levelasset = _targetLevelAsset;

                LevelObject[] allChildren = LevelEditor.Instance.origin.GetComponentsInChildren<LevelObject>(true);
                List<LevelObjectAsset> assets = new List<LevelObjectAsset>();
                var objects = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(levelasset));

                foreach (var child in objects)
                {
                    if (child != levelasset)
                    {
                        AssetDatabase.RemoveObjectFromAsset(child);
                    }
                }
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                foreach (var levelObject in allChildren)
                {
                    if (levelObject.transform.parent == LevelEditor.Instance.origin)
                    {
                        var instance = levelObject.Serialize(levelasset);
                        instance.prefab = PrefabUtility.GetCorrespondingObjectFromSource(levelObject);
                        assets.Add(instance);
                    }
                }
                levelasset.objects = assets.ToArray();
                levelasset.camera = CameraLevelObject.Instance.Serialize(levelasset);
                levelasset.light = LightLevelObject.Instance.Serialize(levelasset);


                EditorUtility.SetDirty(levelasset);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            if (GUILayout.Button("Load Level") && _targetLevelAsset)
            {
                LevelObject[] allChildren = Scripts.LevelEditor.LevelEditor.Instance.origin.GetComponentsInChildren<LevelObject>(true);

                foreach (var child in allChildren)
                {
                    if (child != null)
                    {
                        DestroyImmediate(child.gameObject);
                    }
                }

                List<LevelObject> instances = new List<LevelObject>();
                foreach (var levelObjectAsset in _targetLevelAsset.objects)
                {
                    var instance = PrefabUtility.InstantiatePrefab(levelObjectAsset.prefab, LevelEditor.Instance.origin) as LevelObject;
                    instance.Deserialize(levelObjectAsset);
                    instances.Add(instance);
                }
                CameraLevelObject.Instance.Deserialize(_targetLevelAsset.camera);
                LightLevelObject.Instance.Deserialize(_targetLevelAsset.light);
            }
        }
    }
}