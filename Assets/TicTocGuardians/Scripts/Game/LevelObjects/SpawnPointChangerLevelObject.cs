using TicTocGuardians.Scripts.Assets.LevelAsset;
using TicTocGuardians.Scripts.Assets;
using TicTocGuardians.Scripts.Interface;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using TicTocGuardians.Scripts.Game.Manager;

namespace TicTocGuardians.Scripts.Game.LevelObjects
{
    public class SpawnPointChangerLevelObject : StaticModelLevelObject
    {
        [SerializeField] private ParticleSystem[] spawnPointChangeParticlePrefab = new ParticleSystem[2];
        public void Start()
        {
            GetComponentInChildren<BoxCollider>().OnCollisionEnterAsObservable()
                .Where(collision => collision.contacts[0].normal.y < -0.7)
                .First()
                .Subscribe(_ => ChangeSpawnPoint());
        }

        public void ChangeSpawnPoint()
        {
            GlobalSoundManager.Instance.PlaySFX("SFX_NewStartPoint");
            SpawnPointLevelObject.Instance.transform.position = transform.position + Vector3.up * 3;
            var instance1 = Instantiate(spawnPointChangeParticlePrefab[0]);
            var instance2 = Instantiate(spawnPointChangeParticlePrefab[1]);
            instance1.transform.position = transform.position;
            instance2.transform.position = transform.position;
        }
    }
}