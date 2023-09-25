using Characters.Monsters.Scripts.GOAP_Main;
using UnityEngine;

namespace Characters.Monsters.Scripts.Goals
{
    public class GoalAttackFromMelee : GoalBase
    {
        [SerializeField] private int priority = 40;
        private int _currentPriority;


        public override void OnTickGoal()
        {
            // Cache the values
            float cachedHealth = Damage.health;
            var cachedDistanceToPlayer = Movement.distanceToPlayer;

            // Health Weight
            var healthWeight = 10f - (cachedHealth / 100f) * 10f;
            // Clamp the value to stay within the 0 to 10 range
            healthWeight = Mathf.Clamp(healthWeight, 0f, 10f);
            
            _currentPriority = (int)(priority - healthWeight - cachedDistanceToPlayer);
            //Debug.Log("melee priority:" + _currentPriority);
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
            return Movement.playerSeen; //can do if player seen
        }
    }
}
