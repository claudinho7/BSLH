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

        [Header("Skill Bar")] 
        public GameObject skillBar;
        public Button normalAttBtn;
        public Button heavyAttBtn;
        public Image heavyAttFiller;
        public Button skill1Btn;
        public Button skill2Btn;
        public Button bandageBtn;
        public TextMeshProUGUI skill1Name;
        public TextMeshProUGUI skill2Name;
        public Sprite[] normalAttIcons;
        public Sprite[] heavyAttIcons;
        public Sprite[] heavyAttFillerIcons;
        public Sprite[] skill1Icons;
        public Sprite[] skill2Icons;
        public string[] skill1Names;
        public string[] skill2Names;

        private void Start()
        {
            _playerDamage = GetComponent<PlayerDamage>();
            _playerMovement = GetComponent<PlayerMovement>();
            _canShowUI = true;
            
            map.SetActive(false);
            interact.SetActive(false);
            pauseMenu.SetActive(false);
            timer = 30;
        }

        private void Update()
        {
            maxHealthBar.fillAmount = _playerDamage.maxHealth / 300f;
            currentHealthBar.fillAmount = _playerDamage.currentHealth / 300f;
            currentStaminaBar.fillAmount = _playerMovement.stamina / 100f;

            bandageTextObj.GetComponent<TextMeshProUGUI>().text = _playerDamage.bandageCount.ToString("0");

            if (timer is < 30 and > 0)
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
            skillBar.SetActive(false);
            Time.timeScale = 0;
            _playerMovement.canExecute = false;
        }
        
        public void CloseMap()
        {
            map.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            _canShowUI = true;
            skillBar.SetActive(true);
            Time.timeScale = 1;
            _playerMovement.canExecute = true;
        }

        public void OpenInventory()
        {
            if (!_canShowUI) return;
            Cursor.lockState = CursorLockMode.None;
            inventory.SetActive(true);
            skillBar.SetActive(false);
            _canShowUI = false;
            Time.timeScale = 0;
            _playerMovement.canExecute = false;
        }

        public void CloseInventory()
        {
            inventory.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            _canShowUI = true;
            skillBar.SetActive(true);
            _playerDamage.AttachArmor();
            _playerDamage.AttachWeapon();
            _playerDamage.AttachEssence();
            SwitchWeaponIcons();
            Time.timeScale = 1;
            _playerMovement.canExecute = true;
        }
        
        public void Pause()
        {
            if (!_canShowUI) return;
            pauseMenu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            skillBar.SetActive(false);
            _canShowUI = false;
            Time.timeScale = 0;
            _playerMovement.canExecute = false;
        }

        public void Resume()
        {
            pauseMenu.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            skillBar.SetActive(true);
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
            SceneManager.LoadScene(0);
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
            skillBar.SetActive(false);
            Cursor.lockState = CursorLockMode.None;
            _canShowUI = false;
            Time.timeScale = 0;
            _playerMovement.canExecute = false;
        }
        
        public void CloseCrafting()
        {
            craftingUI.SetActive(false);
            skillBar.SetActive(true);
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

        //skill bars
        #region SkillBar
        private void SwitchWeaponIcons()
        {
            switch (_playerDamage.activeWeapon.name)
            {
                case "Sword(Clone)":
                    normalAttBtn.image.sprite = normalAttIcons[1];
                    heavyAttBtn.image.sprite = heavyAttIcons[1];
                    heavyAttFiller.sprite = heavyAttFillerIcons[1];
                    skill1Btn.image.sprite = skill1Icons[1];
                    skill2Btn.image.sprite = skill2Icons[1];
                    skill1Name.text = skill1Names[1];
                    skill2Name.text = skill2Names[1];
                    break;
                case "Spear(Clone)":
                    normalAttBtn.image.sprite = normalAttIcons[2];
                    heavyAttBtn.image.sprite = heavyAttIcons[2];
                    heavyAttFiller.sprite = heavyAttFillerIcons[2];
                    skill1Btn.image.sprite = skill1Icons[2];
                    skill2Btn.image.sprite = skill2Icons[2];
                    skill1Name.text = skill1Names[2];
                    skill2Name.text = skill2Names[2];
                    break;
                case "Hammer(Clone)":
                    normalAttBtn.image.sprite = normalAttIcons[3];
                    heavyAttBtn.image.sprite = heavyAttIcons[3];
                    heavyAttFiller.sprite = heavyAttFillerIcons[3];
                    skill1Btn.image.sprite = skill1Icons[3];
                    skill2Btn.image.sprite = skill2Icons[3];
                    skill1Name.text = skill1Names[3];
                    skill2Name.text = skill2Names[3];
                    break;
                case "Crossbow(Clone)":
                    normalAttBtn.image.sprite = normalAttIcons[4];
                    heavyAttBtn.image.sprite = heavyAttIcons[4];
                    heavyAttFiller.sprite = heavyAttFillerIcons[4];
                    skill1Btn.image.sprite = skill1Icons[4];
                    skill2Btn.image.sprite = skill2Icons[4];
                    skill1Name.text = skill1Names[4];
                    skill2Name.text = skill2Names[4];
                    break;
                default:
                    normalAttBtn.image.sprite = normalAttIcons[0];
                    heavyAttBtn.image.sprite = heavyAttIcons[0];
                    heavyAttFiller.sprite = heavyAttFillerIcons[0];
                    skill1Btn.image.sprite = skill1Icons[0];
                    skill2Btn.image.sprite = skill2Icons[0];
                    skill1Name.text = skill1Names[0];
                    skill2Name.text = skill2Names[0];
                    break;
            }
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
        #endregion
    }
}
