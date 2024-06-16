using System;
using System.Collections;
using Default.Scripts.Util;
using TicTocGuardians.Scripts.Assets;
using TicTocGuardians.Scripts.Game.Player;
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
        private Canvas beaverLoadingScreen;
        [SerializeField]
        private Canvas rabbitLoadingScreen;
        [SerializeField]
        private Canvas catLoadingScreen;
        
        [SerializeField]
        private PlayerPreview beaverPreview;
        [SerializeField]
        private PlayerPreview rabbitPreview;
        [SerializeField]
        private PlayerPreview catPreview;

        [SerializeField]
        private Camera modelCamera;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            beaverLoadingScreen.gameObject.SetActive(false);
            rabbitLoadingScreen.gameObject.SetActive(false);
            catLoadingScreen.gameObject.SetActive(false);
            modelCamera.gameObject.SetActive(false);
        }

        public void ActiveScene()
        {
            beaverLoadingScreen.gameObject.SetActive(false);
            rabbitLoadingScreen.gameObject.SetActive(false);
            catLoadingScreen.gameObject.SetActive(false);
            modelCamera.gameObject.SetActive(false);
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
            beaverPreview.Wait();
            rabbitPreview.Wait();
            catPreview.GetComponentInChildren<PlayerPreview>().Wait();
            modelCamera.gameObject.SetActive(true);
            LoadingMode mode = (LoadingMode)Random.Range(0, 3);

            switch (mode)
            {
                case LoadingMode.Beaver:
                    beaverLoadingScreen.gameObject.SetActive(true);
                    beaverPreview.Pick();
                    break;
                case LoadingMode.Rabbit:
                    rabbitLoadingScreen.gameObject.SetActive(true);
                    rabbitPreview.Pick();
                    break;
                case LoadingMode.Cat:
                    catLoadingScreen.gameObject.SetActive(true);
                    catPreview.Pick();
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
