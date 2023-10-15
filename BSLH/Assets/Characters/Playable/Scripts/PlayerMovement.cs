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

        private Vector3 _playerVelocity;
        private Vector2 _playerMovement;
        private bool _groundedPlayer;
        [SerializeField]private float playerSpeed = 2.0f;
        [SerializeField]private float sprintSpeed = 1.0f;
        [SerializeField]private float jumpHeight = 1.0f;
        [SerializeField]private float gravityValue = -9.81f;
        [SerializeField] private float rotationSpeed = 4f;

        private bool _isJumping;
        private bool _isSprinting;
        private bool _isBlocking;

        //animation cache
        private static readonly int IsWalking = Animator.StringToHash("IsWalking");
        private static readonly int IsSprinting = Animator.StringToHash("IsSprinting");
        private static readonly int Jump = Animator.StringToHash("Jump");
        private static readonly int IsBlocking = Animator.StringToHash("IsBlocking");
        private static readonly int Dodge = Animator.StringToHash("Dodge");
        private static readonly int LightAtt = Animator.StringToHash("LightAtt");
        private static readonly int HeavyAtt = Animator.StringToHash("HeavyAtt");
        private static readonly int Skill1 = Animator.StringToHash("Skill1");
        private static readonly int Skill2 = Animator.StringToHash("Skill2");
        private static readonly int Bandage = Animator.StringToHash("Bandage");

        private void Awake()
        {
            _playerInput = new PlayerInput();
            
            _animator = GetComponent<Animator>();
            _controller = GetComponent<CharacterController>();
            if (Camera.main != null) _cameraMain = Camera.main.transform;
        }
        
        private void Update()
        {
            #region Grounded
            _groundedPlayer = _controller.isGrounded;
            if (_groundedPlayer && _playerVelocity.y < 0)
            {
                _playerVelocity.y = 0f;
            }
            #endregion

            //movement
            var move = new Vector3(_playerMovement.x, 0, _playerMovement.y);
            var cameraTransform = _cameraMain.transform;
            move = cameraTransform.forward * move.z + cameraTransform.right * move.x;
            move.y = 0;
            
            _controller.Move(move * (Time.deltaTime * (playerSpeed + sprintSpeed)));
            
            //jumping
            if (_isJumping && _groundedPlayer)
            {
                _playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
                _isJumping = false;
            }

            _playerVelocity.y += gravityValue * Time.deltaTime;
            _controller.Move(_playerVelocity * Time.deltaTime);

            //rotation
            if (_playerMovement != Vector2.zero)
            {
                var targetAngle = Mathf.Atan2(_playerMovement.x, _playerMovement.y) * Mathf.Rad2Deg + _cameraMain.eulerAngles.y;
                var rotation = Quaternion.Euler(0f, targetAngle, 0f);
                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
            }

            //play move animation when moving
            _animator.SetBool(IsWalking, move.magnitude > 0.1);
        }
        

        public void PlayerMove(InputAction.CallbackContext context)
        {
            _playerMovement = context.ReadValue<Vector2>();
        }
        
        public void DoJump(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            Debug.Log("jump performed");
            _isJumping = true;
            _animator.SetTrigger(Jump);
        }

        public void PlayerSprint(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                Debug.Log("Sprinting");
                _isSprinting = true;
                sprintSpeed = 5f;
                _animator.SetBool(IsSprinting, true);
            } 
            else if (context.canceled)
            {
                _isSprinting = false;
                sprintSpeed = 1f;
                _animator.SetBool(IsSprinting, false);

            }
        }
        
        //blocking needs fixing
        public void PlayerBlocking(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                Debug.Log("Blocking");
                _isBlocking = true;
                _animator.SetBool(IsBlocking, true);
            } 
            else if (context.canceled)
            {
                _isBlocking = false;
                _animator.SetBool(IsBlocking, false);
            }
        }

        public void DoDodge(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            Debug.Log("Dodging");
            _animator.SetTrigger(Dodge);
        }
        
        public void DoLightAttack(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            Debug.Log("Light Attack");
            _animator.SetTrigger(LightAtt);
        }
        
        public void DoHeavyAttack(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            Debug.Log("Heavy Attack");
            _animator.SetTrigger(HeavyAtt);
        }
        
        public void DoSkill1(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            Debug.Log("Skill 1");
            _animator.SetTrigger(Skill1);
        }
        
        public void DoSkill2(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            Debug.Log("Skill 2");
            _animator.SetTrigger(Skill2);
        }
        
        public void DoBandage(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            Debug.Log("Bandaging");
            _animator.SetTrigger(Bandage);
        }
        
        public void Inventory(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            Debug.Log("Opening Inventory");
        }
        
        public void Interact(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            Debug.Log("Interaction");
        }
        
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
    }
}
