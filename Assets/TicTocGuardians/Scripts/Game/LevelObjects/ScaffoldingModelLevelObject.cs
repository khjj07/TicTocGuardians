using System;
using TicTocGuardians.Scripts.Assets;
using TicTocGuardians.Scripts.Assets.LevelAsset;
using TicTocGuardians.Scripts.Game.Manager;
using TicTocGuardians.Scripts.Interface;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Events;

namespace TicTocGuardians.Scripts.Game.LevelObjects
{
    public class ScaffoldingModelLevelObject : StaticModelLevelObject
    {
        public LevelObject reactableObject;
        public ParticleSystem[] buttonPressedParticlePrefab =new ParticleSystem[2];
        public override LevelObjectAsset Serialize(LevelAsset parent)
        {
            var instance = base.Serialize(parent);
            instance.AddData(parent,StringDataAsset.Create("reactableObject", reactableObject.name));
            return instance;
        }

        public override void Deserialize(LevelObjectAsset asset)
        {
            base.Deserialize(asset);
            reactableObject = GameObject.Find(asset.GetValue("reactableObject") as string).GetComponent<LevelObject>();
        }

        public void Start()
        {
            GetComponentInChildren<BoxCollider>().OnCollisionEnterAsObservable()
                .Where(collision => collision.contacts[0].normal.y < -0.7)
                .ThrottleFirst(TimeSpan.FromSeconds(0.1f))
                .Select(_=> reactableObject as IReactable)
                .Subscribe(x=>
                {
                    GlobalSoundManager.Instance.PlaySFX("SFX_Button");
                    CreateStepInParticle();
                    x.React();
                });

            GetComponentInChildren<BoxCollider>().OnCollisionExitAsObservable()
                .Select(_ => reactableObject as IReactable)
                .Subscribe(x => x.React());
        }

        private void CreateStepInParticle()
        {
            var instance1 = Instantiate(buttonPressedParticlePrefab[0]);
            var instance2 = Instantiate(buttonPressedParticlePrefab[1]);
            instance1.transform.position = transform.position;
            instance2.transform.position = transform.position;
        }

        public void OnDrawGizmos()
        {
            var obj = reactableObject as LevelObject;
            if (obj != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, obj.transform.position);
            }
        }
    }
}
