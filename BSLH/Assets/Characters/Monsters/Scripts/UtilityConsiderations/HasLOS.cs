using Characters.Monsters.Scripts.UtilityCore;
using UnityEngine;


namespace Characters.Monsters.Scripts.UtilityConsiderations
{
    [CreateAssetMenu(fileName = "HasLOS", menuName = "UtilityConsiderations/HasLOS")]
    public class HasLos : Consideration
    {
        public override float ScoreConsideration(AIController aiController)
        {
            if (aiController.Movement.canHitRanged)
            {
                Score = 1f;
            }
            else
            {
                Score = 0f;
            }
            return Score;
        }
    }
}
