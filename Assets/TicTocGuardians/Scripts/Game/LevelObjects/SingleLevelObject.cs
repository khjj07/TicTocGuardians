using System;
using UnityEngine;

namespace TicTocGuardians.Scripts.Game.LevelObjects
{
    public class SingleLevelObject<T> : LevelObject where T : LevelObject
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                    try
                    {
                        _instance = FindObjectOfType<T>();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.StackTrace);
                        return null;
                    }

                return _instance;
            }
        }
    }
}