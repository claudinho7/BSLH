using System.Collections;
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
        public GameObject reticle;
        public GameObject interact;
        public GameObject map;
        public GameObject pauseMenu;
        public GameObject inventory;
        public GameObject deathScreen;
        public GameObject bandageTextObj;
        public GameObject loadingScreen;
        public GameObject craftingUI;
        public GameObject timerObj;
        
        private bool _canShowUI;
        public int timer;

        public Button normalAttBtn;
        public Button heavyAttBtn;
        public Image heavyAttFiller;
        public Button skill1Btn;
        public Button skill2Btn;
        public Button bandageBtn;

        private void Start()
        {
            _playerDamage = GetComponent<PlayerDamage>();
            _playerMovement = GetComponent<PlayerMovement>();
            _canShowUI = true;
            
            map.SetActive(false);
            interact.SetActive(false);
            pauseMenu.SetActive(false);
            timer = 16;
        }

        private void Update()
        {
            maxHealthBar.fillAmount = _playerDamage.maxHealth / 300f;
            currentHealthBar.fillAmount = _playerDamage.currentHealth / 300f;
            currentStaminaBar.fillAmount = _playerMovement.stamina / 100f;

            bandageTextObj.GetComponent<TextMeshProUGUI>().text = _playerDamage.bandageCount.ToString("0");

            if (timer is < 16 and > 0)
            {
                timerObj.SetActive(true);
                timerObj.GetComponent<TextMeshProUGUI>().text = timer.ToString("0");
            }
            else
            {
                timerObj.SetActive(false);
            }
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
            Time.timeScale = 0;
            _playerMovement.canExecute = false;
        }
        
        public void CloseMap()
        {
            map.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            _canShowUI = true;
            Time.timeScale = 1;
            _playerMovement.canExecute = true;
        }

        public void OpenInventory()
        {
            if (!_canShowUI) return;
            Cursor.lockState = CursorLockMode.None;
            inventory.SetActive(true);
            _canShowUI = false;
            Time.timeScale = 0;
            _playerMovement.canExecute = false;
        }

        public void CloseInventory()
        {
            inventory.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            _canShowUI = true;
            _playerDamage.AttachArmor();
            _playerDamage.AttachWeapon();
            _playerDamage.AttachEssence();
            Time.timeScale = 1;
            _playerMovement.canExecute = true;
        }
        
        public void Pause()
        {
            if (!_canShowUI) return;
            pauseMenu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            _canShowUI = false;
            Time.timeScale = 0;
            _playerMovement.canExecute = false;
        }

        public void Resume()
        {
            pauseMenu.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;
            _canShowUI = true;
            _playerMovement.canExecute = true;
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
            Time.timeScale = 0;
            _playerMovement.canExecute = false;
        }
        
        public void CloseCrafting()
        {
            craftingUI.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            _canShowUI = true;
            Time.timeScale = 1;
            _playerMovement.canExecute = true;
        }
        #endregion

        public void ShowReticle()
        {
            reticle.SetActive(true);
        }
        public void HideReticle()
        {
            reticle.SetActive(false);
        }

        public void NormalAttackPressed()
        {
            if (normalAttBtn == null) return;
            normalAttBtn.image.color = _playerMovement.stamina >= 10f ? Color.green : Color.red;
            StartCoroutine(ResetButtonColor(normalAttBtn.image));
        }
        
        public void HeavyAttackPressed()
        {
            if (heavyAttBtn == null) return;
            heavyAttBtn.image.color = _playerMovement.stamina >= 20f ? Color.white : Color.red;
            StartCoroutine(ResetButtonColor(heavyAttBtn.image));
        }

        public void Skill1Pressed()
        {
            if (skill1Btn == null) return;
            skill1Btn.image.color = _playerMovement.stamina >= 30f ? Color.green : Color.red;
            StartCoroutine(ResetButtonColor(skill1Btn.image));
        }
        
        public void Skill2Pressed()
        {
            if (skill2Btn == null) return;
            skill2Btn.image.color = _playerMovement.stamina >= 30f ? Color.green : Color.red;
            StartCoroutine(ResetButtonColor(skill2Btn.image));
        }
        
        public void BandageBtnPressed()
        {
            if (bandageBtn == null) return;
            bandageBtn.image.color = _playerMovement.stamina >= 5f ? Color.green : Color.red;
            StartCoroutine(ResetButtonColor(bandageBtn.image));
        }

        public IEnumerator HeavyButtonFiller()
        {
            while (heavyAttFiller.fillAmount < 1f)
            {
                heavyAttFiller.fillAmount += .2f;
                
                yield return new WaitForSeconds(0.1f);
            }

            heavyAttFiller.fillAmount = 0f;
        }

        private static IEnumerator ResetButtonColor(Graphic image)
        {
            // Wait for the animation interval.
            yield return new WaitForSeconds(0.15f);
            image.color = Color.white;
        }
    }
}
