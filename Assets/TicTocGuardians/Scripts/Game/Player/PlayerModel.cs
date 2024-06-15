using TicTocGuardians.Scripts.Game.Manager;
using UnityEngine;

namespace TicTocGuardians.Scripts.Game.Player
{
    public class PlayerModel : MonoBehaviour
    {
        [SerializeField] protected ParticleSystem[] walkParticlePrefabs = new ParticleSystem[3];
        [SerializeField] protected ParticleSystem[] landingParticlePrefabs = new ParticleSystem[2];
        [SerializeField] private Transform footOrigin;

        public void CreateWalkParticle()
        {
            var instance = Instantiate(walkParticlePrefabs[Random.Range(0, walkParticlePrefabs.Length)]);
            instance.transform.position = footOrigin.position;
            GlobalSoundManager.Instance.PlaySFX("SFX_Footstep-001");
        }

        public void CreateLandingParticle()
        {
            var instance1 = Instantiate(landingParticlePrefabs[0]);
            var instance2 = Instantiate(landingParticlePrefabs[1]);
            instance1.transform.position = footOrigin.position;
            instance2.transform.position = footOrigin.position;
            GlobalSoundManager.Instance.PlaySFX("SFX_Land_2");
        }
    }
}
