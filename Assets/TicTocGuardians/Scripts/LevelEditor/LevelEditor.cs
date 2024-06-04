using System.Collections.Generic;
using Default.Scripts.Util;
using TicTocGuardians.Scripts.Game;
using TicTocGuardians.Scripts.Game.LevelObjects;
using TicTocGuardians.Scripts.Game.Player;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace TicTocGuardians.Scripts.LevelEditor
{
    public class LevelEditor : Singleton<LevelEditor>
    {
        public DefaultAsset modelsFolder;
        public Transform origin;
    }
}