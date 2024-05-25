using System;
using System.Collections;
using Default.Scripts.Util;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TicTocGuardians.Scripts.Game.UI
{
    public class GlobalLoadingManager : Singleton<GlobalLoadingManager>
    {
        public enum LoadingMode
        {
            Default,
            Beaver,
            Rabbit,
            Cat
        }
        [SerializeField]
        private Image defaultLoadingScreen;
        [SerializeField]
        private Image beaverLoadingScreen;
        [SerializeField]
        private Image rabbitLoadingScreen;
        [SerializeField]
        private Image catLoadingScreen;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public IEnumerator Loading(string scene, float defaultDelay, LoadingMode mode = LoadingMode.Default)
        {
            var previousScene = SceneManager.GetActiveScene();
            var asyncOperation = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
            defaultLoadingScreen.gameObject.SetActive(false);
            beaverLoadingScreen.gameObject.SetActive(false);
            rabbitLoadingScreen.gameObject.SetActive(false);
            catLoadingScreen.gameObject.SetActive(false);
            switch (mode)
            {
                case LoadingMode.Default:
                    defaultLoadingScreen.gameObject.SetActive(true);
                    break;
                case LoadingMode.Beaver:
                    beaverLoadingScreen.gameObject.SetActive(true);
                    break;
                case LoadingMode.Rabbit:
                    rabbitLoadingScreen.gameObject.SetActive(true);
                    break;
                case LoadingMode.Cat:
                    catLoadingScreen.gameObject.SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }

            yield return new WaitForSeconds(defaultDelay);
            yield return new WaitUntil(() => asyncOperation.isDone);
            defaultLoadingScreen.gameObject.SetActive(false);
            beaverLoadingScreen.gameObject.SetActive(false);
            rabbitLoadingScreen.gameObject.SetActive(false);
            catLoadingScreen.gameObject.SetActive(false);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(scene));
            SceneManager.UnloadSceneAsync(previousScene.name);
        }
    }
}
