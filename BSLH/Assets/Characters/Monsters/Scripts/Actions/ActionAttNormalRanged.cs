using System.Collections.Generic;
using Characters.Monsters.Scripts.Goals;
using Characters.Monsters.Scripts.GOAP_Main;
using UnityEngine;

namespace Characters.Monsters.Scripts.Actions
{
    public class ActionAttNormalRanged : ActionBase
    {
        private List<System.Type> _supportedGoals = new(new[] { typeof(GoalAttackFromRanged) });
        
        private float _cost;

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
            
            Damage.DoNormalRangedAttack();
            
            Debug.Log("Shooting my bow");
        }

        public override void OnDeactivated()
        {
            base.OnDeactivated();
            Debug.Log("Stopped Shooting");
        }

        public override void OnTick()
        { 
            _cost = Movement.distanceToPlayer switch
            {
                //increase  cost with distance
                > 11f => 5f,
                > 8f and <= 11f => 2f,
                >= 5f and <= 8f => 3f,
                _ => 5f
            };
        }
    }
}
