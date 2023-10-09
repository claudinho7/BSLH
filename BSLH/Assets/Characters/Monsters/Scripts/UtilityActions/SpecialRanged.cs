using Characters.Monsters.Scripts.UtilityCore;
using UnityEngine;


namespace Characters.Monsters.Scripts.UtilityActions
{
    [CreateAssetMenu(fileName = "SpecialRanged", menuName = "UtilityActions/SpecialRanged")]
    public class SpecialRanged : Action
    {
        public override void Execute(AIController aiController)
        {
            aiController.StartCoroutine(aiController.DoSpecialRangedRoutine());
        }
    }
}
