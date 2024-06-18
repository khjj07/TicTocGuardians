using System;
using System.Collections.Generic;
using Default.Scripts.Extension;
using Default.Scripts.Util;
using UniRx;
using UnityEditor;
using UnityEngine;

namespace TicTocGuardians.Scripts.Game.Manager
{
    public class GlobalSoundManager : Singleton<GlobalSoundManager>
    {
#if UNITY_EDITOR
        [SerializeField] private DefaultAsset sfxFolder;
        [SerializeField] private DefaultAsset bgmFolder;
#endif
        [SerializeField] private AudioSource sfxAudioSource;
        [SerializeField] private AudioSource bgmAudioSource;
        private readonly Dictionary<string, AudioClip> _bgmDictionary = new();
        private readonly Dictionary<string, AudioClip> _sfxDictionary = new();

        public void Start()
        {

            foreach (var bgm in Resources.LoadAll<AudioClip>("Sounds/BGM"))
            {
                _bgmDictionary.Add(bgm.name, bgm);
            }

            foreach (var sfx in Resources.LoadAll<AudioClip>("Sounds/SFX"))
            {
                _sfxDictionary.Add(sfx.name, sfx);
            }
        }

        public void PlayBGM(string name)
        {
            bgmAudioSource.clip = _bgmDictionary[name];
            bgmAudioSource.Play();
        }

        public void StopBGM()
        {
            bgmAudioSource.Stop();
        }

        public void PlaySFX(string name, float delay = 0)
        {
            Observable.Timer(TimeSpan.FromSeconds(delay)).Subscribe(_ =>
            {
                sfxAudioSource.PlayOneShot(_sfxDictionary[name]);
            }).AddTo(gameObject);
        }
    }
}