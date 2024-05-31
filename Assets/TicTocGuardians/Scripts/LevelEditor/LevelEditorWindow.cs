using Default.Scripts.Extension;
using System.Collections.Generic;
using TicTocGuardians.Scripts.Assets;
using TicTocGuardians.Scripts.Assets.LevelAsset;
using TicTocGuardians.Scripts.Game;
using TicTocGuardians.Scripts.Game.Abstract;
using UnityEditor;
using UnityEngine;

namespace TicTocGuardians.Scripts.LevelEditor
{
    public class LevelEditorWindow : EditorWindow
    {
        static LevelEditorWindow window;

        private Vector2 _scrollPosition;
        private static int _tabIndex = 0;
        private int _createGridIndex = 0;
        readonly string[] _tabSubject = { "Create", "Level" };
        private GameObject[] objects;
        private LevelObject prefab;
        private LevelAsset _targetLevelAsset;

        public static void Open(Scripts.LevelEditor.LevelEditor tool)
        {
            if (window == null)
            {
                window = CreateInstance<LevelEditorWindow>();
            }

            window.objects = LevelEditor.Instance.modelsFolder.LoadAllObjectsInFolder<GameObject>().ToArray();
            Debug.Log(LevelEditor.Instance.modelsFolder.GetLocalPath());
            window.Show();
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

                prefab = (LevelObject)EditorGUILayout.ObjectField("Level Object", prefab, typeof(LevelObject), true);

                if (GUILayout.Button("Create Level Object", EditorStyles.miniButton) && prefab)
                {
                    var instance = PrefabUtility.InstantiatePrefab(prefab, Scripts.LevelEditor.LevelEditor.Instance.origin) as LevelObject;

                    instance.Initialize(objects[_createGridIndex] as GameObject);

                    Selection.activeObject = instance;
                }
            }
            EditorGUILayout.EndVertical();




            EditorGUILayout.BeginVertical();
            {
                _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                if (objects != null)
                {
                    List<GUIContent> contents = new List<GUIContent>();
                    foreach (var obj in objects)
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

                LevelObject[] allChildren = Scripts.LevelEditor.LevelEditor.Instance.origin.GetComponentsInChildren<LevelObject>(true);
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

                if (LevelEditor.Instance.spawnPoint)
                {
                    levelasset.spawnPoint = LevelEditor.Instance.spawnPoint.Serialize(levelasset);
                }

                if (LevelEditor.Instance.levelCamera)
                {
                    levelasset.levelCamera =LevelEditor.Instance.levelCamera.Serialize(levelasset);
                }

                if (LevelEditor.Instance.levelLight)
                {
                    levelasset.levelLight = LevelEditor.Instance.levelLight.Serialize(levelasset);
                }


                foreach (var levelObject in allChildren)
                {
                    if (levelObject.transform.parent == Scripts.LevelEditor.LevelEditor.Instance.origin)
                    {
                        var instance = levelObject.Serialize(levelasset);
                        instance.prefab = PrefabUtility.GetCorrespondingObjectFromSource(levelObject);

                        assets.Add(instance);
                    }
                }

                levelasset.objects = assets.ToArray();

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

                if (Scripts.LevelEditor.LevelEditor.Instance.spawnPoint)
                {
                    Scripts.LevelEditor.LevelEditor.Instance.spawnPoint.Deserialize(_targetLevelAsset.spawnPoint);
                }
                if (LevelEditor.Instance.levelCamera)
                {
                    LevelEditor.Instance.levelCamera.Deserialize(_targetLevelAsset.levelCamera);
                }
                if (LevelEditor.Instance.levelLight)
                {
                    LevelEditor.Instance.levelLight.Deserialize(_targetLevelAsset.levelLight);
                }

                List<LevelObject> instances = new List<LevelObject>();
                foreach (var levelObjectAsset in _targetLevelAsset.objects)
                {
                    var instance = PrefabUtility.InstantiatePrefab(levelObjectAsset.prefab, Scripts.LevelEditor.LevelEditor.Instance.origin) as LevelObject;
                    instance.Deserialize(levelObjectAsset);
                    instances.Add(instance);
                }
            }
        }
    }
}