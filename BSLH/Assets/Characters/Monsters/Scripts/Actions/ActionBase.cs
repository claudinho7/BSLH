using System.Collections.Generic;
using Characters.Monsters.Scripts.Goals;
using UnityEngine;

namespace Characters.Monsters.Scripts.Actions
{ 
    public class ActionBase : MonoBehaviour
    {
        protected MonsterMovement Movement;
        protected GoalBase LinkedGoals;
        
        private void Awake()
        {
            Movement = GetComponent<MonsterMovement>();
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
    }
}
