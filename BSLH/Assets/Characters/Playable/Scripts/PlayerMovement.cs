using System;
using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Characters.Playable.Scripts
{ 
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Controllers")]
        private Animator _animator;
        public RuntimeAnimatorController[] animatorController;
        private CharacterController _controller;
        private PlayerInput _playerInput;
        private PlayerDamage _playerDamage;
        private PlayerUI _playerUI;
        private GameObject _monster;
        
        private Vector2 _playerMovement;
        private bool _groundedPlayer;
        private bool _inventoryOpened;

        //camera stuff
        [Header("Camera Settings")] 
        public Transform freeCameraTargetPos;
        public Transform aimingCameraTargetPos;
        public AxisState xAxis, yAxis;
        private Vector2 _targetMovementVector = Vector2.zero;
        [SerializeField] private float rotationSpeed;
        [SerializeField][Range(0.001f, 2f)]
        private float movementInputSmoothTime;
        private Quaternion _playerRotation;
        public GameObject freeLookCamera;
        public GameObject lockedCamera;
        public GameObject aimingCamera;
        public CinemachineTargetGroup targetGroup;
        // Reference field for SmoothDamp of movement input
        private Vector2 _movementSmoothVelocity = Vector2.zero;

        //stats
        [Header("Stats")]
        public float stamina = 100f;
        [SerializeField] private float staminaRechargeRate;
        private bool _isSprinting;
        public bool isBlocking;
        public bool canExecute;
        public bool canMove;
        public bool canInteractWithMap;
        public bool canInteractWithCraftingBench;
        
        //combo State
        private enum ComboState { None, Attack1, Attack2, Attack3 }
        private ComboState _currentComboState = ComboState.None;
        private bool _canPerformCombo;
        public float comboCooldownTime = 0.5f; // Adjust as needed
        private float _lastComboTime;

        //animation cache
        private static readonly int Jump = Animator.StringToHash("Jump");
        private static readonly int IsBlocking = Animator.StringToHash("IsBlocking");
        private static readonly int Dodge = Animator.StringToHash("Dodge");
        private static readonly int LightAtt = Animator.StringToHash("LightAtt");
        private static readonly int LightAtt2 = Animator.StringToHash("LightAtt2");
        private static readonly int LightAtt3 = Animator.StringToHash("LightAtt3");
        private static readonly int HeavyAtt = Animator.StringToHash("HeavyAtt");
        private static readonly int Skill1 = Animator.StringToHash("Skill1");
        private static readonly int Skill2 = Animator.StringToHash("Skill2");
        private static readonly int Bandage = Animator.StringToHash("Bandage");
        private static readonly int Horizontal = Animator.StringToHash("Horizontal");
        private static readonly int Vertical = Animator.StringToHash("Vertical");
        private static readonly int Died = Animator.StringToHash("Died");
        private static readonly int Back = Animator.StringToHash("FallBack");
        private static readonly int TakeHit = Animator.StringToHash("TakeHit");

        private void Awake()
        {
            _playerInput = new PlayerInput();
            _playerDamage = GetComponent<PlayerDamage>();
            _animator = GetComponent<Animator>();
            _controller = GetComponent<CharacterController>();
            _playerUI = GetComponent<PlayerUI>();
        }

        private void Start()
        {
            _animator.runtimeAnimatorController = animatorController[0];
            _monster = GameObject.FindGameObjectWithTag("Monster");
            
            canExecute = true;
            canMove = true;
        }

        private void Update()
        {
            _groundedPlayer = _controller.isGrounded;
            
            _playerMovement = Vector2.SmoothDamp(_playerMovement, _targetMovementVector, ref _movementSmoothVelocity, movementInputSmoothTime);

            //movement && sprinting
            switch (_isSprinting && stamina >= 8f)
            {
                case true:
                    _animator.SetFloat(Horizontal, _playerMovement.x * 2f);
                    _animator.SetFloat(Vertical, _playerMovement.y * 2f);
                    CalculateStamina(8f, 0f);
                    break;
                case false:
                    _animator.SetFloat(Horizontal, _playerMovement.x);
                    _animator.SetFloat(Vertical, _playerMovement.y);
                    break;
            }

            //blocking
            switch (isBlocking && stamina >= 7f)
            {
                case true:
                    _animator.SetBool(IsBlocking, true);
                    CalculateStamina(7f, 0f);
                    break;
                case false:
                    _animator.SetBool(IsBlocking, false);
                    break;
            }
            
            //stamina regen
            if (!_isSprinting && !isBlocking && stamina <= 100f)
            {
                stamina += staminaRechargeRate * Time.deltaTime;

                if (stamina >= 100f)
                {
                    stamina = 100f;
                }
            }
            
            //target lock and body rotation
            if (_playerDamage.targetLocked && _monster != null)
            { 
                // Get direction from monster
                var targetDirection = _monster.transform.position - transform.position; 
                // Calculate the Y rotation based on the direction to the target.
                var targetYRotation = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;
                
                _playerRotation = Quaternion.Euler(0f, targetYRotation, 0f);
                transform.rotation = Quaternion.Lerp(transform.rotation, _playerRotation,
                    Time.deltaTime * rotationSpeed);
            }
            else
            {
                yAxis.Update(Time.deltaTime);
                xAxis.Update(Time.deltaTime);
            }
            
            // Reset combo cooldown timer
            if (_canPerformCombo && Time.time - _lastComboTime >= comboCooldownTime)
            {
                _canPerformCombo = false;
            }
        }

        public void LateUpdate()
        {
            if (aimingCamera.activeSelf)
            {
                var localEulerAngles = aimingCameraTargetPos.localEulerAngles;
                localEulerAngles = new Vector3(yAxis.Value, localEulerAngles.y, localEulerAngles.z);
                aimingCameraTargetPos.localEulerAngles = localEulerAngles;
                var transform1 = transform;
                transform1.eulerAngles = new Vector3(transform1.eulerAngles.x, xAxis.Value, transform1.localEulerAngles.z);
            }
            else if(freeLookCamera.activeSelf)
            {
                var localEulerAngles = freeCameraTargetPos.localEulerAngles;
                localEulerAngles = new Vector3(yAxis.Value, localEulerAngles.y, localEulerAngles.z);
                freeCameraTargetPos.localEulerAngles = localEulerAngles;
                
                if (_playerMovement != Vector2.zero)
                {
                    var transform1 = freeCameraTargetPos.transform;
                    transform1.eulerAngles = new Vector3(transform1.eulerAngles.x, xAxis.Value, transform1.localEulerAngles.z);

                    _playerRotation = Quaternion.Euler(0f, transform1.eulerAngles.y, 0f); 
                     transform.rotation = Quaternion.Lerp(transform.rotation, _playerRotation, rotationSpeed * Time.deltaTime);
                }
                else
                {
                    var transform2 = freeCameraTargetPos.transform;
                    transform2.eulerAngles = new Vector3(transform2.eulerAngles.x, xAxis.Value, transform2.localEulerAngles.z);
                }
            }
        }
        
        //action triggers
        #region Action Triggers
        public void PlayerMove(InputAction.CallbackContext context)
        {
            _targetMovementVector = (context.performed && canMove) ? context.ReadValue<Vector2>() : Vector2.zero;
        }

        public void DoJump(InputAction.CallbackContext context)
        {
            if (!context.performed || !canExecute || !_groundedPlayer || !(stamina >= 30f)) return;
            _animator.SetTrigger(Jump);
            CalculateStamina(0f, 30f);
        }

        public void PlayerSprint(InputAction.CallbackContext context)
        {
            if (context.started && stamina >= 8f && canExecute)
            {
                _isSprinting = true;
            } 
            else if (context.canceled || stamina < 8f)
            {
                _isSprinting = false;
            }
        }
        
        public void PlayerBlocking(InputAction.CallbackContext context)
        {
            if (context.started && stamina >= 7f && canExecute)
            {
                isBlocking = true;
            } 
            else if (context.canceled || stamina < 7f)
            {
                isBlocking = false;
            }
        }

        public void DoDodge(InputAction.CallbackContext context)
        {
            if (!context.performed || !(stamina >= 30f) || !canExecute) return;
            _animator.SetTrigger(Dodge);
            CalculateStamina(0f, 30f);
        }
        
        public void DoLightAttack(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _playerUI.NormalAttackPressed();
            }
            if (!context.performed || !(stamina >= 10f) || !canExecute) return;
            _playerDamage.DoNormalAttack();
            CalculateStamina(0f,10f);
            
            if (_canPerformCombo)
            {
                PerformComboAttack();
            }
            else
            {
                // Player can only perform the first attack, ignore subsequent inputs
                _currentComboState = ComboState.Attack1;
                _animator.SetTrigger(LightAtt);
                _lastComboTime = Time.time;
                _canPerformCombo = true;
            }
        }

        public void DoHeavyAttack(InputAction.CallbackContext context)
        {
            if (context.performed && stamina >= 20f && canExecute)
            {
                _animator.SetTrigger(HeavyAtt);
                _playerDamage.DoHeavyAttack();
                CalculateStamina(0f, 20f);
                _playerUI.HeavyAttackPressed();
                StartCoroutine(_playerUI.HeavyButtonFiller());
            }
            
            if (context.canceled)
            {
                StopCoroutine(_playerUI.HeavyButtonFiller());
                _playerUI.heavyAttFiller.fillAmount = 0f;
            }
        }
        
        public void DoSkill1(InputAction.CallbackContext context)
        {
            _playerUI.Skill1Pressed();
            if (!context.performed || !(stamina >= 30f) || !canExecute) return;
            _animator.SetTrigger(Skill1);
            _playerDamage.DoSkill1();
            CalculateStamina(0f,30f);
        }
        
        public void DoSkill2(InputAction.CallbackContext context)
        {
            _playerUI.Skill2Pressed();
            if (!context.performed || !(stamina >= 30f) || !canExecute) return;
            _animator.SetTrigger(Skill2);
            _playerDamage.DoSkill2();
            CalculateStamina(0f,30f);
        }
        
        public void DoBandage(InputAction.CallbackContext context)
        {
            _playerUI.BandageBtnPressed();
            if (!context.performed || !(stamina >= 5f) || !canExecute || _playerDamage.bandageCount <= 0 || Time.timeScale == 0) return;
            _animator.SetTrigger(Bandage);
            CalculateStamina(0f, 5f);
            _playerDamage.DoHeal();
            _playerDamage.bandageCount -= 1;
        }
        
        public void Inventory(InputAction.CallbackContext context)
        {
            if (context.performed && canExecute)
            {
                _playerUI.OpenInventory();
            }
        }
        
        public void Interact(InputAction.CallbackContext context)
        {
            switch (context.performed)
            {
                case true when canExecute && canInteractWithMap:
                    _playerUI.OpenMap();
                    break;
                case true when canExecute && canInteractWithCraftingBench:
                    _playerUI.OpenCrafting();
                    break;
            }
        }

        public void LockTarget(InputAction.CallbackContext context)
        {
            switch (context.performed && _playerDamage.activeWeapon.name != "Crossbow(Clone)")
            {
                case true when (!_playerDamage.targetLocked && _monster != null):
                    _playerDamage.targetLocked = true;
                    lockedCamera.gameObject.SetActive(true);
                    freeLookCamera.gameObject.SetActive(false);
                    targetGroup.m_Targets[1].target = _monster.transform;
                    break;
                case true when _playerDamage.targetLocked:
                    _playerDamage.targetLocked = false;
                    lockedCamera.gameObject.SetActive(false);
                    freeLookCamera.gameObject.SetActive(true);
                    break;
            }
        }
        
        public void Pause(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            _playerUI.Pause();
        }
        
        #endregion
        
        //Enable/Disable
        #region Enable/Disable

        private void OnEnable()
        {
            Cursor.lockState = CursorLockMode.Locked;
            _playerInput.Enable();
        }
        
        private void OnDisable()
        {
            Cursor.lockState = CursorLockMode.None;
            _playerInput.Disable();
        }

        #endregion
        
        //calculate Stamina
        public void CalculateStamina(float drainage, float hit)
        {
            stamina -= drainage * Time.deltaTime;
            stamina -= hit;

            if (stamina < 0f)
            {
                stamina = 0f;
            }
        }
        
        //HandleAnimations
        #region AnimationEvents

        public void AnimControllerChange(int value)
        {
            _animator.runtimeAnimatorController = animatorController[value];
        }
        
        public void AnimationStarted()
        {
            canExecute = false;
        }
        
        public void AnimationEnded()
        {
            canExecute = true;
        }

        public void CollisionEnabled()
        {
            _playerDamage.activeWeapon.GetComponent<BoxCollider>().enabled = true;
        }
        
        public void CollisionDisabled()
        {
            _playerDamage.activeWeapon.GetComponent<BoxCollider>().enabled = false;
        }
        
        #endregion

        //aiming camera switch
        #region AimingCamera On/Off
        public void AimingCameraOn()
        {
            _playerDamage.targetLocked = false;
            lockedCamera.gameObject.SetActive(false);
            freeLookCamera.gameObject.SetActive(false);
            aimingCamera.gameObject.SetActive(true);
            _playerUI.ShowReticle();
        }
        
        public void AimingCameraOff()
        {
            _playerDamage.targetLocked = false;
            lockedCamera.gameObject.SetActive(false);
            freeLookCamera.gameObject.SetActive(true);
            aimingCamera.gameObject.SetActive(false);
            _playerUI.HideReticle();
        }
        #endregion
        
        private void PerformComboAttack()
        {
            switch (_currentComboState)
            {
                case ComboState.None:
                    _currentComboState = ComboState.Attack1; 
                    _animator.SetTrigger(LightAtt);
                    break;

                case ComboState.Attack1:
                    _currentComboState = ComboState.Attack2;
                    _animator.SetTrigger(LightAtt2);
                    break;

                case ComboState.Attack2:
                    _currentComboState = ComboState.Attack3;
                    _animator.SetTrigger(LightAtt3);
                    break;

                case ComboState.Attack3:
                    // Player has reached the end of the combo chain, reset to None
                    //_currentComboState = ComboState.None;
                    // Loop back to the first attack
                    _currentComboState = ComboState.Attack1;
                    _animator.SetTrigger(LightAtt);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            _lastComboTime = Time.time;
            _canPerformCombo = true;
        }

        public void PlayerDied()
        {
            _animator.SetTrigger(Died);
            _playerUI.DeathScreen();
            Debug.Log("Player Dead");
            _playerInput.Disable(); //not working??
        }
        
        public void PlayerHit()
        {
            _animator.StopPlayback();
            _animator.SetTrigger(TakeHit);
            Debug.Log("player got hit");
        }

        public void PlayerWin()
        {
            _animator.SetTrigger(Win);
        }

        public IEnumerator FallBack()
        {
            Debug.Log("pushed back");
            _animator.StopPlayback();
            
            // Apply pushback.
            canExecute = false;
            canMove = false;
            _animator.SetTrigger(Back);

            // Wait for the animation interval.
            yield return new WaitForSeconds(3f);

            canExecute = true;
            canMove = true;
        }

        //teleporting
        #region Teleport
        private string _sceneToBeLoaded;
        private Vector3 _locationToBeTeleported;
        private static readonly int Win = Animator.StringToHash("Win");

        private void HubTeleport()
        {
            _sceneToBeLoaded = "S_Hub";
            _locationToBeTeleported = new Vector3(23, 6, 7);
            StartCoroutine(LoadScene());
        }
        
        private void HubTeleportNew()
        {
            _sceneToBeLoaded = "S_HubNew";
            _locationToBeTeleported = new Vector3(23, 6, 7);
            StartCoroutine(LoadScene());
        }
        
        public void Arena1Teleport()
        {
            _sceneToBeLoaded = "S_Area11";
            _locationToBeTeleported = new Vector3(30, 20, 10);
            StartCoroutine(LoadScene());
        }
        
        public void Arena2Teleport()
        {
            _sceneToBeLoaded = "S_Area2";
            _locationToBeTeleported = new Vector3(30, 3, 10);
            StartCoroutine(LoadScene());
        }
        
        public void Arena3Teleport()
        {
            _sceneToBeLoaded = "S_Area3";
            _locationToBeTeleported = new Vector3(30, 20, 10);
            StartCoroutine(LoadScene());
        }

        private IEnumerator LoadScene()
        {
            // Set the current Scene to be able to unload it later
            var currentScene = SceneManager.GetActiveScene();

            // The Application loads the Scene in the background at the same time as the current Scene.
            var asyncLoad = SceneManager.LoadSceneAsync(_sceneToBeLoaded, LoadSceneMode.Additive);

            // Wait until the last operation fully loads to return anything
            while (!asyncLoad.isDone)
            {
                _playerUI.LoadingScreenOn();
                yield return null;
            }

            // Move the Player to the newly loaded Scene
            SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetSceneByName(_sceneToBeLoaded));
            transform.position = _locationToBeTeleported;
            _playerUI.CloseMap();
            _playerUI.HideInteract();
            canInteractWithMap = false;
            _monster = GameObject.FindGameObjectWithTag("Monster");

            // Unload the previous Scene
            SceneManager.UnloadSceneAsync(currentScene);
            
            _playerUI.LoadingScreenOff();
        }

        public void TriggerTeleportBack()
        {
            StartCoroutine(TeleportBack());
        }

        private IEnumerator TeleportBack()
        {
            var elapsedTime = 0f;

            while (elapsedTime < 30f)
            {
                _playerUI.timer -= 1;
                
                // Wait for the tick interval.
                yield return new WaitForSeconds(1f);
                elapsedTime += 1f;
            }

            // Teleport player to hub
            if (SceneManager.GetActiveScene().buildIndex == 1)
            {
                HubTeleport();
            }
            else
            {
                HubTeleportNew();
            }
            _playerUI.timer = 30;
            _playerDamage.currentHealth = 300;
            _playerDamage.maxHealth = 300;
            _playerDamage.bandageCount = 5;
        }
        
        #endregion
    }
}
