using System.Collections.Generic;
using Characters.Monsters.Scripts.Goals;

namespace Characters.Monsters.Scripts.Actions
{
    public class ActionMoveInMelee : ActionBase
    {
        private List<System.Type> _supportedGoals = new(new[] { typeof(GoalMoveInMelee) });

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
            Movement.MoveInMelee();
        }

        public override void OnDeactivated()
        {
        }

        public override void OnTick()
        { 
            OnActivated(LinkedGoals);
        }
    }
}
