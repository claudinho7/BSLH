using Characters.Monsters.Scripts.Actions;
using UnityEngine;

namespace Characters.Monsters.Scripts.Goals
{
    public class GoalPatrol : GoalBase
    {
        [SerializeField] private int patrolPriority = 10;
        private float _currentPriority;
        [SerializeField] private float priorityBuildRate = 1f;
        [SerializeField] private float priorityDecayRate = 0.1f;

        public override void OnTickGoal()
        {
            if (Movement.IsMoving)
                _currentPriority -= priorityDecayRate * Time.deltaTime;
            else
                _currentPriority += priorityBuildRate * Time.deltaTime;
        }
        
        public override void OnGoalActivated(ActionBase linkedAction)
        {
            base.OnGoalActivated(linkedAction);
            _currentPriority = patrolPriority;
        }
        
        public override int CalculatePriority()
        {
            return Mathf.FloorToInt(_currentPriority);
        }

        public override bool CanRun()
        {
            return !Movement.playerSeen; //can do if player not seen
        }
    }
}
