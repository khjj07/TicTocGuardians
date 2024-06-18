using TicTocGuardians.Scripts.Assets;
using TicTocGuardians.Scripts.Assets.LevelAsset;
using TicTocGuardians.Scripts.Game.Manager;
using TicTocGuardians.Scripts.Interface;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace TicTocGuardians.Scripts.Game.LevelObjects
{
    public class ScaffoldingModelLevelObject : StaticModelLevelObject
    {
        public LevelObject reactableObject;
        public ParticleSystem[] buttonPressedParticlePrefab = new ParticleSystem[2];

        public void Start()
        {
            this.UpdateAsObservable().Select(_ =>
            {
                RaycastHit result;
                Physics.BoxCast(transform.position, Vector3.one / 2, Vector3.up, out result);
                return result.collider;
            }).DistinctUntilChanged().Skip(1).Subscribe(x =>
            {
                var obj = reactableObject as IReactable;
                obj.React();
                if (x != null)
                {
                    GlobalSoundManager.Instance.PlaySFX("SFX_Button");
                    CreateStepInParticle();
                }
            });
        }

        public void OnDrawGizmos()
        {
            var obj = reactableObject;
            if (obj != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, obj.transform.position);
            }
        }

        public override LevelObjectAsset Serialize(LevelAsset parent)
        {
            var instance = base.Serialize(parent);
            instance.AddData(parent, StringDataAsset.Create("reactableObject", reactableObject.name));
            return instance;
        }

        public override void Deserialize(LevelObjectAsset asset)
        {
            base.Deserialize(asset);
            reactableObject = GameObject.Find(asset.GetValue("reactableObject") as string).GetComponent<LevelObject>();
        }

        private void CreateStepInParticle()
        {
            var instance1 = Instantiate(buttonPressedParticlePrefab[0]);
            var instance2 = Instantiate(buttonPressedParticlePrefab[1]);
            instance1.transform.position = transform.position;
            instance2.transform.position = transform.position;
        }
    }
}