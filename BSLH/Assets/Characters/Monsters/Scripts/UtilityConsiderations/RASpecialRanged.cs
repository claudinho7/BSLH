using Characters.Monsters.Scripts.UtilityCore;
using UnityEngine;


namespace Characters.Monsters.Scripts.UtilityConsiderations
{
    [CreateAssetMenu(fileName = "RA_SR", menuName = "UtilityConsiderations/RepeatedActionSpecialRanged")]
    public class RaSpecialRanged : Consideration
    {
        [SerializeField] private AnimationCurve responseCurve;
        public override float ScoreConsideration(AIController aiController)
        {
            Score = responseCurve.Evaluate(Mathf.Clamp01(aiController.actionCounterSpecialRanged / 2));
            return Score;
        }
    }
}
