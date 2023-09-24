using Characters.Monsters.Scripts.Actions;
using UnityEngine;

namespace Characters.Monsters.Scripts.Goals
{
    public interface IGoal
    {
        public int CalculatePriority();
        public bool CanRun();
        public void OnTickGoal();
        public void OnGoalActivated(ActionBase linkedAction);
        public void OnGoalDeactivated();
    }
    
    public class GoalBase : MonoBehaviour, IGoal
    {
        protected MonsterMovement Movement;
        protected GOAPUI.GOAPUI DebugUI;
        protected ActionBase LinkedAction;
        
        private void Awake()
        {
            Movement = GetComponent<MonsterMovement>();
        }

        private void Start()
        {
            DebugUI = FindObjectOfType<GOAPUI.GOAPUI>();
        }

        private void Update()
        {
            OnTickGoal();
            
            DebugUI.UpdateGoal(this, GetType().Name, LinkedAction ? "Running" : "Paused", CalculatePriority());
        }

        public virtual int CalculatePriority()
        {
            return -1;
        }

        public virtual bool CanRun()
        {
            return false;
        }

        public virtual void OnTickGoal()
        {
        }

        public virtual void OnGoalActivated(ActionBase linkedAction)
        {
            LinkedAction = linkedAction;
        }

        public virtual void OnGoalDeactivated()
        {
            LinkedAction = null;
        }
    }
}
