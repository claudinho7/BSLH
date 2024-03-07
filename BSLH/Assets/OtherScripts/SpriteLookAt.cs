using UnityEngine;

namespace OtherScripts
{
    public class SpriteLookAt : MonoBehaviour
    {
        private void LateUpdate()
        {
            if (Camera.main == null) return;
            var transform1 = Camera.main.transform;
            var rotation = transform1.rotation;
            transform.LookAt(transform.position + rotation * Vector3.forward, rotation * Vector3.up);
        }
    }
}
