using TicTocGuardians.Scripts.Interface;
using UnityEngine;

namespace TicTocGuardians.Scripts.Game.ETC
{
    public class BeaverBox : MonoBehaviour, IMovable
    {
        public void MoveX(float speed)
        {
            transform.Translate(Vector3.right* speed);
        }

        public void MoveZ(float speed)
        {
            transform.Translate(Vector3.forward * speed);
        }
    }
}