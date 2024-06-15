using System;
using System.Collections;
using Default.Scripts.Util;
using TicTocGuardians.Scripts.Assets;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;


namespace TicTocGuardians.Scripts.Game.Manager
{
    public class GlobalLoadingManager : Singleton<GlobalLoadingManager>
    {
        public enum LoadingMode
        {
            Beaver,
            Rabbit,
            Cat
        }
        [SerializeField]
        private Image beaverLoadingScreen;
        [SerializeField]
        private Image rabbitLoadingScreen;
        [SerializeField]
        private Image catLoadingScreen;

        private Canvas _canvas;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            _canvas = GetComponent<Canvas>();
        }

        public void Update()
        {
            _canvas.worldCamera = Camera.current;
        }

        public void ActiveScene()
        {
            beaverLoadingScreen.gameObject.SetActive(false);
            rabbitLoadingScreen.gameObject.SetActive(false);
            catLoadingScreen.gameObject.SetActive(false);
        }

        public IEnumerator Load(string scene, float defaultDelay)
        {
            //var previousScene = SceneManager.GetActiveScene();
            GlobalSoundManager.Instance.StopBGM();
            var loadOperation = SceneManager.LoadSceneAsync(scene);
            loadOperation.allowSceneActivation = false;
            beaverLoadingScreen.gameObject.SetActive(false);
            rabbitLoadingScreen.gameObject.SetActive(false);
            catLoadingScreen.gameObject.SetActive(false);

            LoadingMode mode = (LoadingMode)Random.Range(0, 3);

            switch (mode)
            {
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
            yield return new WaitUntil(() => loadOperation.progress>=0.9f);
            GlobalSoundManager.Instance.PlaySFX("SFX_UI_Select_2");
            loadOperation.allowSceneActivation = true;
            //SceneManager.UnloadSceneAsync(previousScene.name);
        }
    }
}
