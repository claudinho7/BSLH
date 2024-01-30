using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        public GameObject pauseMenu;
        public GameObject inventory;
        public GameObject deathScreen;
        public GameObject bandageTextObj;
        public GameObject loadingScreen;
        public GameObject craftingUI;

        private bool _canShowUI;

        private void Start()
        {
            _playerDamage = GetComponent<PlayerDamage>();
            _playerMovement = GetComponent<PlayerMovement>();
            _canShowUI = true;
            
            map.SetActive(false);
            interact.SetActive(false);
            pauseMenu.SetActive(false);
        }

        private void Update()
        {
            maxHealthBar.fillAmount = _playerDamage.maxHealth / 300f;
            currentHealthBar.fillAmount = _playerDamage.currentHealth / 300f;
            currentStaminaBar.fillAmount = _playerMovement.stamina / 100f;

            bandageTextObj.GetComponent<TextMeshProUGUI>().text = _playerDamage.bandageCount.ToString("0");
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
            if (!_canShowUI) return;
            map.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            _canShowUI = false;
        }
        
        public void CloseMap()
        {
            map.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            _canShowUI = true;
        }

        public void OpenInventory()
        {
            if (!_canShowUI) return;
            Cursor.lockState = CursorLockMode.None;
            inventory.SetActive(true);
            _canShowUI = false;

        }

        public void CloseInventory()
        {
            inventory.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            _canShowUI = true;
            _playerDamage.AttachArmor();
            _playerDamage.AttachWeapon();
        }
        
        public void Pause()
        {
            if (!_canShowUI) return;
            pauseMenu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            _canShowUI = false;
            Time.timeScale = 0;
        }

        public void Resume()
        {
            pauseMenu.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;
            _canShowUI = true;
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        public void RestartGame()
        {
            SceneManager.LoadScene(1);
            Time.timeScale = 1;
            deathScreen.SetActive(false);
            _canShowUI = true;
        }

        public void DeathScreen()
        {
            deathScreen.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            _canShowUI = false;
        }

        public void LoadingScreenOn()
        {
            loadingScreen.SetActive(true);
        }

        public void LoadingScreenOff()
        {
            loadingScreen.SetActive(false);
        }

        //crafting
        #region Crafting
        public void OpenCrafting()
        {
            if (!_canShowUI) return;
            craftingUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            _canShowUI = false;
        }
        
        public void CloseCrafting()
        {
            craftingUI.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            _canShowUI = true;
        }
        #endregion
    }
}
