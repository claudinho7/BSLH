using System.Collections.Generic;
using Characters.Monsters.Scripts.Goals;

namespace Characters.Monsters.Scripts.Actions
{
    public class ActionIdle : ActionBase
    {
        private List<System.Type> _supportedGoals = new(new[] {typeof(GoalIdle)});

        public override List<System.Type> GetSupportedGoals()
        {
            return _supportedGoals;
        }

        public override float Cost()
        {
            return 0f;
        }
    }
}
