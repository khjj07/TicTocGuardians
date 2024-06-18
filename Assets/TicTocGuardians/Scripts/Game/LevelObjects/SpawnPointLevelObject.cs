using TicTocGuardians.Scripts.Assets;
using TicTocGuardians.Scripts.Game.Manager;
using UnityEditor;
using UnityEngine;

namespace TicTocGuardians.Scripts.Game.LevelObjects
{
    public class SpawnPointLevelObject : StaticModelLevelObject
    {
        public virtual void Start()
        {
            if (LevelManager.Instance) LevelManager.Instance.spawnPoints.Add(this);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0, 1, 0, 0.5f);
            Gizmos.DrawMesh(GlobalLevelSetting.instance.playerMesh, transform.position + Vector3.up * 3,
                transform.rotation, Vector3.one);
            var spawnStyle = new GUIStyle();
            spawnStyle.fontSize = 20;
            spawnStyle.alignment = TextAnchor.MiddleCenter;
            spawnStyle.normal.textColor = Color.green;
            Handles.Label(transform.position + Vector3.up, "Spawn Point", spawnStyle);
        }
#endif
    }
}