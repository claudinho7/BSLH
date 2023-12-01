using UnityEngine;
using UnityEngine.UI;

namespace Characters.Playable.Scripts
{
    public class PlayerUI : MonoBehaviour
    {
        private PlayerDamage _playerDamage;
        private PlayerMovement _playerMovement;
        public Image currentHealthBar;
        public Image maxHealthBar;
        public Image currentStaminaBar;
        public GameObject interact;
        public GameObject map;

        private void Start()
        {
            _playerDamage = GetComponent<PlayerDamage>();
            _playerMovement = GetComponent<PlayerMovement>();
            
            map.SetActive(false);
            interact.SetActive(false);
        }

        private void Update()
        {
            maxHealthBar.fillAmount = _playerDamage.maxHealth / 300f;
            currentHealthBar.fillAmount = _playerDamage.currentHealth / 300f;
            currentStaminaBar.fillAmount = _playerMovement.stamina / 100f;
        }

        public void ShowInteract()
        {
            interact.SetActive(true);
        }

        public void HideInteract()
        {
            interact.SetActive(false);
        }

        public void OpenMap()
        {
            map.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
        }
        
        public void CloseMap()
        {
            map.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
