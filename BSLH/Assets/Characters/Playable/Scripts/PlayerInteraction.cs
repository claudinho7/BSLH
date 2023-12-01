using UnityEngine;

namespace Characters.Playable.Scripts
{
    public class PlayerInteraction : MonoBehaviour
    {
        private PlayerUI _playerUI;
        private PlayerMovement _playerMovement;

        private void Awake()
        {
            _playerUI = GetComponent<PlayerUI>();
            _playerMovement = GetComponent<PlayerMovement>();
        }

        private void OnTriggerStay(Collider other)
        {
            // Check if the collision started with teleporter
            if (!other.gameObject.CompareTag("Teleporter")) return;
            _playerUI.ShowInteract();
            _playerMovement.canInteractWithMap = true;
        }

        private void OnTriggerExit(Collider other)
        {
            // Check if the collision ended with teleporter
            if (!other.gameObject.CompareTag("Teleporter")) return;
            _playerUI.HideInteract();
            _playerMovement.canInteractWithMap = false;
        }
    }
}
