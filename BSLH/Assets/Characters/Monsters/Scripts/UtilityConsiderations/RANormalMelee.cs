using Characters.Monsters.Scripts.UtilityCore;
using UnityEngine;


namespace Characters.Monsters.Scripts.UtilityConsiderations
{
    [CreateAssetMenu(fileName = "RA_NM", menuName = "UtilityConsiderations/RepeatedActionNormalMelee")]
    public class RaNormalMelee : Consideration
    {
        [SerializeField] private AnimationCurve responseCurve;
        public override float ScoreConsideration(AIController aiController)
        {
            Score = responseCurve.Evaluate(Mathf.Clamp01(aiController.actionCounterNormalMelee / 3));
            return Score;
        }
    }
}
