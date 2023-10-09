using Characters.Monsters.Scripts.UtilityCore;
using UnityEngine;


namespace Characters.Monsters.Scripts.UtilityConsiderations
{
    [CreateAssetMenu(fileName = "OwnHealth", menuName = "UtilityConsiderations/OwnHealth")]
    public class OwnHealth : Consideration
    {
        [SerializeField] private AnimationCurve responseCurve;
        public override float ScoreConsideration(AIController aiController)
        {
            Score = responseCurve.Evaluate(Mathf.Clamp01(aiController.damage.currentHealth / 100));
            return Score;
        }
    }
}
