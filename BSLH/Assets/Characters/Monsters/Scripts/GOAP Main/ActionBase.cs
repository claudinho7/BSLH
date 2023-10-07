using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Characters.Monsters.Scripts.GOAP_Main
{ 
    public class ActionBase : MonoBehaviour
    {
        protected MonsterMovement Movement;
        protected MonsterDamage Damage;
        protected GoalBase LinkedGoals;
        protected bool SpecialMeleeUsed;
        protected bool SpecialRangedUsed;
        protected bool UltimateUsed;
        
        private void Awake()
        {
            Movement = GetComponent<MonsterMovement>();
            Damage = GetComponent<MonsterDamage>();
        }

        private void Update()
        {
            OnTick();
        }

        public virtual List<System.Type> GetSupportedGoals()
        {
            return null;
        }

        public virtual float Cost()
        {
            return 0f;
        }

        public virtual void OnActivated(GoalBase linkedGoal)
        {
            LinkedGoals = linkedGoal;
        }

        public virtual void OnDeactivated()
        {
            LinkedGoals = null;
        }

        public virtual void OnTick()
        {
        }
        
        protected IEnumerator WaitForSpecialMelee()
        {
            SpecialMeleeUsed = true;
            yield return new WaitForSeconds(5.0f);
            SpecialMeleeUsed = false;
        }
        
        protected IEnumerator WaitForSpecialRanged()
        {
            SpecialRangedUsed = true;
            yield return new WaitForSeconds(8.0f);
            SpecialRangedUsed = false;
        }
        
        protected IEnumerator WaitForUltimate()
        {
            UltimateUsed = true;
            yield return new WaitForSeconds(15.0f);
            UltimateUsed = false;
        }
    }
}
