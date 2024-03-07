using UnityEngine;

namespace OtherScripts
{
    public class CameraFollow : MonoBehaviour
    {
        public GameObject player;
        public Vector3 offset;

        private void Update()
        {
            transform.position = player.transform.position + offset;
        }
    }
}
