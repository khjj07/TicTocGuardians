using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Default.Scripts.Extension
{
    public static class UnityEditorAssetExtensions
    {
        /// <summary> ���� �ּ����κ��� Assets�� �����ϴ� ���� ��� ��� </summary>
        public static string GetLocalPath(this UnityEditor.DefaultAsset @this)
        {
            bool success =
                UnityEditor.AssetDatabase.TryGetGUIDAndLocalFileIdentifier(@this, out string guid, out long _);

            if (success)
                return UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            else
                return null;
        }

        /// <summary> ���� �ּ����κ��� ���� ��� ��� </summary>
        public static string GetAbsolutePath(this UnityEditor.DefaultAsset @this)
        {
            string path = GetLocalPath(@this);
            if (path == null)
                return null;

            path = path.Substring(path.IndexOf('/') + 1);
            return Application.dataPath + "/" + path;
        }

        /// <summary> ���� �ּ����κ��� DirectoryInfo ��ü ��� </summary>
        public static System.IO.DirectoryInfo GetDirectoryInfo(this DefaultAsset @this)
        {
            string absPath = GetAbsolutePath(@this);
            return (absPath != null) ? new System.IO.DirectoryInfo(absPath) : null;
        }

        public static List<T> LoadAllObjectsInFolder<T>(this UnityEditor.DefaultAsset @this) where T : class
        {
            List<T> assets =new List<T>();
            // ���� �� ��� �ڻ��� GUID �迭�� ������
            string[] assetGUIDs = AssetDatabase.FindAssets("", new[] { GetLocalPath(@this) });

            foreach (string guid in assetGUIDs)
            {
                // GUID�� ��η� ��ȯ
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                // �ڻ��� Object Ÿ������ �ε�
                if (AssetDatabase.LoadAssetAtPath<Object>(assetPath) is T tmp)
                {
                    assets.Add(tmp);
                }
               

            }

            return assets;
        }
    }
}
