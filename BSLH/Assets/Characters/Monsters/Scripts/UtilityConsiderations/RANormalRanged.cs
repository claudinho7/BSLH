using Characters.Monsters.Scripts.UtilityCore;
using UnityEngine;


namespace Characters.Monsters.Scripts.UtilityConsiderations
{
    [CreateAssetMenu(fileName = "RA_NR", menuName = "UtilityConsiderations/RepeatedActionNormalRanged")]
    public class RaNormalRanged : Consideration
    {
        [SerializeField] private AnimationCurve responseCurve;
        public override float ScoreConsideration(AIController aiController)
        {
            Score = responseCurve.Evaluate(Mathf.Clamp01(aiController.actionCounterNormalRanged / 3));
            return Score;
        }
    }
}
