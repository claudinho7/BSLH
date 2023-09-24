using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Characters.Monsters.Scripts.GOAPUI
{
    public class GoalUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI Name;
        [SerializeField] private Slider priority;
        [SerializeField] private TextMeshProUGUI status;

        public void UpdateGoalInfo(string _name, string _status, float _priority)
        {
            Name.text = _name;
            status.text = _status;
            priority.value = _priority;
        }
    }
}
