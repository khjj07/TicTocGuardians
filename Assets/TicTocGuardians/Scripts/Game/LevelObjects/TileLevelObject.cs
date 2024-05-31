using TicTocGuardians.Scripts.Assets;
using TicTocGuardians.Scripts.Assets.LevelAsset;
using TicTocGuardians.Scripts.Game.Abstract;
using UnityEditor;
using UnityEngine;

namespace TicTocGuardians.Scripts.Game.LevelObjects
{
    [CustomEditor(typeof(TileLevelObject))]
    public class TileLevelObjectEditor : Editor
    {
        private Vector2 _scrollPosition;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            TileLevelObject obj = (TileLevelObject)target;


            var tileData = obj.data;
            GUILayout.Label("Tiles", EditorStyles.whiteLargeLabel);
            EditorGUILayout.BeginVertical();
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            for (int i = 0; i < obj.row; i++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int j = 0; j < obj.column; j++)
                {
                    if (tileData.Get(i, j))
                    {
                        GUI.backgroundColor = Color.green;
                    }
                    else
                    {
                        GUI.backgroundColor = Color.gray;
                    }

                    if (GUILayout.Button("", EditorStyles.helpBox, GUILayout.Width(50), GUILayout.Height(50)))
                    {

                        if (tileData.Get(i, j))
                        {
                            obj.data.Set(i, j, false);
                        }
                        else
                        {
                            obj.data.Set(i, j, true);
                        }
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
            if (GUI.changed)
            {
                obj.GenerateTile(obj.modelPrefab);
            }
        }
    }

    [ExecuteAlways]
    public class TileLevelObject : LevelObject
    {
        private LevelObject prefab;

        [SerializeField] private Vector3 tileOffset;
        [SerializeField] private Vector3 tileRotation;
        [SerializeField] private Vector3 tileScale=Vector3.one;
        [SerializeField] private bool blockAround = false;

        [Range(1, 100)]
        public int row;
        [Range(1, 100)]
        public int column;

        [Range(0.1f, 100)]
        public float offset = 1;

        public LevelTileData data;

        public void GenerateTile(GameObject modelPrefab)
        {
            prefab = GlobalLevelSetting.instance.defaultTilePrefab;

            var allChildren = transform.GetComponentsInChildren<StaticLevelObject>();
            foreach (var child in allChildren)
            {
                if (child != transform)
                {
                    DestroyImmediate(child.gameObject);
                }
            }

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    if (data.Get(i, j))
                    {
                        var instance = (LevelObject)PrefabUtility.InstantiatePrefab(prefab, transform);
                        instance.transform.localPosition = new Vector3(j * offset, 0, -i * offset)+ tileOffset;
                        instance.transform.localRotation = Quaternion.Euler(tileRotation);
                        instance.transform.localScale = tileScale;
                        instance.Initialize(modelPrefab);
                    }
                    else
                    {
                        if (blockAround)
                        {
                            CreateUnpassableBlock(j,i);
                        }
                    }
                }
            }

            if (blockAround)
            {
                for (int i = -1; i < column + 1; i++)
                {
                    CreateUnpassableBlock(i, -1);
                    CreateUnpassableBlock(i, row);
                }
                for (int i = -1; i < row + 1; i++)
                {
                    CreateUnpassableBlock(-1, i);
                    CreateUnpassableBlock(column, i);
                }
            }
        }

        public void CreateUnpassableBlock(int x, int z)
        {
            var instance = (LevelObject)PrefabUtility.InstantiatePrefab(GlobalLevelSetting.instance.unPassableLevelObject, transform);
            instance.transform.localPosition = new Vector3(x * offset, 0, -z * offset) + tileOffset;
            instance.transform.localRotation = Quaternion.Euler(tileRotation);
            var tmpScale = tileScale;
            tmpScale.x *= offset;
            tmpScale.z *= offset;
            instance.transform.localScale = tmpScale;
        }

        public override void Initialize(GameObject modelPrefab)
        {
            this.modelPrefab = modelPrefab;
        }

        public override LevelObjectAsset Serialize(LevelAsset parent)
        {
            var asset = new LevelObjectAsset();
            asset.name = gameObject.name;
            asset.position = transform.position;
            asset.eulerAngles = transform.eulerAngles;
            asset.scale = transform.localScale;
            asset.modelPrefab = modelPrefab;

            asset.AddData(parent, IntegerDataAsset.Create("row", row));
            asset.AddData(parent, IntegerDataAsset.Create("column", column));
            asset.AddData(parent, FloatDataAsset.Create("offset", offset));
            asset.AddData(parent, BoolDataAsset.Create("blockAround", blockAround));
            asset.AddData(parent, Bool2DArrayDataAsset.Create("data", data.ToArray(), row, column));
            asset.AddData(parent, Vector3DataAsset.Create("tileOffset",tileOffset));
            asset.AddData(parent, Vector3DataAsset.Create("tileRotation", tileRotation));
            asset.AddData(parent, Vector3DataAsset.Create("tileScale", tileScale));
            return asset;
        }

        public override void Deserialize(LevelObjectAsset asset)
        {
            gameObject.name = asset.name;
            transform.position = asset.position;
            transform.eulerAngles = asset.eulerAngles;
            transform.localScale = asset.scale;

            modelPrefab = asset.modelPrefab;

            row = (int)asset.GetValue("row");
            column = (int)asset.GetValue("column");
            offset = (float)asset.GetValue("offset");
            tileOffset = (Vector3)asset.GetValue("tileOffset");
            tileRotation = (Vector3)asset.GetValue("tileRotation");
            tileScale = (Vector3)asset.GetValue("tileScale");
            blockAround = (bool)asset.GetValue("blockAround");
            Bool2DArrayDataAsset tmp = asset.GetData("data") as Bool2DArrayDataAsset;
            data.FromArray(tmp.GetDataToArray(),row,column);

            GenerateTile(modelPrefab);
        }

    }
}
