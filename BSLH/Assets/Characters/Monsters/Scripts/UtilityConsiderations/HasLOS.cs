using Characters.Monsters.Scripts.UtilityCore;
using UnityEngine;


namespace Characters.Monsters.Scripts.UtilityConsiderations
{
    [CreateAssetMenu(fileName = "HasLOS", menuName = "UtilityConsiderations/HasLOS")]
    public class HasLos : Consideration
    {
        public override float ScoreConsideration(AIController aiController)
        {
            Score = aiController.Movement.canHitRanged ? 1f : 0f;
            return Score;
        }
    }
}
