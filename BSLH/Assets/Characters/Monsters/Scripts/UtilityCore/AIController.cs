using System.Collections;
using Characters.Playable.Scripts;
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
        private PlayerDamage _playerDamage;

        //animations
        //public bool animationStarted;
        //public bool animationEnded;
        
        //action counters
        public int actionCounterNormalMelee;
        public int actionCounterNormalRanged;
        public int actionCounterSpecialMelee;
        public int actionCounterSpecialRanged;
        public bool canDoUltimate;
        
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
            StartCoroutine(FindPlayer());
            
            canDoUltimate = true;
        }

        private void Update()
        {
            if (Movement.isPatrolling && damage.currentHealth > 0f || _playerDamage.currentHealth <= 0f) return; //if player not seen and is patrolling or if AI is dead -> break
            if (!AIBrain.FinishedDeciding) return;
            AIBrain.FinishedDeciding = false;
            AIBrain.BestAction.Execute(this);
            ActionCounter(AIBrain.BestAction.actionName);
        }

        private void OnFinishedAction()
        {
            AIBrain.DecideBestAction(actionsAvailable);
        }

        //Animations Start/Stop
        #region HandleAnimations
        public void AnimationStarted()
        {
            //animationStarted = true;
            //animationEnded = false;
        }

        public void AnimationEnded()
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
                Movement.MoveInMelee();
                yield return new WaitForSeconds(0.2f); // Yielding .2 seconds to save performance
            }
            Debug.Log("Started Normal Melee");
            Movement.LookAtPlayer();
            _animator.SetTrigger(NormalAttMelee);
            damage.DoNormalMeleeAttack();
        }

        public IEnumerator DoNormalRangedRoutine()
        {
            while (Movement.distanceToPlayer is < 5f or >12f)
            {
                Movement.MoveInRanged();
                yield return new WaitForSeconds(0.2f); // Yielding .2 seconds to save performance
            }
            Debug.Log("Started Normal Ranged");
            Movement.LookAtPlayer();
            _animator.SetTrigger(NormalAttRanged);
            damage.DoNormalRangedAttack();
        }
        
        public IEnumerator DoSpecialMeleeRoutine()
        {
            while (Movement.distanceToPlayer > 2f)
            {
                Movement.MoveInMelee();
                yield return new WaitForSeconds(0.2f); // Yielding .2 seconds to save performance
            }
            Debug.Log("Started Special Melee");
            Movement.LookAtPlayer();
            _animator.SetTrigger(SpecialAttMelee);
            damage.DoSpecialMeleeAttack();
        }

        public IEnumerator DoSpecialRangedRoutine()
        {
            while (Movement.distanceToPlayer is < 5f or >12f)
            {
                Movement.MoveInRanged();
                yield return new WaitForSeconds(0.2f); // Yielding .2 seconds to save performance
            }
            Debug.Log("Started Special Ranged");
            Movement.LookAtPlayer();
            _animator.SetTrigger(SpecialAttRanged);
            damage.DoSpecialRangedAttack();
        }
        
        public IEnumerator DoUltimateRoutine()
        {
            while (Movement.distanceToPlayer > 12f)
            {
                Movement.MoveInRanged();
                yield return new WaitForSeconds(0.2f); // Yielding .2 seconds to save performance
            }
            Debug.Log("Started Ultimate");
            Movement.LookAtPlayer();
            _animator.SetTrigger(UltimateAtt);
            damage.DoUltimateAttack();
            StartCoroutine(UltimateCooldown());
        }
        #endregion
        
        private void ActionCounter(string actionName)
        {
            switch (actionName)
            {
                // Check the action name and update the corresponding counter
                case "NormalMelee":
                    actionCounterNormalMelee += 1;
                    actionCounterNormalRanged = 0;
                    actionCounterSpecialMelee = 0;
                    actionCounterSpecialRanged = 0;
                    break;
                case "NormalRanged":
                    actionCounterNormalRanged += 1;
                    actionCounterNormalMelee = 0;
                    actionCounterSpecialMelee = 0;
                    actionCounterSpecialRanged = 0;
                    break;
                case "SpecialMelee":
                    actionCounterSpecialMelee += 1;
                    actionCounterNormalMelee = 0;
                    actionCounterNormalRanged = 0;
                    actionCounterSpecialRanged = 0;
                    break;
                case "SpecialRanged":
                    actionCounterSpecialRanged += 1;
                    actionCounterNormalMelee = 0;
                    actionCounterNormalRanged = 0;
                    actionCounterSpecialMelee = 0;
                    break;
                case "Ultimate" :
                    actionCounterSpecialRanged = 0;
                    actionCounterNormalMelee = 0;
                    actionCounterNormalRanged = 0;
                    actionCounterSpecialMelee = 0;
                    break;
            }
        }

        private IEnumerator UltimateCooldown()
        {
            canDoUltimate = false;

            yield return new WaitForSeconds(15.0f); // Wait for 15 seconds

            canDoUltimate = true;
        }

        private IEnumerator FindPlayer()
        {
            while (_playerDamage == null)
            {
                _playerDamage = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerDamage>();
                Movement.playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

                yield return null;
            }
        }
    }
}
