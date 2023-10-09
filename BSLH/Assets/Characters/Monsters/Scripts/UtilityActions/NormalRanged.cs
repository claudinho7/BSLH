using Characters.Monsters.Scripts.UtilityCore;
using UnityEngine;


namespace Characters.Monsters.Scripts.UtilityActions
{
    [CreateAssetMenu(fileName = "NormalRanged", menuName = "UtilityActions/NormalRanged")]
    public class NormalRanged : Action
    {
        public override void Execute(AIController aiController)
        {
            aiController.StartCoroutine(aiController.DoNormalRangedRoutine());
        }
    }
}
