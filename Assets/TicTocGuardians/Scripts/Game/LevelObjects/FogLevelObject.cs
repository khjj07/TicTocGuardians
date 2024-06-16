using UnityEditor;
using UnityEngine;

namespace TicTocGuardians.Scripts.Game.LevelObjects
{
    namespace TicTocGuardians.Scripts.Game.LevelObjects
    {
        public class FogLevelObject : LevelObject
        {
            public bool simulate = true;
        }

        [CustomEditor(typeof(FogLevelObject))]
        public class FogLevelObjectInspector : Editor
        {
            
            public void OnValidate()
            {
                var levelObject = (FogLevelObject)target;
                if (levelObject.simulate)
                {
                    var ps = levelObject.GetComponent<ParticleSystem>();
                    ps.Simulate(1,true,true); //set value and play straightly
                }
            }
        }
    }
}