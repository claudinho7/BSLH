using Characters.Monsters.Scripts.UtilityCore;
using UnityEngine;

namespace Characters.Monsters.Scripts.UtilityActions
{
    [CreateAssetMenu(fileName = "NormalMelee", menuName = "UtilityActions/NormalMelee")]
    public class NormalMelee : Action
    {
        public override void Execute(AIController aiController)
        {
            aiController.StartCoroutine(aiController.DoNormalMeleeRoutine());
        }
    }
}
