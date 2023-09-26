using System;
using System.Collections;
using Characters.Monsters.Scripts;
using UnityEngine;

namespace Characters.Playable.Scripts
{
    public interface IDamageStats
    {
        public enum DamageType { Slashing, Piercing, Blunt }
        public enum ArmorType { Light, Medium, Heavy }
        public enum ConditionType { None, Poison, Decay, Blind, Bleed, Stagger, PushBack }
    }

    public class PlayerDamage : MonoBehaviour, IDamageStats
    {
        //health
        public float maxHealth = 100f;
        private float _currentHealth;
        private float _totalDamageTaken; // used to calculate flat damage taken

        //stats
        public IDamageStats.DamageType damageType; //this damage type
        public IDamageStats.ArmorType armorType; //this armor type
        public IDamageStats.ConditionType conditionType; //this condition type
        public float baseDamage; //weapons flat damage before modifiers
        public float conditionDamage; //damage over time value
        public float conditionTime; //damage over time duration
        public bool hasCondition; //check if damage over time is applied
        public float skillModifier = 1f; // to be added depending on the type of skill used


        //gear stuff
        public GameObject activeWeapon; //equipped weapon
        private GameObject _previousActiveWeapon;
        public GameObject activeArmor; //equipped armor
        private GameObject _previousActiveArmor;
        private string _equippedWeaponName;
        private string _equippedArmorName;

        private void Start()
        {
            _currentHealth = maxHealth;

            // Initialize the Gear variable at the start.
            _previousActiveWeapon = activeWeapon;
            _previousActiveArmor = activeArmor;
            _equippedWeaponName = activeWeapon.name;
            _equippedArmorName = activeArmor.name;
            SwitchGear(); //trigger gear update on start
            conditionType = IDamageStats.ConditionType.None; //set condition to none
        }

        private void Update()
        { 
            // Check if there is an equipped weapon.
            if (activeWeapon != null || activeArmor != null)
            {
                // Get the name of the equipped GameObject.
                _equippedWeaponName = activeWeapon.name;
                _equippedArmorName = activeArmor.name;

                // Check if the active Gear GameObject has changed.
                if (_previousActiveWeapon != activeWeapon || _previousActiveArmor != activeArmor)
                {
                    SwitchGear();
                    // Update the previous Gear variable.
                    _previousActiveWeapon = activeWeapon;
                    _previousActiveArmor = activeArmor;
                }
            }
        }

        //this receives the damage
        private void TakeDamage(float damage)
        {
            // Subtract the calculated damage from the current health.
            _currentHealth -= damage;

            // Implement any additional logic for handling damage effects, death, etc.
            if (_currentHealth <= 0f)
            {
                // Entity is defeated, you can destroy or disable it, play death animation, etc.
                Destroy(gameObject);
            }
        }

        //this calculates the hit damage
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

        //this gets the over time damage and conditions
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
                IDamageStats.ArmorType.Light => 1.3f,
                IDamageStats.ArmorType.Medium => 1.1f,
                IDamageStats.ArmorType.Heavy => 0.7f,
                _ => 1f
            };
        }

        private float GetPiercingMultiplier()
        {
            // Define piercing damage multipliers for each armor type.
            return armorType switch
            {
                IDamageStats.ArmorType.Light => 1.2f,
                IDamageStats.ArmorType.Medium => 1.3f,
                IDamageStats.ArmorType.Heavy => 1f,
                _ => 1f
            };
        }

        private float GetBluntMultiplier()
        {
            // Define blunt damage multipliers for each armor type.
            return armorType switch
            {
                IDamageStats.ArmorType.Light => 1.2f,
                IDamageStats.ArmorType.Medium => 1.2f,
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

        private void PushBack()
        {
            //add pushback
            Debug.Log("pushed back");
        }
        private void ApplyDecay(float damage)
        {
            maxHealth -= damage;
        }
        
        private IEnumerator DoTickingDamage(float damage, float duration)
        {
            var elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                // Apply ticking damage.
                TakeDamage(damage);

                // Wait for the tick interval.
                yield return new WaitForSeconds(5);

                elapsedTime += 5;
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
                yield return new WaitForSeconds(3);

                elapsedTime += 3;
            }
        }
        
        private IEnumerator Stagger(float duration)
        {
            var elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                // Apply blinding.
                Debug.Log("is Stunned");

                // Wait for the tick interval.
                yield return new WaitForSeconds(2);

                elapsedTime += 2;
            }
        }

        //switch gear
        private void SwitchGear()
        {
            switch (_equippedWeaponName)
            {
                case "Sword":
                    baseDamage = 10f;
                    damageType = IDamageStats.DamageType.Slashing;
                    break;
                case "Spear":
                    baseDamage = 8f;
                    damageType = IDamageStats.DamageType.Piercing;
                    break;
                case "Hammer":
                    baseDamage = 17f;
                    damageType = IDamageStats.DamageType.Blunt;
                    break;
                case "GreatSword":
                    baseDamage = 14f;
                    damageType = IDamageStats.DamageType.Slashing;
                    break;
                case "Daggers":
                    baseDamage = 8f;
                    damageType = IDamageStats.DamageType.Slashing;
                    break;
                case "Crossbow":
                    baseDamage = 7f;
                    damageType = IDamageStats.DamageType.Piercing;
                    break;
                case "Bow":
                    baseDamage = 5f;
                    damageType = IDamageStats.DamageType.Piercing;
                    break;
            }

            armorType = _equippedArmorName switch
            {
                "Light" => IDamageStats.ArmorType.Light,
                "Medium" => IDamageStats.ArmorType.Medium,
                "Heavy" => IDamageStats.ArmorType.Heavy,
                _ => armorType
            };
        }


        //Skills
        #region Skills
        //The skill modifier gets applied to the base damage on collision
        public void DoNormalAttack()
        {
            skillModifier = 1f; // to be added depending on the type of skill used
            hasCondition = false;
        }

        public void DoHeavyAttack()
        {
            //modifiers change depending on weapon
            hasCondition = false;
            skillModifier = _equippedWeaponName switch
            {
                "Sword" => 4f,
                "Spear" => 3f,
                "Hammer" => 6f,
                "GreatSword" => 5f,
                "Daggers" => 2f,
                "Crossbow" => 3f,
                "Bow" => 2f,
                _ => skillModifier
            };
        }

        public void DoSkill1()
        {
            //modifiers change depending on weapon
            switch (_equippedWeaponName)
            {
                case "Sword":
                    skillModifier = 3f;
                    hasCondition = true;
                    conditionDamage = 3;
                    conditionTime = 3;
                    conditionType = IDamageStats.ConditionType.Bleed;
                    break;
                case "Spear":
                    skillModifier = 3f;
                    hasCondition = true;
                    conditionDamage = 5;
                    conditionTime = 3;
                    conditionType = IDamageStats.ConditionType.Bleed;
                    break;
                case "Hammer":
                    skillModifier = 4f;
                    hasCondition = true;
                    conditionDamage = 0;
                    conditionTime = 0;
                    conditionType = IDamageStats.ConditionType.Stagger;
                    break;
                case "GreatSword":
                    skillModifier = 4f;
                    hasCondition = true;
                    conditionDamage = 5;
                    conditionTime = 3;
                    conditionType = IDamageStats.ConditionType.Bleed;
                    break;
                case "Daggers":
                    skillModifier = 3f;
                    hasCondition = true;
                    conditionDamage = 3;
                    conditionTime = 3;
                    conditionType = IDamageStats.ConditionType.Bleed;
                    break;
                case "Crossbow":
                    skillModifier = 5f;
                    hasCondition = true;
                    conditionDamage = 5;
                    conditionTime = 3;
                    conditionType = IDamageStats.ConditionType.Bleed;
                    break;
                case "Bow":
                    skillModifier = 3f;
                    hasCondition = true;
                    conditionDamage = 2;
                    conditionTime = 3;
                    conditionType = IDamageStats.ConditionType.Bleed;
                    break;
            }
        }

        public void DoSkill2()
        {
            //modifiers change depending on weapon
            switch (_equippedWeaponName)
            {
                case "Sword":
                    skillModifier = 3f;
                    hasCondition = true;
                    conditionDamage = 0;
                    conditionTime = 0;
                    conditionType = IDamageStats.ConditionType.Stagger;
                    break;
                case "Spear":
                    skillModifier = 4f;
                    hasCondition = true;
                    conditionDamage = 0;
                    conditionTime = 0;
                    conditionType = IDamageStats.ConditionType.PushBack;
                    break;
                case "Hammer":
                    skillModifier = 5f;
                    hasCondition = true;
                    conditionDamage = 0;
                    conditionTime = 0;
                    conditionType = IDamageStats.ConditionType.PushBack;
                    break;
                case "GreatSword":
                    skillModifier = 3f;
                    hasCondition = true;
                    conditionDamage = 0;
                    conditionTime = 0;
                    conditionType = IDamageStats.ConditionType.Stagger;
                    break;
                case "Daggers":
                    skillModifier = 5f;
                    hasCondition = false;
                    break;
                case "Crossbow":
                    skillModifier = 5f;
                    hasCondition = true;
                    conditionDamage = 0;
                    conditionTime = 0;
                    conditionType = IDamageStats.ConditionType.PushBack;
                    break;
                case "Bow":
                    skillModifier = 5f;
                    hasCondition = false;
                    break;
            }
        }
        #endregion

        
        //look for collision and receive damage
        private void OnCollisionEnter(Collision collision)
        {
            // Check if the collision is with a monster.
            if (!collision.gameObject.CompareTag("Monster")) return;
            // Access the damage script on the colliding object.
            var damageScript = collision.gameObject.GetComponent<MonsterDamage>();

            if (damageScript == null) return;
            // Calculate and apply the damage.
            CalculateTotalDamageReceived(damageScript.baseDamage + damageScript.skillModifier, damageScript.damageType);

            //if the skill used by enemy has a condition add it and make it false
            if (!damageScript.hasCondition) return;
            ApplyCondition(damageScript.conditionDamage, damageScript.conditionTime, damageScript.conditionType);
            damageScript.hasCondition = false;
        }
    }
}
