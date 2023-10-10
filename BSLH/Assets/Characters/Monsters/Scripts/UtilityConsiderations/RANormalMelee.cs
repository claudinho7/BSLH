using Characters.Monsters.Scripts.UtilityCore;
using UnityEngine;


namespace Characters.Monsters.Scripts.UtilityConsiderations
{
    [CreateAssetMenu(fileName = "RA_NM", menuName = "UtilityConsiderations/RepeatedActionNormalMelee")]
    public class RaNormalMelee : Consideration
    {
        public override float ScoreConsideration(AIController aiController)
        {
            Score = aiController.actionCounterNormalMelee switch
            {
                0 => 1f,
                1 => 1f,
                2 => 0.7f,
                >= 3 => 0f,
                _ => Score
            };
            return Score;
        }
    }
}
