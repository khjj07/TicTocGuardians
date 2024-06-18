using TicTocGuardians.Scripts.Game.Manager;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace TicTocGuardians.Scripts.Game.LevelObjects
{
    public class SpawnPointChangerLevelObject : SpawnPointLevelObject
    {
        [SerializeField] private ParticleSystem[] spawnPointChangeParticlePrefab = new ParticleSystem[2];
        private int playCount;

        public override void Start()
        {
            base.Start();
            CreateChangeStream();
        }

        public override void Deserialize(LevelObjectAsset asset)
        {
            base.Deserialize(asset);
        }

        public override void Initialize(GameObject modelObject)
        {
            base.Initialize(modelObject);
        }

        public void CreateChangeStream()
        {
            GetComponentInChildren<MeshCollider>().OnCollisionEnterAsObservable()
                .Where(collision => collision.contacts[0].normal.y < -0.7)
                .First()
                .Subscribe(_ => ChangeSpawnPoint()).AddTo(gameObject);
        }

        public void ChangeSpawnPoint()
        {
            var manager = LevelManager.Instance as GeneralLevelManager;
            GlobalSoundManager.Instance.PlaySFX("SFX_NewStartPoint");
            var instance1 = Instantiate(spawnPointChangeParticlePrefab[0]);
            var instance2 = Instantiate(spawnPointChangeParticlePrefab[1]);
            instance1.transform.position = transform.position;
            instance2.transform.position = transform.position;
            this.UpdateAsObservable().Where(_ => playCount > LevelManager.Instance.playCount).First().Subscribe(_ =>
            {
                for (var i = manager._currentPlayPhaseIndex + 1; i < 3; i++) manager.currentSpawnPointIndices[i]--;
                CreateChangeStream();
            }).AddTo(gameObject);
            playCount = LevelManager.Instance.playCount;
            for (var i = manager._currentPlayPhaseIndex + 1; i < 3; i++) manager.currentSpawnPointIndices[i]++;
        }
    }
}