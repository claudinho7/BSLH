using Characters.Monsters.Scripts.UtilityCore;
using UnityEngine;


namespace Characters.Monsters.Scripts.UtilityConsiderations
{
    [CreateAssetMenu(fileName = "CanDoUltimate", menuName = "UtilityConsiderations/CanDoUltimate")]
    public class CanDoUltimate : Consideration
    {
        public override float ScoreConsideration(AIController aiController)
        {
            if (aiController.canDoUltimate && aiController.damage.currentHealth <= 70f)
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
