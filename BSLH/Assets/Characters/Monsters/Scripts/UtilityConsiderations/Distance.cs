using Characters.Monsters.Scripts.UtilityCore;
using UnityEngine;


namespace Characters.Monsters.Scripts.UtilityConsiderations
{
    [CreateAssetMenu(fileName = "Distance", menuName = "UtilityConsiderations/Distance")]
    public class Distance : Consideration
    {
        [SerializeField] private AnimationCurve responseCurve;
        public override float ScoreConsideration(AIController aiController)
        {
            const float maxDistance = 15f; // Set the maximum distance you want to consider
            var clampedDistance = Mathf.Clamp(aiController.Movement.distanceToPlayer, 0f, maxDistance);
            Score = responseCurve.Evaluate(clampedDistance / maxDistance);
            return Score;
        }
    }
}
