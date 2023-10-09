using Characters.Monsters.Scripts.UtilityCore;
using UnityEngine;


namespace Characters.Monsters.Scripts.UtilityActions
{
    [CreateAssetMenu(fileName = "Patrolling", menuName = "UtilityActions/Patrolling")]
    public class Patrolling : Action
    {
        public override void Execute(AIController aiController)
        {
            aiController.Patrol();
        }

        public override void SetRequiredDestination(AIController aiController)
        {
            //i need to change this depending on the Monster
        }
    }
}
