using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Characters.Playable.Scripts
{ 
    public class PlayerMovement : MonoBehaviour
    {
        private Animator _animator;
        private CharacterController _controller;
        private Transform _cameraMain;
        private PlayerInput _playerInput;
        private PlayerDamage _playerDamage;
        private PlayerUI _playerUI;
        private GameObject _monster;

        [SerializeField] private float cameraRotationSpeed = 3f;
        public GameObject freeLookCamera;
        public GameObject lockedCamera;
        public CinemachineTargetGroup targetGroup;
        private Vector2 _playerMovement;
        private Quaternion _playerRotation;
        private bool _groundedPlayer;
        private bool _inventoryOpened;
        
        private Vector2 _targetMovementVector = Vector2.zero;
        [SerializeField][Range(0.001f, 2f)]
        private float movementInputSmoothTime;

        //stats
        public float stamina = 100f;
        [SerializeField] private float staminaRechargeRate;
        private bool _isSprinting;
        public bool isBlocking;
        public bool canExecute;
        public bool canMove;
        public bool canInteractWithMap;
        public bool canInteractWithCraftingBench;

        //animation cache
        private static readonly int Jump = Animator.StringToHash("Jump");
        private static readonly int IsBlocking = Animator.StringToHash("IsBlocking");
        private static readonly int Dodge = Animator.StringToHash("Dodge");
        private static readonly int LightAtt = Animator.StringToHash("LightAtt");
        private static readonly int HeavyAtt = Animator.StringToHash("HeavyAtt");
        private static readonly int Skill1 = Animator.StringToHash("Skill1");
        private static readonly int Skill2 = Animator.StringToHash("Skill2");
        private static readonly int Bandage = Animator.StringToHash("Bandage");
        private static readonly int Horizontal = Animator.StringToHash("Horizontal");
        private static readonly int Vertical = Animator.StringToHash("Vertical");
        private static readonly int Died = Animator.StringToHash("Died");
        private static readonly int Back = Animator.StringToHash("FallBack");

        private void Awake()
        {
            _playerInput = new PlayerInput();
            _playerDamage = GetComponent<PlayerDamage>();
            _animator = GetComponent<Animator>();
            _controller = GetComponent<CharacterController>();
            _playerUI = GetComponent<PlayerUI>();

            if (Camera.main != null) _cameraMain = Camera.main.transform;

            canExecute = true;
            canMove = true;
        }

        /// <summary>
        /// Reference field for SmoothDamp of movement input
        /// </summary>
        private Vector2 _movementSmoothVelocity = Vector2.zero;
        
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
            switch (isBlocking)
            {
                case true:
                    _animator.SetBool(IsBlocking, true);
                    CalculateStamina(5f, 0f);
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
                //look at monster
                // Get direction from monster
                var targetDirection = _monster.transform.position - transform.position; 
                // Calculate the Y rotation based on the direction to the target.
                var targetYRotation = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;
                
                _playerRotation = Quaternion.Euler(0f, targetYRotation, 0f);
                transform.rotation = Quaternion.Lerp(transform.rotation, _playerRotation,
                    Time.deltaTime * cameraRotationSpeed);
            }
            else
            {
                //rotate where camera is looking
                if (_playerMovement == Vector2.zero) return;
                _playerRotation = Quaternion.Euler(0f, _cameraMain.eulerAngles.y, 0f);
                transform.rotation = Quaternion.Lerp(transform.rotation, _playerRotation,
                    Time.deltaTime * cameraRotationSpeed);
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
            if (context.started && stamina >= 5f && canExecute)
            {
                isBlocking = true;
            } 
            else if (context.canceled)
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
            if (!context.performed || !(stamina >= 10f) || !canExecute) return;
            _animator.SetTrigger(LightAtt);
            _playerDamage.DoNormalAttack();
            CalculateStamina(0f,10f);
        }
        
        public void DoHeavyAttack(InputAction.CallbackContext context)
        {
            if (!context.performed || !(stamina >= 20f) || !canExecute) return;
            _animator.SetTrigger(HeavyAtt);
            _playerDamage.DoHeavyAttack();
            CalculateStamina(0f,20f);
        }
        
        public void DoSkill1(InputAction.CallbackContext context)
        {
            if (!context.performed || !(stamina >= 20f) || !canExecute) return;
            _animator.SetTrigger(Skill1);
            _playerDamage.DoSkill1();
            CalculateStamina(0f,20f);
        }
        
        public void DoSkill2(InputAction.CallbackContext context)
        {
            if (!context.performed || !(stamina >= 20f) || !canExecute) return;
            _animator.SetTrigger(Skill2);
            _playerDamage.DoSkill2();
            CalculateStamina(0f,20f);
        }
        
        public void DoBandage(InputAction.CallbackContext context)
        {
            if (!context.performed || !(stamina >= 5f) || !canExecute || _playerDamage.bandageCount <= 0) return;
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
            switch (context.performed)
            {
                case true when (!_playerDamage.targetLocked && _monster != null):
                    _playerDamage.targetLocked = true;
                    lockedCamera.gameObject.SetActive(true);
                    freeLookCamera.gameObject.SetActive(false);
                    targetGroup.m_Targets[1].target = _monster.transform;
                    break;
                case true when (_playerDamage.targetLocked && _monster != null):
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

        public void PlayerDied()
        {
            _animator.SetTrigger(Died);
            _playerUI.DeathScreen();
            Debug.Log("Player Dead");
            _playerInput.Disable(); //not working??
        }

        public IEnumerator FallBack()
        {
            Debug.Log("pushed back");

            // Apply pushback.
            canExecute = false;
            canMove = false;
            _animator.SetTrigger(Back);

            // Wait for the animation interval.
            yield return new WaitForSeconds(3.3f);

            canExecute = true;
            canMove = true;
        }

        //teleporting
        #region Teleport
        private string _sceneToBeLoaded;
        private Vector3 _locationToBeTeleported;

        public void HubTeleport()
        {
            _sceneToBeLoaded = "S_Hub";
            _locationToBeTeleported = new Vector3(23, 6, 7);
            StartCoroutine(LoadScene());
        }
        
        public void Arena1Teleport()
        {
            _sceneToBeLoaded = "S_Area1";
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
        
        public void Arena4Teleport()
        {
            _sceneToBeLoaded = "S_Arena4";
            _locationToBeTeleported = new Vector3(20, 0, 20);
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
            _playerUI.LoadingScreenOff();
            
            // Unload the previous Scene
            SceneManager.UnloadSceneAsync(currentScene);
        }

        #endregion
    }
}
