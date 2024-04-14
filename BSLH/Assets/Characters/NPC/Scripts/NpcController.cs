using System.Collections;
using Characters.Playable.Scripts;
using OtherScripts;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Characters.NPC.Scripts
{
    public class NpcController : MonoBehaviour
    {
        private Animator _animator;
        public PlayerMovement playerMovement;
        public TractorScript tractorScript;

        public float speed;
        private bool _playerInRange;

        public bool getUpActive;
        public bool wavingActive;
        public bool sittingActive;

        private Transform _playerLocation;
        public Transform chairLocation;
        public Transform mapLocation;
        public Transform spaceLocation;
        public Transform itemsLocation;
        public Transform hideLocation;
        public Transform caveLocation;

        private AudioSource _audioSource;
        public AudioClip[] npcVoice;

        //anim cache
        private static readonly int GetUp = Animator.StringToHash("GetUp");
        private static readonly int Movement = Animator.StringToHash("Movement");
        private static readonly int Tutorial1 = Animator.StringToHash("Tutorial1");
        private static readonly int Tutorial2 = Animator.StringToHash("Tutorial2");
        private static readonly int Fallow = Animator.StringToHash("Fallow");
        private static readonly int Blacksmith = Animator.StringToHash("Blacksmith");
        private static readonly int Map = Animator.StringToHash("Map");
        private static readonly int Wave = Animator.StringToHash("Wave");
        private static readonly int PickUp = Animator.StringToHash("PickUp");
        private static readonly int Point = Animator.StringToHash("Point");
        private static readonly int Sitting = Animator.StringToHash("Sitting");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            if (sittingActive)
            {
                PlaySit();
            }
            else if (wavingActive)
            {
                StartCoroutine(HubTutorial());
            } 
            else if (getUpActive)
            {
                StartCoroutine(StartTutorial());
            }
            
            _playerLocation = playerMovement.transform;
        }

        //play anims
        #region PlayAnims
        public void StartEnding()
        {
            StartCoroutine(Ending());
        }
        
        private void PlayGetUp()
        {
            _animator.SetTrigger(GetUp);
            _audioSource.clip = npcVoice[0];
            _audioSource.Play();
        }
        private void PlayTutorial1()
        {
            _animator.SetTrigger(Tutorial1);
            _audioSource.clip = npcVoice[1];
            _audioSource.Play();
        }
        private void PlayPickUp()
        {
            _animator.SetTrigger(PickUp);
            _audioSource.clip = npcVoice[2];
            _audioSource.Play();
        }
        private void PlayTutorial2()
        {
            _animator.SetTrigger(Tutorial2);
            _audioSource.clip = npcVoice[3];
            _audioSource.Play();
        }
        private void PlayPoint()
        {
            _animator.SetTrigger(Point);
            _audioSource.clip = npcVoice[4];
            _audioSource.Play();
        }
        private void PlayEnding()
        {
            _animator.SetTrigger(Fallow);
            _audioSource.clip = npcVoice[5];
            _audioSource.Play();
        }

        private void PlayWave()
        {
            _animator.SetBool(Wave, true);
            _audioSource.clip = npcVoice[6];
            _audioSource.Play();
        }
        private void PlayBlacksmith()
        {
            _animator.SetTrigger(Blacksmith);
            _audioSource.clip = npcVoice[7];
            _audioSource.Play();
        }
        private void PlayMap()
        {
            _animator.SetTrigger(Map);
            _audioSource.clip = npcVoice[8];
            _audioSource.Play();
        }
        private void PlaySit()
        {
            _animator.SetBool(Sitting, true);
        }
        #endregion

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            _playerInRange = true;
            
            if (!sittingActive) return;
            _audioSource.clip = npcVoice[9];
            _audioSource.Play();
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            _playerInRange = false;
        }

        private IEnumerator HubTutorial()
        {
            while (!_playerInRange)
            {
                PlayWave();

                yield return new WaitForSeconds(5f);
            }
            wavingActive = false;
            _animator.SetBool(Wave, false);
            
            PlayBlacksmith();
            
            yield return new WaitForSeconds(27f);
            
            //start moving to map
            _animator.SetBool(Movement, true);
            while (Vector3.Distance(transform.position, mapLocation.position) > 0.1f)
            {
                // Calculate the direction towards the chair
                var transform1 = transform;
                var position = transform1.position;
                var direction = (mapLocation.position - position).normalized;

                // Move towards the chair
                position += direction * (speed * Time.deltaTime);
                transform1.position = position;

                // Rotate towards the direction of movement
                if (direction != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(direction);
                }

                yield return null; // Wait for the next frame
            }
            _animator.SetBool(Movement, false);
            
            PlayMap();
            
            yield return new WaitForSeconds(23f);

            StartCoroutine(MoveToChair());
        }

        private IEnumerator StartTutorial()
        {
            PlayGetUp();

            yield return new WaitForSeconds(26);
            
            //start moving back for space
            _animator.SetBool(Movement, true);
            while (Vector3.Distance(transform.position, spaceLocation.position) > 0.1f)
            {
                // Calculate the direction towards the chair
                var transform1 = transform;
                var position = transform1.position;
                var direction = (spaceLocation.position - position).normalized;

                // Move towards the chair
                position += direction * (speed * Time.deltaTime);
                transform1.position = position;

                // Rotate towards the direction of movement
                if (direction != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(direction);
                }

                yield return null; // Wait for the next frame
            }
            _animator.SetBool(Movement, false);
            
            PlayTutorial1();
            
            yield return new WaitForSeconds(45);
            
            //start moving to items
            _animator.SetBool(Movement, true);
            while (Vector3.Distance(transform.position, itemsLocation.position) > 0.1f)
            {
                // Calculate the direction towards the chair
                var transform1 = transform;
                var position = transform1.position;
                var direction = (itemsLocation.position - position).normalized;

                // Move towards the chair
                position += direction * (speed * Time.deltaTime);
                transform1.position = position;

                // Rotate towards the direction of movement
                if (direction != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(direction);
                }

                yield return null; // Wait for the next frame
            }
            _animator.SetBool(Movement, false);
            
            PlayPickUp();
            
            yield return new WaitForSeconds(15.2f);
            
            //start moving back for space
            _animator.SetBool(Movement, true);
            while (Vector3.Distance(transform.position, spaceLocation.position) > 0.1f)
            {
                // Calculate the direction towards the chair
                var transform1 = transform;
                var position = transform1.position;
                var direction = (spaceLocation.position - position).normalized;

                // Move towards the chair
                position += direction * (speed * Time.deltaTime);
                transform1.position = position;

                // Rotate towards the direction of movement
                if (direction != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(direction);
                }

                yield return null; // Wait for the next frame
            }
            _animator.SetBool(Movement, false);
            
            PlayTutorial2();
            
            yield return new WaitForSeconds(34f);
            
            //start moving back for space
            _animator.SetBool(Movement, true);
            while (Vector3.Distance(transform.position, hideLocation.position) > 0.1f)
            {
                // Calculate the direction towards the chair
                var transform1 = transform;
                var position = transform1.position;
                var direction = (hideLocation.position - position).normalized;

                // Move towards the chair
                position += direction * (speed * Time.deltaTime);
                transform1.position = position;

                // Rotate towards the direction of movement
                if (direction != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(direction);
                }

                yield return null; // Wait for the next frame
            }
            _animator.SetBool(Movement, false);
            
            PlayPoint();
            
            yield return new WaitForSeconds(23);

            tractorScript.canMove = true;
        }

        private IEnumerator MoveToChair()
        {
            _animator.SetBool(Movement, true);
            while (Vector3.Distance(transform.position, chairLocation.position) > 0.1f)
            {
                // Calculate the direction towards the chair
                var transform1 = transform;
                var position = transform1.position;
                var direction = (chairLocation.position - position).normalized;

                // Move towards the chair
                position += direction * (speed * Time.deltaTime);
                transform1.position = position;

                // Rotate towards the direction of movement
                if (direction != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(direction);
                }

                yield return null; // Wait for the next frame
            }
            _animator.SetBool(Movement, false);
            
            transform.rotation = Quaternion.LookRotation(chairLocation.position);
            
            sittingActive = true;
            PlaySit();
        }
        
        private IEnumerator Ending()
        {
            //start moving to player
            _animator.SetBool(Movement, true);
            speed = 6f;
            while (Vector3.Distance(transform.position, _playerLocation.position + Vector3.back) > 0.1f)
            {
                // Calculate the direction towards the player
                var transform1 = transform;
                var position = transform1.position;
                var direction = ((_playerLocation.position + Vector3.back) - position).normalized;

                // Move towards the player
                position += direction * (speed * Time.deltaTime);
                transform1.position = position;

                // Rotate towards the direction of movement
                if (direction != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(direction);
                }

                yield return null; // Wait for the next frame
            }
            _animator.SetBool(Movement, false);
            speed = 0.6f;
            
            //stop player input
            var device = InputSystem.devices[0];
            InputSystem.DisableDevice(device);
            
            transform.LookAt(_playerLocation);
            
            PlayEnding();

            yield return new WaitForSeconds(20f);
            
            //restore player input
            InputSystem.EnableDevice(device);
            
            playerMovement.TriggerTeleportBack();
            
            //start moving to cave
            _animator.SetBool(Movement, true);
            while (Vector3.Distance(transform.position, caveLocation.position) > 0.1f)
            {
                // Calculate the direction towards the player
                var transform1 = transform;
                var position = transform1.position;
                var direction = (caveLocation.position - position).normalized;

                // Move towards the player
                position += direction * (speed * Time.deltaTime);
                transform1.position = position;

                // Rotate towards the direction of movement
                if (direction != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(direction);
                }

                yield return null; // Wait for the next frame
            }
            _animator.SetBool(Movement, false);
        }
    }
}
