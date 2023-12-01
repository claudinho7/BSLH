using UnityEngine;

namespace OtherScripts
{
    public class PauseMenu : MonoBehaviour

    {

        [SerializeField] private GameObject pauseMenu;
        
        public void Pause()
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
        }

        public void Continue()
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1;
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                pauseMenu.gameObject.SetActive(!pauseMenu.gameObject.activeSelf);
            }
        }
    }
}
