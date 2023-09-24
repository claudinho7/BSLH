using UnityEngine;

namespace Characters.Monsters.Scripts.Goals
{
    public class GoalIdle : GoalBase
    {
        [SerializeField] private int priority = 1;
        public override int CalculatePriority()
        {
            return priority;
        }

        public override bool CanRun()
        {
            return !Movement.playerSeen; //can do if player not seen
        }
    }
}
