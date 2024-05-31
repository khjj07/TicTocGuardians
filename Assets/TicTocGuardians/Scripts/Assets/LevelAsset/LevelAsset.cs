using TicTocGuardians.Scripts.Game;
using TicTocGuardians.Scripts.Game.Abstract;
using UnityEditor;
using UnityEngine;

namespace TicTocGuardians.Scripts.Assets.LevelAsset
{
    public class LevelAsset : ScriptableObject
    {
        [Header("Setting")] 
        public SceneAsset scene;
        public float timeLimit;

        [Header("Objects")]
        public BaseObjectAsset spawnPoint;
        public BaseObjectAsset levelCamera;
        public BaseObjectAsset levelLight;
        public LevelObjectAsset[] objects;
    }
}
