using System.Collections.Generic;
using Characters.Monsters.Scripts.Goals;
using Characters.Monsters.Scripts.GOAP_Main;
using UnityEngine;

namespace Characters.Monsters.Scripts.Actions
{
    public class ActionMoveInRanged : ActionBase
    {
        private List<System.Type> _supportedGoals = new(new[] { typeof(GoalAttackFromRanged) });

        private float _cost;
        private bool _isActive;

        public override List<System.Type> GetSupportedGoals()
        {
            return _supportedGoals;
        }

        public override float Cost()
        {
            return _cost;
        }
        
        public override void OnActivated(GoalBase linkedGoal)
        {
            base.OnActivated(linkedGoal);
            _isActive = true;
            Movement.MoveInRanged();
            Debug.Log("Moving in range");
        }

        public override void OnDeactivated()
        {
            base.OnDeactivated();
            _isActive = false;
            Movement.ResetIsMoving();
            Debug.Log("Finished moving in ranged");
        }

        public override void OnTick()
        {
            _cost = Movement.distanceToPlayer switch
            {
                //increase  cost with distance
                > 11f => 1f,
                > 8f and <= 11f => 5f,
                >= 5f and <= 8f => 4f,
                _ => 1f
            };

            //if this action is active and player too far move again
            if (!_isActive || !(Movement.distanceToPlayer > 11f)) return;
            Movement.ResetIsMoving();
            Movement.MoveInRanged();
        }
    }
}
