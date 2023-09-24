using System.Collections.Generic;
using UnityEngine;

namespace Characters.Monsters.Scripts.GOAPUI
{
    // ReSharper disable once InconsistentNaming
    public class GOAPUI : MonoBehaviour
    {
        [SerializeField] private GameObject goalPrefab;
        [SerializeField] private RectTransform goalRoot;

        Dictionary<MonoBehaviour, GoalUI> _displayedGoals = new Dictionary<MonoBehaviour, GoalUI>();

        public void UpdateGoal(MonoBehaviour goal, string _name, string _status, float _priority)
        {
            // add if not present
            if (!_displayedGoals.ContainsKey(goal))
                _displayedGoals[goal] = Instantiate(goalPrefab, Vector3.zero, Quaternion.identity, goalRoot).GetComponent<GoalUI>();

            _displayedGoals[goal].UpdateGoalInfo(_name, _status, _priority);
        }
    }
}
