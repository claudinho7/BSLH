using Characters.Monsters.Scripts.GOAP_Main;
using UnityEngine;

namespace Characters.Monsters.Scripts.Goals
{
    public class GoalPatrol : GoalBase
    {
        [SerializeField] private int patrolPriority = 10;

        public override int CalculatePriority()
        {
            return patrolPriority;
        }

        public override bool CanRun()
        {
            return !Movement.playerSeen; //can do if player not seen
        }
    }
}
