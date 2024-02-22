using System;
using System.Collections;
using System.Collections.Generic;
using Characters.Monsters.Scripts.UtilityCore;
using Characters.Playable.Scripts;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Characters.Monsters.Scripts
{
    public class MonsterDamage : MonoBehaviour, IDamageStats
    {
        private AIController _aiController;
        [SerializeField] private List<GameObject> droppedLoot;

        //health
        public float maxHealth = 100;
        public float currentHealth;
        private float _totalDamageTaken;

        //stats
        public IDamageStats.DamageType damageType; //this condition type
        public IDamageStats.ArmorType armorType; //this condition type
        public IDamageStats.ConditionType conditionType; //this condition type
        public float baseDamage; //weapons flat damage before modifiers
        public float conditionDamage; //damage over time value
        public float conditionTime; //damage over time duration
        public bool hasCondition; //check if damage over time is applied
        public float skillModifier = 1f; // to be added depending on the type of skill used
        
        private string _monsterName;
        public GameObject projectile;
        public GameObject projectileSpawner;
        public GameObject vfxObj;
        
        //animation cache
        private Animator _animator;
        private static readonly int Died = Animator.StringToHash("Died");
        
        private void Awake()
        {
            _aiController = GetComponent<AIController>();
            currentHealth = maxHealth;
            _monsterName = gameObject.name;
            conditionType = IDamageStats.ConditionType.None; //set condition to none
            _animator = GetComponent<Animator>();
        }

        private void TakeDamage(float damage)
        {
            if (currentHealth <= 0) return;
            
            // Subtract the calculated damage from the current health.
            currentHealth -= damage;

            //floating text
            var damageTextInstance =  Instantiate(_aiController.playerDamage.damageDoneTextPrefab,_aiController.playerDamage.damageDoneLocation.transform);
            damageTextInstance.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = damage.ToString("F");
            
            // Implement any additional logic for handling damage effects, death, etc.
            if (currentHealth <= 0f)
            {
                // Entity is defeated, you can destroy or disable it, play death animation, etc.
                _animator.SetTrigger(Died);
                StartCoroutine(DiedTimer());
            }
        }
        
        private void CalculateTotalDamageReceived(float baseDamageReceived, IDamageStats.DamageType damageTypeReceived)
        {
            // Get defined damage multipliers.
            var slashingMultiplier = GetSlashingMultiplier();
            var piercingMultiplier = GetPiercingMultiplier();
            var bluntMultiplier = GetBluntMultiplier();
            var damageReduction = GetDamageReduction();

            // Calculate damage based on damage type and armor type.

            _totalDamageTaken = damageTypeReceived switch
            {
                IDamageStats.DamageType.Slashing => baseDamageReceived * slashingMultiplier - damageReduction,
                IDamageStats.DamageType.Piercing => baseDamageReceived * piercingMultiplier - damageReduction,
                IDamageStats.DamageType.Blunt => baseDamageReceived * bluntMultiplier - damageReduction,
                _ => 0f
            };

            TakeDamage(_totalDamageTaken);
        }

        private void ApplyCondition(float damage, float duration, IDamageStats.ConditionType condition)
        {
            switch (condition)
            {
                case IDamageStats.ConditionType.Poison:
                    StartCoroutine(DoTickingDamage(damage, duration));
                    break;
                case IDamageStats.ConditionType.Decay:
                    ApplyDecay(damage);
                    break;
                case IDamageStats.ConditionType.Blind:
                    StartCoroutine(BlindPlayer(duration));
                    break;
                case IDamageStats.ConditionType.Bleed:
                    StartCoroutine(DoTickingDamage(damage, duration));
                    break;
                case IDamageStats.ConditionType.Stagger:
                    StartCoroutine(Stagger(duration));
                    break;
                case IDamageStats.ConditionType.PushBack:
                    PushBack();
                    break;
                case IDamageStats.ConditionType.None:
                default:
                    throw new ArgumentOutOfRangeException(nameof(condition), condition, null);
            }
        }
        
        //defining multipliers
        #region Define Multipliers
        private float GetSlashingMultiplier()
        {
            // Define slashing damage multipliers for each armor type.
            return armorType switch
            {
                IDamageStats.ArmorType.Light => 1.4f,
                IDamageStats.ArmorType.Medium => 1.2f,
                IDamageStats.ArmorType.Heavy => 1f,
                _ => 1f
            };
        }

        private float GetPiercingMultiplier()
        {
            // Define piercing damage multipliers for each armor type.
            return armorType switch
            {
                IDamageStats.ArmorType.Light => 1.2f,
                IDamageStats.ArmorType.Medium => 1.4f,
                IDamageStats.ArmorType.Heavy => 1f,
                _ => 1f
            };
        }

        private float GetBluntMultiplier()
        {
            // Define blunt damage multipliers for each armor type.
            return armorType switch
            {
                IDamageStats.ArmorType.Light => 1.4f,
                IDamageStats.ArmorType.Medium => 1.4f,
                IDamageStats.ArmorType.Heavy => 1.3f,
                _ => 1f
            };
        }
        
        private float GetDamageReduction()
        {
            // Define damage flat reduction values for each armor type.
            return armorType switch
            {
                IDamageStats.ArmorType.Light => 1f,
                IDamageStats.ArmorType.Medium => 3f,
                IDamageStats.ArmorType.Heavy => 6f,
                _ => 0f
            };
        }
        #endregion

        //execute conditions
        #region Conditions

        private void PushBack()
        {
            //add pushback
            Debug.Log("pushed back");
        }
        
        private void ApplyDecay(float damage)
        {
            maxHealth -= damage;
            Debug.Log("decay applied");
        }
        
        private IEnumerator DoTickingDamage(float damage, float duration)
        {
            var elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                // Apply ticking damage.
                TakeDamage(damage);

                // Wait for the tick interval.
                yield return new WaitForSeconds(1);

                elapsedTime += 1;
            }
        }
        
        private IEnumerator BlindPlayer(float duration)
        {
            var elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                // Apply blinding.
                Debug.Log("Player is Blinded");

                // Wait for the tick interval.
                yield return new WaitForSeconds(1);

                elapsedTime += 1;
            }
        }
        
        private IEnumerator Stagger(float duration)
        {
            var elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                // Apply stagger.
                Debug.Log("is Stunned");

                // Wait for the tick interval.
                yield return new WaitForSeconds(1);

                elapsedTime += 1;
            }
        }
        
        private IEnumerator Rage()
        {
            var elapsedTime = 0f;

            while (elapsedTime < 10f)
            {
                var oldBaseDamage = baseDamage;
                // Apply rage
                baseDamage += 10f;
                var vfx = Instantiate(vfxObj, transform);
                Debug.Log("is enraged");

                // Wait for the tick interval.
                yield return new WaitForSeconds(10);

                elapsedTime = 10f;
                baseDamage = oldBaseDamage;
                Destroy(vfx);
            }
        }

        #endregion
        
        //Skills
        #region Skills
        //The skill modifier gets applied to the base damage on collision
        public void DoNormalMeleeAttack()
        {
            skillModifier = 1f; // to be changed depending on the type of skill used
            hasCondition = false;
            conditionType = IDamageStats.ConditionType.None;
        }

        public void DoNormalRangedAttack()
        {
            switch (_monsterName)
            {
                case "Gorgon": //bow shot
                    skillModifier = 1f;
                    hasCondition = false;
                    conditionType = IDamageStats.ConditionType.None;
                    break;
                case "Gargoyle": //throw
                    skillModifier = 1f;
                    hasCondition = false;
                    conditionType = IDamageStats.ConditionType.None;
                    break;
                case "Satyr": //headbutt
                    skillModifier = 4f;
                    hasCondition = true;
                    conditionDamage = 15;
                    conditionTime = 0;
                    conditionType = IDamageStats.ConditionType.Decay;
                    break;
            }
            //spawn projectile
            var newProjectile = Instantiate(projectile, projectileSpawner.transform, true);
            newProjectile.transform.position = projectileSpawner.transform.position;
        }

        public void DoSpecialMeleeAttack()
        {
            //modifiers change depending on monster type
            switch (_monsterName)
            {
                case "Gorgon": //tail swipe
                    skillModifier = 3f;
                    hasCondition = true;
                    conditionDamage = 0;
                    conditionTime = 3;
                    conditionType = IDamageStats.ConditionType.Stagger;
                    break;
                case "Gargoyle": //bite
                    skillModifier = 4f;
                    hasCondition = true;
                    conditionDamage = 4;
                    conditionTime = 5;
                    conditionType = IDamageStats.ConditionType.Bleed;
                    currentHealth += 20f; //heal for 20
                    //check to not go above max health
                    if (currentHealth >= maxHealth)
                    {
                        currentHealth = maxHealth;
                    }
                    break;
                case "Satyr": //shield bash
                    skillModifier = 3f;
                    hasCondition = true;
                    conditionDamage = 0;
                    conditionTime = 0;
                    conditionType = IDamageStats.ConditionType.PushBack;
                    break;
            }
        }

        public void DoSpecialRangedAttack()
        {
            //modifiers change depending on monster type
            switch (_monsterName)
            {
                case "Gorgon": //poison spit
                    skillModifier = 3f;
                    hasCondition = true;
                    conditionDamage = 5;
                    conditionTime = 5;
                    conditionType = IDamageStats.ConditionType.Poison;
                    break;
                case "Gargoyle": //double slam
                    skillModifier = 5f;
                    hasCondition = true;
                    conditionDamage = 0;
                    conditionTime = 3;
                    conditionType = IDamageStats.ConditionType.Stagger;
                    _aiController.Movement.MoveInMelee();
                    break;
                case "Satyr": //decay magic
                    skillModifier = 5f;
                    hasCondition = true;
                    conditionDamage = 5;
                    conditionTime = 0;
                    conditionType = IDamageStats.ConditionType.Decay;
                    
                    //spawn projectile
                    var newProjectile = Instantiate(projectile, projectileSpawner.transform, true);
                    newProjectile.transform.position = projectileSpawner.transform.position;
                    
                    break;
            }
        }
        
        public void DoUltimateAttack()
        {
            //modifiers change depending on monster type
            switch (_monsterName)
            {
                case "Gorgon": //blinding shot
                    skillModifier = 5f;
                    hasCondition = true;
                    conditionDamage = 0;
                    conditionTime = 3;
                    conditionType = IDamageStats.ConditionType.Blind;
                    break;
                case "Gargoyle": //rage
                    skillModifier = 1f;
                    hasCondition = false;
                    conditionType = IDamageStats.ConditionType.None;
                    StartCoroutine(Rage());
                    break;
                case "Satyr": //heal
                    skillModifier = 1f;
                    hasCondition = false;
                    conditionType = IDamageStats.ConditionType.None;
                    currentHealth += 40f; //heal for 40 every ultimate
                    
                    //playVfx
                    Instantiate(vfxObj, transform);
                    break;
            }
        }
        #endregion
        
        private void OnTriggerEnter(Collider other)
        {
            // Check if the collision is with a player weapon.
            if (other.gameObject.layer != LayerMask.NameToLayer("PlayerWeapon")) return;
            // Access the damage script on the colliding object parent.
            var damageScript = other.gameObject.GetComponentInParent<PlayerDamage>();

            if (damageScript == null) return;
            // Calculate and apply the damage.
            CalculateTotalDamageReceived(damageScript.baseDamage + damageScript.skillModifier, damageScript.damageType);
            
            //if the skill used by enemy has a condition add it and make it false
            if (!damageScript.hasCondition) return;
            ApplyCondition(damageScript.conditionDamage, damageScript.conditionTime, damageScript.conditionType);
            damageScript.hasCondition = false;
        }

        private IEnumerator DiedTimer()
        {
            var elapsedTime = 0f;
            const float delayBetweenSpawns = 0.3f;

            while (elapsedTime < 5f)
            {
                foreach (var loot in droppedLoot)
                {
                    // Get the position of the spawning object
                    var spawnPosition = transform.position;

                    // Calculate random angle in radians
                    var randomAngle = Random.Range(0f, 2f * Mathf.PI);

                    // Calculate random distance from the main position
                    var randomDistance = Random.Range(0f, 2f);

                    // Convert polar coordinates to Cartesian coordinates
                    spawnPosition.x += randomDistance * Mathf.Cos(randomAngle);
                    spawnPosition.z += randomDistance * Mathf.Sin(randomAngle);
                    spawnPosition.y += 1.5f;

                    // Instantiate the object at the modified spawn position
                    Instantiate(loot, spawnPosition, Quaternion.identity);

                    // Wait for the specified delay between spawns
                    yield return new WaitForSeconds(delayBetweenSpawns);
                }

                GameObject.Find("Player").GetComponent<PlayerMovement>().TriggerTeleportBack();
                
                // Wait for the tick interval.
                yield return new WaitForSeconds(5f);
                elapsedTime += 5f;
            }
            // Destroy the gameObject after all spawns are done
            Destroy(gameObject);
        }

    }
}
