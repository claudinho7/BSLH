using UnityEngine;
using UnityEngine.UI;

namespace Characters.Monsters.Scripts
{
    public class MonsterUI : MonoBehaviour
    {
        private MonsterDamage _monsterDamage;
        public Image maxHealthBar;
        public Image currentHealthBar;
        public GameObject targetLock;

        private Transform _cameraMain;

        private void Start()
        {
            _monsterDamage = GetComponent<MonsterDamage>();
            if (Camera.main != null) _cameraMain = Camera.main.transform;
            
            targetLock.SetActive(false);
        }

        private void Update()
        {
            maxHealthBar.transform.LookAt(_cameraMain);
            currentHealthBar.transform.LookAt(_cameraMain);

            maxHealthBar.fillAmount = _monsterDamage.maxHealth / 300f;
            currentHealthBar.fillAmount = _monsterDamage.currentHealth / 300f;
        }
        
        public void ShowTargetLock()
        {
            targetLock.SetActive(true);
        }

        public void HideTargetLock()
        {
            targetLock.SetActive(false);
        }
    }
}
