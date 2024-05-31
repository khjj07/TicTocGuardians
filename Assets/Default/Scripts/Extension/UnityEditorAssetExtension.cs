using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Default.Scripts.Extension
{
    public static class UnityEditorAssetExtensions
    {
        /// <summary> 폴더 애셋으로부터 Assets로 시작하는 로컬 경로 얻기 </summary>
        public static string GetLocalPath(this UnityEditor.DefaultAsset @this)
        {
            bool success =
                UnityEditor.AssetDatabase.TryGetGUIDAndLocalFileIdentifier(@this, out string guid, out long _);

            if (success)
                return UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            else
                return null;
        }

        /// <summary> 폴더 애셋으로부터 절대 경로 얻기 </summary>
        public static string GetAbsolutePath(this UnityEditor.DefaultAsset @this)
        {
            string path = GetLocalPath(@this);
            if (path == null)
                return null;

            path = path.Substring(path.IndexOf('/') + 1);
            return Application.dataPath + "/" + path;
        }

        /// <summary> 폴더 애셋으로부터 DirectoryInfo 객체 얻기 </summary>
        public static System.IO.DirectoryInfo GetDirectoryInfo(this DefaultAsset @this)
        {
            string absPath = GetAbsolutePath(@this);
            return (absPath != null) ? new System.IO.DirectoryInfo(absPath) : null;
        }

        public static List<T> LoadAllObjectsInFolder<T>(this UnityEditor.DefaultAsset @this) where T : class
        {
            List<T> assets =new List<T>();
            // 폴더 내 모든 자산의 GUID 배열을 가져옴
            string[] assetGUIDs = AssetDatabase.FindAssets("", new[] { GetLocalPath(@this) });

            foreach (string guid in assetGUIDs)
            {
                // GUID를 경로로 변환
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                // 자산을 Object 타입으로 로드
                if (AssetDatabase.LoadAssetAtPath<Object>(assetPath) is T tmp)
                {
                    assets.Add(tmp);
                }
               

            }

            return assets;
        }
    }
}
