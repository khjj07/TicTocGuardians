using TicTocGuardians.Scripts.Assets;
using UnityEditor;
using UnityEngine;

namespace TicTocGuardians.Scripts.Game
{
    public class LevelSpawnPoint : BaseObject<BaseObjectAsset>
    {
        public override BaseObjectAsset Serialize(PlaceAsset parent)
        {
            var asset = new BaseObjectAsset();
            asset.name = gameObject.name;
            asset.position = transform.position;
            asset.eulerAngles = transform.eulerAngles;
            asset.scale = transform.localScale;
            return asset;
        }

        public override void Deserialize(BaseObjectAsset asset)
        {
            gameObject.name = asset.name;
            transform.position = asset.position;
            transform.eulerAngles = asset.eulerAngles;
            transform.localScale = asset.scale;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = new Color(0, 1, 0, 0.5f);
            Gizmos.DrawMesh(GlobalLevelSetting.instance.playerMesh, transform.position,
                transform.rotation, Vector3.one * 100);
            GUIStyle spawnStyle = new GUIStyle();
            spawnStyle.fontSize = 20;
            spawnStyle.alignment = TextAnchor.MiddleCenter;
            spawnStyle.normal.textColor = Color.green;
            Handles.Label(transform.position + Vector3.up * 5f, "Spawn Point", spawnStyle);
        }
    }
}
