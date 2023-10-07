using System.Collections.Generic;
using Characters.Monsters.Scripts.Goals;
using Characters.Monsters.Scripts.GOAP_Main;
using UnityEngine;

namespace Characters.Monsters.Scripts.Actions
{
    public class ActionAttNormalMelee : ActionBase
    {
        private List<System.Type> _supportedGoals = new(new[] { typeof(GoalAttackFromMelee) });
        
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
            
            Damage.DoNormalMeleeAttack();
            
            Debug.Log("Swipe Attack");
        }
        
        public override void OnDeactivated()
        {
            base.OnDeactivated();
            Debug.Log("Swipe Attack Over");
        }

        public override void OnTick()
        {
            _cost = Movement.isInMelee ? 2f : 5f;
        }
    }
}
