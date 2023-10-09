using Characters.Monsters.Scripts.UtilityCore;
using UnityEngine;


namespace Characters.Monsters.Scripts.UtilityActions
{
    [CreateAssetMenu(fileName = "SpecialMelee", menuName = "UtilityActions/SpecialMelee")]
    public class SpecialMelee : Action
    {
        public override void Execute(AIController aiController)
        {
            aiController.StartCoroutine(aiController.DoSpecialMeleeRoutine());

        }
    }
}
