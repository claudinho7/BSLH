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

        [Header("References")]
        private Animator _animator;
        public MonsterDamage damage;
        public MonsterUI monsterUI;
        public PlayerDamage playerDamage;

        private bool _playerFound;

        [Header("Counters")]
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

        private void Awake()
        {
            Movement = GetComponent<AIMovement>();
            AIBrain = GetComponent<AIBrain>();
            _animator = GetComponent<Animator>();
            
            // Check if _playerDamage is still null and the player is not yet found.
            if (playerDamage == null && !_playerFound)
            {
                StartCoroutine(FindPlayer());
            }
            else
            {
                // PlayerDamage is already set, or player search is in progress.
                Debug.Log(playerDamage == null
                    ? "PlayerDamage not set; still searching for the player..."
                    : "PlayerDamage already set.");
            }
        }

        private void Start()
        {
            canDoUltimate = true;
        }

        private void Update()
        {
            //if player not seen and is patrolling or if AI is dead -> break
            if (damage.currentHealth <= 0) return;
            if (playerDamage == null || playerDamage.currentHealth <= 0f || Movement.isPatrolling) return;
            if (!AIBrain.FinishedDeciding) return;
            AIBrain.FinishedDeciding = false;
            AIBrain.BestAction.Execute(this);
            ActionCounter(AIBrain.BestAction.actionName);
        }

        private void LateUpdate()
        {
            if (playerDamage == null) return;
            if (playerDamage.targetLocked)
            {
                monsterUI.ShowTargetLock();
            }
            else
            {
                monsterUI.HideTargetLock();
            }
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
            while (Movement.distanceToPlayer is < 5f or >11f)
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
            while (Movement.distanceToPlayer is < 5f or >11f)
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
            while (Movement.distanceToPlayer > 11f)
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
            while (!_playerFound)
            {
                var playerObject = GameObject.FindGameObjectWithTag("Player");

                if (playerObject != null)
                {
                    playerDamage = playerObject.GetComponent<PlayerDamage>();

                    if (playerDamage != null)
                    {
                        _playerFound = true; // Exit the loop when the player and script are found.
                    }
                    else
                    {
                        Debug.LogWarning("PlayerDamage script not found on the 'Player' GameObject. Retrying...");
                    }
                }
                else
                {
                    Debug.LogWarning("Player GameObject not found. Retrying...");
                }

                yield return new WaitForSeconds(5.0f); // Adjust the delay between attempts.
            }
        }
    }
}
