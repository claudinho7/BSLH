using Characters.Monsters.Scripts.UtilityCore;
using UnityEngine;


namespace Characters.Monsters.Scripts.UtilityConsiderations
{
    [CreateAssetMenu(fileName = "IsPatrolling", menuName = "UtilityConsiderations/IsPatrolling")]
    public class IsPatrolling : Consideration
    {
        public override float ScoreConsideration(AIController aiController)
        {
            Score = aiController.Movement.isPatrolling ? 1 : 0;
            return Score;
        }
    }
}
