using Characters.Monsters.Scripts.UtilityCore;
using UnityEngine;


namespace Characters.Monsters.Scripts.UtilityConsiderations
{
    [CreateAssetMenu(fileName = "RA_SR", menuName = "UtilityConsiderations/RepeatedActionSpecialRanged")]
    public class RaSpecialRanged : Consideration
    {
        public override float ScoreConsideration(AIController aiController)
        {
            Score = aiController.actionCounterSpecialRanged switch
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
