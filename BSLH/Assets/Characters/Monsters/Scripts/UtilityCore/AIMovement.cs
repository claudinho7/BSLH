using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Characters.Monsters.Scripts.UtilityCore
{
    public class AIMovement : MonoBehaviour
    {
        private NavMeshAgent _agent;
        public Transform playerTransform; // Reference to the player's Transform component
        private Animator _animator;

        //Vision
        private const float ProximityDistance = 15f;
        private Vector3 _directionToPlayer;
        private Vector3 _playerPosition;
        public bool canHitMelee;
        public bool canHitRanged;
        public float distanceToPlayer;
        
        private bool _playerFound;
        
        //for random patrol
        private const float PatrolRange = 15f;
        private Transform _centerPoint;
        public bool isPatrolling; // Track whether patrolling is active.
        
        //animation cache
        private static readonly int Speed = Animator.StringToHash("Speed");
        
        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _animator = GetComponent<Animator>();
            
            // Check if _playerDamage is still null and the player is not yet found.
            if (playerTransform == null && !_playerFound)
            {
                StartCoroutine(FindPlayer());
            }
            else
            {
                // PlayerDamage is already set, or player search is in progress.
                if (playerTransform == null)
                {
                    Debug.Log("PlayerTransform not set; still searching for the player...");
                }
                else
                {
                    Debug.Log("PlayerTransform already set.");
                }
            }
        }
        
        private void Start()
        {
            _centerPoint = this.transform;
            isPatrolling = true;
        }

        private void Update()
        {
            _animator.SetFloat(Speed, _agent.velocity.magnitude); //if agent moving -> use walk / speed animation
            
            // Check if patrolling is active.
            if (isPatrolling && _agent.remainingDistance <= _agent.stoppingDistance)
            {
                Patrol();
            }

            #region AI Vision
            // Calculate the direction from AI to player
            var position = transform.position;
            if (playerTransform != null)
            {
                _playerPosition = playerTransform.position;
            }
            else
            {
                Debug.LogWarning("Player Transform Not Found");
                _playerPosition = new Vector3(500, 500, 500);
            }
            
            _directionToPlayer = _playerPosition - position;
            distanceToPlayer = Vector3.Distance(position, _playerPosition); //set distance to player

            // Create a ray from AI's position towards the player
            var ray = new Ray(position, _directionToPlayer);

            // Draw the ray in the scene view
            Debug.DrawRay(position, _directionToPlayer, Color.red);

            // Perform the raycast
            if (Physics.Raycast(ray, out var hit, ProximityDistance))
            {
                // Check if the hit object is the player
                if (hit.collider.CompareTag("Player"))
                {
                    // There is nothing blocking the line of sight to the player
                    canHitMelee = true;
                    canHitRanged = true;
                    isPatrolling = false; //stop patrolling
                    //Debug.Log("Player is in line of sight!");
                }
                else
                {
                    // Something else is in between the AI and the player
                    canHitRanged = false;
                    isPatrolling = false; //stop patrolling
                    //Debug.Log("Something is blocking the line of sight.");
                    canHitMelee = distanceToPlayer < 3f; //can hit melee even if not in LOS within 3f
                }
            }
            else
            {
                // The raycast did not hit anything
                //Debug.Log("No objects in sight.");
                canHitMelee = false;
                canHitRanged = false;
            }
            #endregion
        }

        public void LookAtPlayer()
        {
            if (_directionToPlayer == Vector3.zero) return;
            _directionToPlayer.y = 0f; // Ensure the AI remains level (no tilting)
            // Set the AI's rotation
            _agent.transform.forward = _directionToPlayer;
        }

        public void MoveInMelee()
        {
            _agent.SetDestination(playerTransform.position);
        }

        public void MoveInRanged()
        {
            // Calculate the direction from AI to player.
            var position = playerTransform.position;
            var directionToPlayer = transform.position - position;
            directionToPlayer.Normalize();
            // Calculate the destination 10 units away from the player.
            var destination = position + directionToPlayer * 10f;
            // Set the calculated destination for the NavMesh agent.
            _agent.SetDestination(destination);
        }

        //Patrolling
        #region Patrolling

        private void Patrol()
        {
            if (!RandomPoint(_centerPoint.position, PatrolRange, out var point)) return;
            Debug.DrawRay(point, Vector3.up, Color.blue, 1f);
            _agent.SetDestination(point);
        }

        //chose random point in mesh
        private static bool RandomPoint(Vector3 center, float range, out Vector3 result)
        {
            var randomPoint = center + Random.insideUnitSphere * range;

            if (NavMesh.SamplePosition(randomPoint, out var hit, 1f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }

            result = Vector3.zero;
            return false;
        }

        #endregion
        
        private IEnumerator FindPlayer()
        {
            while (!_playerFound)
            {
                var playerObject = GameObject.FindGameObjectWithTag("Player");

                if (playerObject != null)
                {
                    playerTransform = playerObject.transform;

                    if (playerTransform != null)
                    {
                        _playerFound = true; // Exit the loop when the player and script are found.
                    }
                    else
                    {
                        Debug.LogWarning("PlayerTransform not found on the 'Player' GameObject. Retrying...");
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
