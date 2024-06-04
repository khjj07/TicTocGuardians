using Default.Scripts.Util;

namespace TicTocGuardians.Scripts.Game.LevelObjects
{
    public class SingleLevelObject<T> : LevelObject where T : LevelObject
    {
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    try
                    {
                        _instance = (T)FindObjectOfType<T>();
                    }
                    catch (System.Exception e)
                    {
                        UnityEngine.Debug.LogError(e.StackTrace);
                        return null;
                    }
                }

                return _instance;
            }
        }

        private static T _instance;
    }
}