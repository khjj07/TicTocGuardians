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
    }
}
