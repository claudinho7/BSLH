using System.Collections.Generic;
using Characters.Monsters.Scripts.Goals;

namespace Characters.Monsters.Scripts.Actions
{
    public class ActionPatrol : ActionBase
    {
        private List<System.Type> _supportedGoals = new(new[] { typeof(GoalPatrol) });

        public override List<System.Type> GetSupportedGoals()
        {
            return _supportedGoals;
        }

        public override float Cost()
        {
            return 0f;
        }
        
        public override void OnActivated(GoalBase linkedGoal)
        {
            base.OnActivated(linkedGoal);
            Movement.StartPatrolling();
        }

        public override void OnDeactivated()
        {
            Movement.StopPatrolling();
        }

        public override void OnTick()
        {
            OnActivated(LinkedGoals);
        }
    }
}
