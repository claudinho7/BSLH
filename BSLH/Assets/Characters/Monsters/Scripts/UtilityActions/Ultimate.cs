using Characters.Monsters.Scripts.UtilityCore;
using UnityEngine;


namespace Characters.Monsters.Scripts.UtilityActions
{
    [CreateAssetMenu(fileName = "Ultimate", menuName = "UtilityActions/Ultimate")]
    public class Ultimate : Action
    {
        public override void Execute(AIController aiController)
        {
            aiController.StartCoroutine(aiController.DoUltimateRoutine());
        }
    }
}
