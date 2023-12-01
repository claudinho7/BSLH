using UnityEngine;
using UnityEngine.SceneManagement;

namespace OtherScripts
{
    public class MainMenu : MonoBehaviour
    {
        public void NewGame()
        {
            SceneManager.LoadScene(1);
        }
        
        public void ContinueGame()
        {
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
