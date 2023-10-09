using Characters.Monsters.Scripts.UtilityCore;
using UnityEngine;


namespace Characters.Monsters.Scripts.UtilityConsiderations
{
    [CreateAssetMenu(fileName = "RepeatedAction", menuName = "UtilityConsiderations/RepeatedAction")]
    public class RepeatedAction : Consideration
    {
        [SerializeField] private AnimationCurve responseCurve;
        public override float ScoreConsideration(AIController aiController)
        {
            Score = responseCurve.Evaluate(Mathf.Clamp01(aiController.actionCounter / 100));
            return Score;
        }
    }
}
