using System;
using System.Collections;
using Characters.Monsters.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        private PlayerMovement _playerMovement;
        
        //health
        public float maxHealth = 100f;
        public float currentHealth;
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
        public bool targetLocked;
        private bool _iFrames;


        //gear stuff
        public GameObject[] weaponsList;
        public GameObject activeWeapon; //equipped weapon
        public GameObject activeShield; //equipped weapon
        private GameObject _previousActiveWeapon;
        public GameObject[] armoursList;
        public GameObject activeArmor; //equipped armor
        private GameObject _previousActiveArmor;
        private string _equippedWeaponName;
        private string _equippedArmorName;
        public Transform weaponBone;
        public Transform shieldBone;
        public GameObject weaponSlot;
        public GameObject armorSlot;
        public int bandageCount = 5;
        public Transform projectileSpawnLoc;
        public GameObject projectileObj;


        public Image blindingImage;
        
        //floating damage text
        public GameObject damageReceivedTextPrefab;
        public GameObject damageDoneTextPrefab;
        public GameObject healthReceivedPrefab;
        public GameObject damageDoneLocation;
        public GameObject damageReceivedLocation;
        public GameObject healthReceivedLocation;

        private void Awake()
        {
            _playerMovement = GetComponent<PlayerMovement>();
            currentHealth = maxHealth;
            
            //set gear
            //AttachWeapon();
            //AttachArmor();
        }

        private void Start()
        {
            // Initialize the Gear variable at the start.
            _previousActiveWeapon = activeWeapon;
            _previousActiveArmor = activeArmor;
            _equippedWeaponName = activeWeapon.name;
            _equippedArmorName = activeArmor.name;
            SwitchGear(); //trigger gear update on start
            conditionType = IDamageStats.ConditionType.None; //set condition to none
            activeWeapon.GetComponent<BoxCollider>().enabled = false; //set the weapon collision off at start
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
                if (_previousActiveWeapon == activeWeapon && _previousActiveArmor == activeArmor) return;
                SwitchGear();
                // Update the previous Gear variable.
                _previousActiveWeapon = activeWeapon;
                _previousActiveArmor = activeArmor;
            }
        }

        //this receives the damage
        private void TakeDamage(float damage)
        {
            // Subtract the calculated damage from the current health.
            currentHealth -= damage;
            
            //floating text
            var damageTextInstance =  Instantiate(damageReceivedTextPrefab, damageReceivedLocation.transform);
            damageTextInstance.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = damage.ToString("F");

            // Implement any additional logic for handling death, etc.
            if (!(currentHealth <= 0f)) return;
            _playerMovement.PlayerDied();
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

            //send the damage to the Take Damage function
            if (_playerMovement.isBlocking)
            {
                TakeDamage(_totalDamageTaken / 3);
                _playerMovement.CalculateStamina(0f, 10f);
                Debug.Log("player took" + _totalDamageTaken / 3 + "damage while blocking");
            }
            else
            {
                TakeDamage(_totalDamageTaken);
                Debug.Log("player took" + _totalDamageTaken + "damage");
            }
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

        //Status Effects
        #region Execute Status Effects

        private void PushBack()
        {
            StartCoroutine(_playerMovement.FallBack());
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
                blindingImage.GetComponent<Image>().enabled = true;

                // Wait for the tick interval.
                yield return new WaitForSeconds(1);

                elapsedTime += 1;
            }

            blindingImage.GetComponent<Image>().enabled = false;
        }
        
        private IEnumerator Stagger(float duration)
        {
            var elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                // Apply stagger.
                Debug.Log("is Stunned");
                _playerMovement.canMove = false;
                _playerMovement.canExecute = false;
                
                // Wait for the tick interval.
                yield return new WaitForSeconds(1);

                elapsedTime += 1;
            }
            _playerMovement.canMove = true;
            _playerMovement.canExecute = true;
        }
        
        #endregion

        //switch gear
        private void SwitchGear()
        {
            switch (_equippedWeaponName)
            {
                case "Fist(Clone)":
                    baseDamage = 1f;
                    damageType = IDamageStats.DamageType.Blunt;
                    break;
                case "Sword(Clone)":
                    baseDamage = 10f;
                    damageType = IDamageStats.DamageType.Slashing;
                    break;
                case "Spear(Clone)":
                    baseDamage = 8f;
                    damageType = IDamageStats.DamageType.Piercing;
                    break;
                case "Hammer(Clone)":
                    baseDamage = 17f;
                    damageType = IDamageStats.DamageType.Blunt;
                    break;
                case "Crossbow(Clone)":
                    baseDamage = 7f;
                    damageType = IDamageStats.DamageType.Piercing;
                    break;
            }

            armorType = _equippedArmorName switch
            {
                "Naked" => IDamageStats.ArmorType.Light,
                "Light" => IDamageStats.ArmorType.Light,
                "Medium" => IDamageStats.ArmorType.Medium,
                "Heavy" => IDamageStats.ArmorType.Heavy,
                _ => armorType
            };
        }

        //attach weapon to bone
        public void AttachWeapon()
        {
            if (weaponsList != null && weaponBone != null && shieldBone != null)
            {
                //destroy previous prefab
                if (weaponBone.childCount > 0)
                {
                    Destroy(weaponBone.GetChild(0).gameObject);
                    _playerMovement.AimingCameraOff();
                    
                    if (shieldBone.childCount > 0)
                    {
                        Destroy(shieldBone.GetChild(0).gameObject);
                    }
                }
                
                // Instantiate the prefab and make the attachedPrefab a child of the bone.
                if (weaponSlot.transform.childCount < 1)
                {
                    activeWeapon = Instantiate(weaponsList[0], weaponBone, true);
                    activeShield = Instantiate(weaponsList[6], shieldBone, true);
                    _playerMovement.AnimControllerChange(0);
                }
                else if (weaponSlot.transform.GetChild(0).gameObject.name == "SwordIcon(Clone)")
                {
                    activeWeapon = Instantiate(weaponsList[1], weaponBone, true);
                    activeShield = Instantiate(weaponsList[2], shieldBone, true);
                    _playerMovement.AnimControllerChange(0);
                } 
                else if (weaponSlot.transform.GetChild(0).gameObject.name == "SpearIcon(Clone)")
                {
                    activeWeapon = Instantiate(weaponsList[3], weaponBone, true);
                    activeShield = Instantiate(weaponsList[6], shieldBone, true);
                    _playerMovement.AnimControllerChange(1);
                }
                else if (weaponSlot.transform.GetChild(0).gameObject.name == "HammerIcon(Clone)")
                {
                    activeWeapon = Instantiate(weaponsList[4], weaponBone, true);
                    activeShield = Instantiate(weaponsList[6], shieldBone, true);
                    _playerMovement.AnimControllerChange(2);
                }
                else if (weaponSlot.transform.GetChild(0).gameObject.name == "CrossbowIcon(Clone)")
                {
                    activeWeapon = Instantiate(weaponsList[5], weaponBone, true);
                    activeShield = Instantiate(weaponsList[6], shieldBone, true);
                    _playerMovement.AnimControllerChange(3);
                    _playerMovement.AimingCameraOn();
                }
                
                
                // Adjust the local position to raise the weapon in the hand.
                var offsetWeapon = new Vector3(0f, 1f, 0f);
                var offsetShield = new Vector3(0f, 0.4f, 0.3f); 
                activeWeapon.transform.localPosition = offsetWeapon;
                activeShield.transform.localPosition = offsetShield;

                // Reset the local rotation if needed.
                activeWeapon.transform.localRotation = Quaternion.identity;
            }
            else
            {
                Debug.LogError("Bone or prefab reference is missing.");
            }
        }

        public void AttachArmor()
        {
            if (armoursList == null) return;
            if (armorSlot.transform.childCount < 1)
            {
                activeArmor = armoursList[0];
            }
            else if (armorSlot.transform.GetChild(0).gameObject.name == "Light(Clone)")
            {
                activeArmor = armoursList[1];
            }
            else if (armorSlot.transform.GetChild(0).gameObject.name == "Medium(Clone)")
            {
                activeArmor = armoursList[2];
            }
            else if (armorSlot.transform.GetChild(0).gameObject.name == "Heavy(Clone)")
            {
                activeArmor = armoursList[3];
            }
        }


        //Skills
        #region Skills
        //The skill modifier gets applied to the base damage on collision
        public void DoNormalAttack()
        {
            skillModifier = 1f; // to be added depending on the type of skill used
            hasCondition = false;
            conditionType = IDamageStats.ConditionType.None;
        }

        public void DoHeavyAttack()
        {
            //modifiers change depending on weapon
            hasCondition = false;
            conditionType = IDamageStats.ConditionType.None;
            skillModifier = _equippedWeaponName switch
            {
                "Sword(Clone)" => 4f,
                "Spear(Clone)" => 3f,
                "Hammer(Clone)" => 6f,
                "GreatSword(Clone)" => 5f,
                "Daggers(Clone)" => 2f,
                "Crossbow(Clone)" => 3f,
                "Bow(Clone)" => 2f,
                _ => skillModifier
            };
        }

        public void DoSkill1()
        {
            //modifiers change depending on weapon
            switch (_equippedWeaponName)
            {
                case "Sword(Clone)":
                    skillModifier = 3f;
                    hasCondition = true;
                    conditionDamage = 3;
                    conditionTime = 5;
                    conditionType = IDamageStats.ConditionType.Bleed;
                    break;
                case "Spear(Clone)":
                    skillModifier = 3f;
                    hasCondition = true;
                    conditionDamage = 5;
                    conditionTime = 5;
                    conditionType = IDamageStats.ConditionType.Bleed;
                    break;
                case "Hammer(Clone)":
                    skillModifier = 4f;
                    hasCondition = true;
                    conditionDamage = 0;
                    conditionTime = 0;
                    conditionType = IDamageStats.ConditionType.Stagger;
                    break;
                case "GreatSword(Clone)":
                    skillModifier = 4f;
                    hasCondition = true;
                    conditionDamage = 5;
                    conditionTime = 5;
                    conditionType = IDamageStats.ConditionType.Bleed;
                    break;
                case "Daggers(Clone)":
                    skillModifier = 3f;
                    hasCondition = true;
                    conditionDamage = 3;
                    conditionTime = 5;
                    conditionType = IDamageStats.ConditionType.Bleed;
                    break;
                case "Crossbow(Clone)":
                    skillModifier = 5f;
                    hasCondition = true;
                    conditionDamage = 5;
                    conditionTime = 5;
                    conditionType = IDamageStats.ConditionType.Bleed;
                    break;
                case "Bow(Clone)":
                    skillModifier = 3f;
                    hasCondition = true;
                    conditionDamage = 2;
                    conditionTime = 5;
                    conditionType = IDamageStats.ConditionType.Bleed;
                    break;
            }
        }

        public void DoSkill2()
        {
            //modifiers change depending on weapon
            switch (_equippedWeaponName)
            {
                case "Sword(Clone)":
                    skillModifier = 3f;
                    hasCondition = true;
                    conditionDamage = 0;
                    conditionTime = 0;
                    conditionType = IDamageStats.ConditionType.Stagger;
                    break;
                case "Spear(Clone)":
                    skillModifier = 4f;
                    hasCondition = true;
                    conditionDamage = 0;
                    conditionTime = 0;
                    conditionType = IDamageStats.ConditionType.PushBack;
                    break;
                case "Hammer(Clone)":
                    skillModifier = 5f;
                    hasCondition = true;
                    conditionDamage = 0;
                    conditionTime = 0;
                    conditionType = IDamageStats.ConditionType.PushBack;
                    break;
                case "GreatSword(Clone)":
                    skillModifier = 3f;
                    hasCondition = true;
                    conditionDamage = 0;
                    conditionTime = 0;
                    conditionType = IDamageStats.ConditionType.Stagger;
                    break;
                case "Daggers(Clone)":
                    skillModifier = 5f;
                    hasCondition = false;
                    conditionType = IDamageStats.ConditionType.None;
                    break;
                case "Crossbow(Clone)":
                    skillModifier = 5f;
                    hasCondition = true;
                    conditionDamage = 0;
                    conditionTime = 0;
                    conditionType = IDamageStats.ConditionType.PushBack;
                    break;
                case "Bow(Clone)":
                    skillModifier = 5f;
                    hasCondition = false;
                    conditionType = IDamageStats.ConditionType.None;
                    break;
            }
        }

        [SerializeField] private float shootForce;
        public void SpawnProjectile()
        {
            var projectile = Instantiate(projectileObj, projectileSpawnLoc.position, _playerMovement.aimingCamera.transform.rotation);
            
            var cameraForward = _playerMovement.aimingCamera.transform.forward;

            //Calculate Direction
            var forceDirection = cameraForward;

            if (Physics.Raycast(_playerMovement.aimingCamera.transform.position, cameraForward, out var hit, 500f))
            {
                forceDirection = (hit.point - projectileSpawnLoc.position).normalized;
            }
            
            //add force
            var addForce = forceDirection * shootForce + transform.up * 0;
            projectile.GetComponent<Rigidbody>().AddForce(addForce, ForceMode.Impulse);
        }

        public void DoHeal()
        {
            currentHealth += 30f; //heal for 30
            //check to not go above max health
            if (currentHealth >= maxHealth)
            {
                currentHealth = maxHealth;
            }
            
            //floating text
            var damageTextInstance =  Instantiate(healthReceivedPrefab, healthReceivedLocation.transform);
            damageTextInstance.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "+30";
        }
        #endregion
        
        //iFrames
        public void StartIFrames()
        {
            _iFrames = true;
        }

        public void StopIFrames()
        {
            _iFrames = false;
        }

        //look for collision and receive damage
        private void OnTriggerEnter(Collider other)
        {
            // Check if the collision is with a monster weapon.
            if (other.gameObject.layer != LayerMask.NameToLayer("MonsterWeapon")) return;
            
            //check if player has iFrames
            if (_iFrames) return;

            // Access the damage script on the colliding object parent.
            var damageScript = other.gameObject.GetComponentInParent<MonsterDamage>();

            if (damageScript == null) return;
            // Calculate and apply the damage.
            CalculateTotalDamageReceived(damageScript.baseDamage + damageScript.skillModifier,
                damageScript.damageType);

            //if the skill used by enemy has a condition add it and make it false
            if (!damageScript.hasCondition) return;
            ApplyCondition(damageScript.conditionDamage, damageScript.conditionTime, damageScript.conditionType);
            damageScript.hasCondition = false;
        }
    }
}
