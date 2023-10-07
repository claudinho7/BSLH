using System.Collections.Generic;
using Characters.Monsters.Scripts.Goals;
using Characters.Monsters.Scripts.GOAP_Main;
using UnityEngine;

namespace Characters.Monsters.Scripts.Actions
{
    public class ActionAttSpecialRanged : ActionBase
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
            
            Damage.DoSpecialRangedAttack();
            
            StartCoroutine(WaitForSpecialRanged());
            Debug.Log("Special Ranged");
        }
        
        public override void OnDeactivated()
        {
            base.OnDeactivated();
            Debug.Log("Special Ranged Over");
        }

        public override void OnTick()
        {
            // do this if in perfect range and 5sec has passed since last used
            _cost = SpecialRangedUsed switch
            {
                true => 5f,
                false when Movement.isInRanged => 1f,
                _ => 5f
            };
        }
    }
}
