using Characters.Monsters.Scripts.UtilityCore;
using UnityEngine;


namespace Characters.Monsters.Scripts.UtilityConsiderations
{
    [CreateAssetMenu(fileName = "RA_SM", menuName = "UtilityConsiderations/RepeatedActionSpecialMelee")]
    public class RaSpecialMelee : Consideration
    { 
        public override float ScoreConsideration(AIController aiController)
        {
            Score = aiController.actionCounterSpecialMelee switch
            {
                0 => 1f,
                1 => 0.7f,
                >= 2 => 0f,
                _ => Score
            };
            return Score;
        }
    }
}
