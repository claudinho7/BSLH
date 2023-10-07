using System.Collections.Generic;
using Characters.Monsters.Scripts.Goals;
using Characters.Monsters.Scripts.GOAP_Main;
using UnityEngine;

namespace Characters.Monsters.Scripts.Actions
{
    public class ActionAttUltimate : ActionBase
    {
        private List<System.Type> _supportedGoals = new(new[] { typeof(GoalAttackFromRanged), typeof(GoalAttackFromMelee) });
        
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
            
            Damage.DoUltimateAttack();
            
            StartCoroutine(WaitForUltimate());
            Debug.Log("Ultimate");
        }
        
        public override void OnDeactivated()
        {
            base.OnDeactivated();
            Debug.Log("Ultimate Over");
        }

        public override void OnTick()
        {
            // do this every 10sec if health lower than 70
            if (UltimateUsed)
            {
                _cost = 5f;
            } 
            else if (!UltimateUsed && Damage.maxHealth <= 70f && Movement.canHitMelee)
            {
                _cost = 0f;
            }
            else
            {
                _cost = 5f;
            }
        }
    }
}
