using System.Collections;
using UnityEngine;

namespace Characters.Monsters.Scripts.UtilityCore
{
    public class AIController : MonoBehaviour
    {
        public AIMovement Movement { get; private set; }
        private AIBrain AIBrain { get; set; }
        public Action[] actionsAvailable;

        private Animator _animator;
        public MonsterDamage damage;
        
        //animations
        //public bool animationStarted;
        //public bool animationEnded;
        private float _animationLength;
        private bool _movementComplete;
        
        //action counters
        public float actionCounterNormalMelee;
        public float actionCounterNormalRanged;
        public float actionCounterSpecialMelee;
        public float actionCounterSpecialRanged;
        
        //cache animations
        private static readonly int NormalAttMelee = Animator.StringToHash("NormalAttMelee");
        private static readonly int NormalAttRanged = Animator.StringToHash("NormalAttRanged");
        private static readonly int SpecialAttMelee = Animator.StringToHash("SpecialAttMelee");
        private static readonly int SpecialAttRanged = Animator.StringToHash("SpecialAttRanged");
        private static readonly int UltimateAtt = Animator.StringToHash("UltimateAtt");

        private void Start()
        {
            Movement = GetComponent<AIMovement>();
            AIBrain = GetComponent<AIBrain>();
            _animator = GetComponent<Animator>();
            damage = GetComponent<MonsterDamage>();
        }

        private void Update()
        {
            if (Movement.isPatrolling && damage.currentHealth > 0f) return; //if player not seen and is patrolling or if AI is dead -> break
            if (AIBrain.FinishedDeciding)
            {
                AIBrain.FinishedDeciding = false;
                AIBrain.BestAction.Execute(this);
                ActionCounter(AIBrain.BestAction.actionName);
            }
        }

        private void OnFinishedAction()
        {
            AIBrain.DecideBestAction(actionsAvailable);
        }

        //Animations Start/Stop
        #region HandleAnimations
        private void AnimationStarted()
        {
            //animationStarted = true;
            //animationEnded = false;
        }

        private void AnimationEnded()
        { 
            //animationStarted = false;
            //animationEnded = true;
            OnFinishedAction();
        }
        #endregion
        
        //Execute animations and Damage Modifiers
        #region Skills
        public IEnumerator DoNormalMeleeRoutine()
        {
            while (Movement.distanceToPlayer > 2f)
            {
                Debug.Log("Moving in melee");
                Movement.MoveInMelee();
                yield return new WaitForSeconds(0.2f); // Yielding .2 seconds to save performance
            }
            Debug.Log("Started Normal Melee");
            _animator.SetTrigger(NormalAttMelee);
            damage.DoNormalMeleeAttack();
        }

        public IEnumerator DoNormalRangedRoutine()
        {
            while (Movement.distanceToPlayer is < 5f or >12f)
            {
                Debug.Log("Moving in ranged");
                Movement.MoveInRanged();
                yield return new WaitForSeconds(0.2f); // Yielding .2 seconds to save performance
            }
            Debug.Log("Started Normal Ranged");
            _animator.SetTrigger(NormalAttRanged);
            damage.DoNormalRangedAttack();
        }
        
        public IEnumerator DoSpecialMeleeRoutine()
        {
            while (Movement.distanceToPlayer > 2f)
            {
                Debug.Log("Moving in melee");
                Movement.MoveInMelee();
                yield return new WaitForSeconds(0.2f); // Yielding .2 seconds to save performance
            }
            Debug.Log("Started Special Melee");
            _animator.SetTrigger(SpecialAttMelee);
            damage.DoSpecialMeleeAttack();
        }

        public IEnumerator DoSpecialRangedRoutine()
        {
            while (Movement.distanceToPlayer is < 5f or >12f)
            {
                Debug.Log("Moving in ranged");
                Movement.MoveInRanged();
                yield return new WaitForSeconds(0.2f); // Yielding .2 seconds to save performance
            }
            Debug.Log("Started Special Ranged");
            _animator.SetTrigger(SpecialAttRanged);
            damage.DoSpecialRangedAttack();
        }
        
        public IEnumerator DoUltimateRoutine()
        {
            while (Movement.distanceToPlayer > 12f)
            {
                Debug.Log("Moving in ranged");
                Movement.MoveInRanged();
                yield return new WaitForSeconds(0.2f); // Yielding .2 seconds to save performance
            }
            Debug.Log("Started Ultimate");
            _animator.SetTrigger(UltimateAtt);
            damage.DoUltimateAttack();
        }
        #endregion
        
        private void ActionCounter(string actionName)
        {
            switch (actionName)
            {
                // Check the action name and update the corresponding counter
                case "NormalMelee":
                    actionCounterNormalMelee += 1f;
                    actionCounterNormalRanged = 0f;
                    actionCounterSpecialMelee = 0f;
                    actionCounterSpecialRanged = 0f;
                    Debug.Log("normal melee counter updated");
                    break;
                case "NormalRanged":
                    actionCounterNormalRanged += 1f;
                    actionCounterNormalMelee = 0f;
                    actionCounterSpecialMelee = 0f;
                    actionCounterSpecialRanged = 0f;
                    break;
                case "SpecialMelee":
                    actionCounterSpecialMelee += 1f;
                    actionCounterNormalMelee = 0f;
                    actionCounterNormalRanged = 0f;
                    actionCounterSpecialRanged = 0f;
                    Debug.Log("special melee counter updated");
                    break;
                case "SpecialRanged":
                    actionCounterSpecialRanged += 1f;
                    actionCounterNormalMelee = 0f;
                    actionCounterNormalRanged = 0f;
                    actionCounterSpecialMelee = 0f;
                    break;
            }
        }
    }
}
