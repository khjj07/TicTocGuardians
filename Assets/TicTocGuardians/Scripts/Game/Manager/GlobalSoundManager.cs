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
        [SerializeField] private DefaultAsset sfxFolder;
        [SerializeField] private DefaultAsset bgmFolder;
        [SerializeField] private AudioSource sfxAudioSource;
        [SerializeField] private AudioSource bgmAudioSource;
        private Dictionary<string,AudioClip> _bgmDictionary = new Dictionary<string, AudioClip>();
        private Dictionary<string, AudioClip> _sfxDictionary = new Dictionary<string, AudioClip>();

        public void Awake()
        {
            foreach (var bgm in bgmFolder.LoadAllObjectsInFolder<AudioClip>())
            {
                _bgmDictionary.Add(bgm.name, bgm);
            }
            foreach (var bgm in sfxFolder.LoadAllObjectsInFolder<AudioClip>())
            {
                _sfxDictionary.Add(bgm.name, bgm);
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

        public void PlaySFX(string name, float delay=0)
        {
            Observable.Timer(TimeSpan.FromSeconds(delay)).Subscribe(_ =>
            {
                sfxAudioSource.PlayOneShot(_sfxDictionary[name]);
            }).AddTo(gameObject);
        }
    }
}
