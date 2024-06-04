using TicTocGuardians.Scripts.Assets;
using TicTocGuardians.Scripts.Assets.LevelAsset;
using UnityEditor;
using UnityEngine;

namespace TicTocGuardians.Scripts.Game.LevelObjects
{
    public class SpawnPointLevelObject : SingleLevelObject<SpawnPointLevelObject>
    {
        void OnDrawGizmos()
        {
            Gizmos.color = new Color(0, 1, 0, 0.5f);
            Gizmos.DrawMesh(GlobalLevelSetting.instance.playerMesh, transform.position,
                transform.rotation, Vector3.one);
            GUIStyle spawnStyle = new GUIStyle();
            spawnStyle.fontSize = 20;
            spawnStyle.alignment = TextAnchor.MiddleCenter;
            spawnStyle.normal.textColor = Color.green;
            Handles.Label(transform.position + Vector3.up, "Spawn Point", spawnStyle);
        }
    }
}
