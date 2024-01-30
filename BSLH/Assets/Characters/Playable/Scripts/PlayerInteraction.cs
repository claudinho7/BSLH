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
            if (other.gameObject.CompareTag("Teleporter"))
            {
                _playerUI.ShowInteract();
                _playerMovement.canInteractWithMap = true;
            }
            else if (other.gameObject.CompareTag("CraftingStation"))
            {
                _playerUI.ShowInteract();
                _playerMovement.canInteractWithCraftingBench = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            // Check if the collision ended with teleporter
            if (other.gameObject.CompareTag("Teleporter"))
            {
                _playerUI.HideInteract();
                _playerMovement.canInteractWithMap = false;
            }
            else if (other.gameObject.CompareTag("CraftingStation"))
            {
                _playerUI.HideInteract();
                _playerMovement.canInteractWithCraftingBench = false;
            }
        }
    }
}
