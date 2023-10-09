using Characters.Monsters.Scripts.UtilityCore;
using UnityEngine;


namespace Characters.Monsters.Scripts.UtilityConsiderations
{
    [CreateAssetMenu(fileName = "RA_SM", menuName = "UtilityConsiderations/RepeatedActionSpecialMelee")]
    public class RaSpecialMelee : Consideration
    {
        [SerializeField] private AnimationCurve responseCurve;
        public override float ScoreConsideration(AIController aiController)
        {
            Score = responseCurve.Evaluate(Mathf.Clamp01(aiController.actionCounterSpecialMelee / 2));
            return Score;
        }
    }
}
