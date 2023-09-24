using UnityEngine;

namespace Characters.Monsters.Scripts.Goals
{
    public class GoalMoveInMelee : GoalBase
    {
        [SerializeField] private int priority = 30;
        private int _currentPriority;
        
        
        public override void OnTickGoal()
        {
            if (Movement.playerSeen && !Movement.canHitRanged)
            {
                _currentPriority = priority;
            }
            else
            {
                _currentPriority = 0;
            }
        }

        public override void OnGoalDeactivated()
        {
            base.OnGoalDeactivated();
            _currentPriority = 0;
        }
        
        public override int CalculatePriority()
        {
            return _currentPriority;
        }

        public override bool CanRun()
        {
            return Movement.playerSeen && !Movement.canHitRanged; //can do if player seen
        }
    }
}
