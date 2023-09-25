using System.Collections.Generic;
using Characters.Monsters.Scripts.Goals;
using Characters.Monsters.Scripts.GOAP_Main;
using UnityEngine;

namespace Characters.Monsters.Scripts.Actions
{
    public class ActionAttSpecialMelee : ActionBase
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
            StartCoroutine(WaitForSpecialMelee());
            Debug.Log("Special Melee");
        }
        
        public override void OnDeactivated()
        {
            base.OnDeactivated();
            Debug.Log("Special Melee Over");
        }

        public override void OnTick()
        {
            // do this if in melee and 5sec has passed since last used
            _cost = SpecialMeleeUsed switch
            {
                true => 5f,
                false when Movement.isInMelee => 1f,
                _ => 5f
            };
        }
    }
}
