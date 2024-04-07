using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OtherScripts
{
    public class CameraMovement : MonoBehaviour
    {
        private CinemachineVirtualCamera _camera;
        public Canvas canvas;

        private void Awake()
        {
            _camera = GetComponent<CinemachineVirtualCamera>();
        }

        private void Start()
        {
            var device = InputSystem.devices[0];
            InputSystem.DisableDevice(device);
            _camera.Priority = 100;
            canvas.enabled = false;
        }

        private void EndCamera()
        {
            var device = InputSystem.devices[0];
            _camera.Priority = 0;
            InputSystem.EnableDevice(device);
            canvas.enabled = true;
        }
    }
}
