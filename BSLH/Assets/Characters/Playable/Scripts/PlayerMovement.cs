using UnityEngine;
using UnityEngine.InputSystem;

namespace Characters.Playable.Scripts
{ 
    public class PlayerMovement : MonoBehaviour
    {
        private Animator _animator;
        private CharacterController _controller;
        private Transform _cameraMain;
        private PlayerInput _playerInput;
        private PlayerDamage _playerDamage;

        [SerializeField] private float cameraRotationSpeed = 3f;
        private Vector2 _playerMovement;
        private Quaternion _playerRotation;
        private bool _groundedPlayer;

        private bool _isSprinting;
        private bool _isBlocking;

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
        private static readonly int CanExecute = Animator.StringToHash("CanExecute");

        private void Awake()
        {
            _playerInput = new PlayerInput();

            _playerDamage = GetComponent<PlayerDamage>();
            _animator = GetComponent<Animator>();
            _controller = GetComponent<CharacterController>();
            if (Camera.main != null) _cameraMain = Camera.main.transform;
        }

        private void Start()
        {
            _animator.SetBool(CanExecute, true);
        }

        private void Update()
        {
            #region Grounded
            _groundedPlayer = _controller.isGrounded;
            #endregion

            //movement && sprinting
            switch (_isSprinting)
            {
                case false:
                    _animator.SetFloat(Horizontal, _playerMovement.x);
                    _animator.SetFloat(Vertical, _playerMovement.y);
                    break;
                case true:
                    _animator.SetFloat(Horizontal, _playerMovement.x * 2f);
                    _animator.SetFloat(Vertical, _playerMovement.y * 2f);
                    break;
            }
            
            //rotation
            if (_playerMovement != Vector2.zero)
            {
                _playerRotation = Quaternion.Euler(0f, _cameraMain.eulerAngles.y, 0f);
                transform.rotation = Quaternion.Lerp(transform.rotation, _playerRotation, Time.deltaTime * cameraRotationSpeed);
            }

            //blocking
            switch (_isBlocking)
            {
                case true:
                    _animator.SetBool(IsBlocking, true);
                    break;
                case false:
                    _animator.SetBool(IsBlocking, false);
                    break;
            }
        }


        //action triggers
        #region Action Triggers
        
        public void PlayerMove(InputAction.CallbackContext context)
        {
            _playerMovement = context.ReadValue<Vector2>();
        }
        
        public void DoJump(InputAction.CallbackContext context)
        {
            if (!context.performed || !_groundedPlayer) return;
            _animator.SetTrigger(Jump);
        }

        public void PlayerSprint(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                _isSprinting = true;
            } 
            else if (context.canceled)
            {
                _isSprinting = false;
            }
        }
        
        public void PlayerBlocking(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                _isBlocking = true;
            } 
            else if (context.canceled)
            {
                _isBlocking = false;
            }
        }

        public void DoDodge(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            _animator.SetTrigger(Dodge);
        }
        
        public void DoLightAttack(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            _animator.SetTrigger(LightAtt);
            _playerDamage.DoNormalAttack();
        }
        
        public void DoHeavyAttack(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            _animator.SetTrigger(HeavyAtt);
            _playerDamage.DoHeavyAttack();
        }
        
        public void DoSkill1(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            _animator.SetTrigger(Skill1);
            _playerDamage.DoSkill1();
        }
        
        public void DoSkill2(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            _animator.SetTrigger(Skill2);
            _playerDamage.DoSkill2();
        }
        
        public void DoBandage(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            _animator.SetTrigger(Bandage);
        }
        
        public void Inventory(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            Debug.Log("Inventory Opened");
        }
        
        public void Interact(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            Debug.Log("Interacted");
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
        
        //HandleAnimations
        public void AnimationStarted()
        {
            _animator.SetBool(CanExecute, false);
            _playerDamage.activeWeapon.GetComponent<BoxCollider>().enabled = true;
        }
        
        public void AnimationEnded()
        {
            _animator.SetBool(CanExecute, true);
            _playerDamage.activeWeapon.GetComponent<BoxCollider>().enabled = false;
        }
    }
}
