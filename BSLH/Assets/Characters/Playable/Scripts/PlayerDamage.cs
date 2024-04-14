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
        public enum ArmorType { Naked, Light, Medium, Heavy }
        public enum EssenceType { None, Offence, Defence, Speed }
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
        public IDamageStats.EssenceType essenceType; //this essence type
        public IDamageStats.ConditionType conditionType; //this condition type
        public float baseDamage; //weapons flat damage before modifiers
        public float conditionDamage; //damage over time value
        public float conditionTime; //damage over time duration
        public bool hasCondition; //check if damage over time is applied
        public float skillModifier = 1f; // to be added depending on the type of skill used
        private float _essenceDefence; // to be added depending on the type of essence used
        private float _essenceOffence; // to be added depending on the type of essence used
        public bool targetLocked;
        private bool _iFrames;
        [SerializeField] private float shootForce;


        //gear stuff
        public GameObject[] weaponsList;
        public GameObject activeWeapon; //equipped weapon
        public GameObject activeShield; //equipped shield
        private GameObject _previousActiveWeapon;
        public GameObject[] armoursList;
        public GameObject activeArmor; //equipped armor
        private GameObject _previousActiveArmor;
        public GameObject[] essenceList;
        public GameObject activeEssence; //equipped essence
        private GameObject _previousActiveEssence;
        private string _equippedWeaponName;
        private string _equippedArmorName;
        private string _equippedEssenceName;
        public Transform weaponBone;
        public GameObject weaponSlot;
        public GameObject armorSlot;
        public GameObject essenceSlot;
        public int bandageCount = 5;
        public Transform projectileSpawnLoc;
        public GameObject projectileObj;
        private Vector3 _offsetWeapon;
        private Quaternion _offsetWeaponRotation;


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
        }

        private void Start()
        {
            // Initialize the Gear variable at the start.
            _previousActiveWeapon = activeWeapon;
            _previousActiveArmor = activeArmor;
            _previousActiveEssence = activeEssence;
            _equippedWeaponName = activeWeapon.name;
            _equippedArmorName = activeArmor.name;
            _equippedEssenceName = activeEssence.name;
            SwitchGear(); //trigger gear update on start
            conditionType = IDamageStats.ConditionType.None; //set condition to none
            activeWeapon.GetComponent<BoxCollider>().enabled = false; //set the weapon collision off at start
        }

        private void Update()
        {
            // Check if there is an equipped weapon.
            if (activeWeapon != null || activeArmor != null || activeEssence != null)
            {
                // Get the name of the equipped GameObject.
                _equippedWeaponName = activeWeapon.name;
                _equippedArmorName = activeArmor.name;
                _equippedEssenceName = activeEssence.name;

                // Check if the active Gear GameObject has changed.
                if (_previousActiveWeapon == activeWeapon && _previousActiveArmor == activeArmor && _previousActiveEssence == activeEssence) return;
                SwitchGear();
                // Update the previous Gear variable.
                _previousActiveWeapon = activeWeapon;
                _previousActiveArmor = activeArmor;
                _previousActiveEssence = activeEssence;
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
                IDamageStats.ArmorType.Naked => 2f,
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
                IDamageStats.ArmorType.Naked => 2f,
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
                IDamageStats.ArmorType.Naked => 2f,
                IDamageStats.ArmorType.Light => 1.1f,
                IDamageStats.ArmorType.Medium => 1.1f,
                IDamageStats.ArmorType.Heavy => 1.4f,
                _ => 1f
            };
        }
        
        private float GetDamageReduction()
        {
            // Define damage flat reduction values for each armor type.
            return armorType switch
            {
                IDamageStats.ArmorType.Naked => 0f + _essenceDefence,
                IDamageStats.ArmorType.Light => 1f + _essenceDefence,
                IDamageStats.ArmorType.Medium => 3f + _essenceDefence,
                IDamageStats.ArmorType.Heavy => 5f + _essenceDefence,
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
                    baseDamage = 10f + _essenceOffence;
                    damageType = IDamageStats.DamageType.Slashing;
                    break;
                case "Spear(Clone)":
                    baseDamage = 8f + _essenceOffence;
                    damageType = IDamageStats.DamageType.Piercing;
                    break;
                case "Hammer(Clone)":
                    baseDamage = 15f + _essenceOffence;
                    damageType = IDamageStats.DamageType.Blunt;
                    break;
                case "Crossbow(Clone)":
                    baseDamage = 7f + _essenceOffence;
                    damageType = IDamageStats.DamageType.Piercing;
                    break;
            }

            armorType = _equippedArmorName switch
            {
                "Naked" => IDamageStats.ArmorType.Naked,
                "Light" => IDamageStats.ArmorType.Light,
                "Medium" => IDamageStats.ArmorType.Medium,
                "Heavy" => IDamageStats.ArmorType.Heavy,
                _ => armorType
            };

            switch (_equippedEssenceName)
            {
                case "None":
                    _essenceOffence = 0f;
                    _essenceDefence = 0f;
                    gameObject.GetComponent<Animator>().speed = 1;
                    essenceType = IDamageStats.EssenceType.None;
                    break;
                case "Offence":
                    _essenceOffence = 3f;
                    _essenceDefence = 0f;
                    gameObject.GetComponent<Animator>().speed = 1;
                    essenceType = IDamageStats.EssenceType.Offence;
                    break;
                case "Defence":
                    _essenceDefence = 2f;
                    _essenceOffence = 0f;
                    gameObject.GetComponent<Animator>().speed = 1;
                    essenceType = IDamageStats.EssenceType.Defence;
                    break;
                case "Speed":
                    _essenceOffence = 0f;
                    _essenceDefence = 0f;
                    gameObject.GetComponent<Animator>().speed = 1.3f; //increase speed animation
                    essenceType = IDamageStats.EssenceType.Speed;
                    break;
            }
        }

        //attach weapon to bone
        public void AttachWeapon()
        {
            if (weaponsList != null && weaponBone != null)
            {
                //destroy previous prefab
                if (weaponBone.childCount > 0)
                {
                    Destroy(weaponBone.GetChild(0).gameObject);
                    activeShield.SetActive(false);
                    _playerMovement.AimingCameraOff();
                }
                
                // Instantiate the prefab and make the attachedPrefab a child of the bone.
                if (weaponSlot.transform.childCount < 1)
                {
                    activeWeapon = Instantiate(weaponsList[0], weaponBone, true);
                    activeShield.SetActive(false);
                    _playerMovement.AnimControllerChange(0);
                }
                else if (weaponSlot.transform.GetChild(0).gameObject.name == "SwordIcon(Clone)")
                {
                    activeWeapon = Instantiate(weaponsList[1], weaponBone, true);
                    activeShield.SetActive(true);
                    _playerMovement.AnimControllerChange(0);
                    
                    // Adjust the local position to raise the weapon in the hand.
                    _offsetWeapon = new Vector3(0.2f, -0.4f, 0.1f);
                    _offsetWeaponRotation = Quaternion.Euler(10f, -115f, 0f);
                } 
                else if (weaponSlot.transform.GetChild(0).gameObject.name == "SpearIcon(Clone)")
                {
                    activeWeapon = Instantiate(weaponsList[3], weaponBone, true);
                    activeShield.SetActive(false);
                    _playerMovement.AnimControllerChange(1);

                    // Adjust the local position to raise the weapon in the hand.
                    _offsetWeapon = new Vector3(0.4f, 0.5f, 0f);
                    _offsetWeaponRotation = Quaternion.Euler(-17f, -10f, -35f);
                }
                else if (weaponSlot.transform.GetChild(0).gameObject.name == "HammerIcon(Clone)")
                {
                    activeWeapon = Instantiate(weaponsList[4], weaponBone, true);
                    activeShield.SetActive(false);
                    _playerMovement.AnimControllerChange(2);
                    
                    // Adjust the local position to raise the weapon in the hand.
                    _offsetWeapon = new Vector3(0.9f, 1.4f, -0.3f);
                    _offsetWeaponRotation = Quaternion.Euler(-21f, 0.2f, -27f);
                }
                else if (weaponSlot.transform.GetChild(0).gameObject.name == "CrossbowIcon(Clone)")
                {
                    activeWeapon = Instantiate(weaponsList[5], weaponBone, true);
                    activeShield.SetActive(false);
                    _playerMovement.AnimControllerChange(3);
                    _playerMovement.AimingCameraOn();
                    
                    // Adjust the local position to raise the weapon in the hand.
                    _offsetWeapon = new Vector3(0.2f, 0.2f, 0.1f);
                    _offsetWeaponRotation = Quaternion.Euler(-11f, -34f, 14f);
                }
                
                activeWeapon.transform.localPosition = _offsetWeapon;
                activeWeapon.transform.localRotation = _offsetWeaponRotation;
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
        
        public void AttachEssence()
        {
            if (essenceList == null) return;
            if (essenceSlot.transform.childCount < 1)
            {
                activeEssence = essenceList[0];
            }
            else if (essenceSlot.transform.GetChild(0).gameObject.name == "Offence(Clone)")
            {
                activeEssence = essenceList[1];
            }
            else if (essenceSlot.transform.GetChild(0).gameObject.name == "Defence(Clone)")
            {
                activeEssence = essenceList[2];
            }
            else if (essenceSlot.transform.GetChild(0).gameObject.name == "Speed(Clone)")
            {
                activeEssence = essenceList[3];
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
                "Sword(Clone)" => 5f,
                "Spear(Clone)" => 4f,
                "Hammer(Clone)" => 6f,
                "Crossbow(Clone)" => 4f,
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
                    skillModifier = 5f;
                    hasCondition = true;
                    conditionDamage = 0;
                    conditionTime = 0;
                    conditionType = IDamageStats.ConditionType.Stagger;
                    break;
                case "Crossbow(Clone)":
                    skillModifier = 5f;
                    hasCondition = true;
                    conditionDamage = 0;
                    conditionTime = 0;
                    conditionType = IDamageStats.ConditionType.Stagger;
                    break;
            }
        }

        public void DoSkill2()
        {
            //modifiers change depending on weapon
            switch (_equippedWeaponName)
            {
                case "Sword(Clone)":
                    skillModifier = 4f;
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
                case "Crossbow(Clone)":
                    skillModifier = 5f;
                    hasCondition = true;
                    conditionDamage = 3;
                    conditionTime = 5;
                    conditionType = IDamageStats.ConditionType.Bleed;
                    break;
            }
        }
        
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
            
            //play hit anim
            _playerMovement.PlayerHit();

            //if the skill used by enemy has a condition add it and make it false
            if (!damageScript.hasCondition) return;
            ApplyCondition(damageScript.conditionDamage, damageScript.conditionTime, damageScript.conditionType);
            damageScript.hasCondition = false;
        }
    }
}
