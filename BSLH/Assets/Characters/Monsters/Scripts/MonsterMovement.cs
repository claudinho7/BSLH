using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Characters.Monsters.Scripts
{ 
    public class MonsterMovement : MonoBehaviour
    {
        private NavMeshAgent _agent;
        public Transform playerTransform; // Reference to the player's Transform component
        private Animator _animator;

        //for random patrol
        private const float PatrolRange = 15f;
        private Transform _centerPoint;
        private bool _isPatrolling; // Track whether patrolling is active.
        
        //for vision
        [SerializeField] private float proximityDistance;
        public bool playerSeen;
        public bool canHitMelee;
        public bool canHitRanged;
        public bool isInMelee;
        public bool isInRanged;
        public float distanceToPlayer;

        private bool _isMoving;
        
        //animation cache
        private static readonly int Speed = Animator.StringToHash("Speed");


        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            _centerPoint = this.transform;
        }

        private void Update()
        {
            _animator.SetFloat(Speed, _agent.velocity.magnitude); //if agent moving -> use walk / speed animation

            #region AI Vision
            // Calculate the direction from AI to player
            var position = transform.position;
            var playerPosition = playerTransform.position;
            var directionToPlayer = playerPosition - position;
            distanceToPlayer = Vector3.Distance(position, playerPosition);

            // Create a ray from AI's position towards the player
            var ray = new Ray(position, directionToPlayer);

            // Draw the ray in the scene view
            Debug.DrawRay(position, directionToPlayer, Color.red);

            // Perform the raycast
            if (Physics.Raycast(ray, out var hit, proximityDistance))
            {
                // Check if the hit object is the player
                if (hit.collider.CompareTag("Player"))
                {
                    // There is nothing blocking the line of sight to the player
                    playerSeen = true;
                    canHitMelee = true;
                    canHitRanged = true;
                    //Debug.Log("Player is in line of sight!");
                }
                else
                {
                    // Something else is in between the AI and the player
                    canHitRanged = false;
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
            
            switch (distanceToPlayer)
            {
                case <= 4f:
                    isInMelee = true;
                    isInRanged = false;
                    break;
                case >= 9f and <= 11f:
                    isInMelee = false;
                    isInRanged = true;
                    break;
                default:
                    isInMelee = false;
                    isInRanged = false;
                    break;
            }
        }

        public void ResetIsMoving()
        {
            _isMoving = false;
        }
        
        public void MoveInMelee()
        {
            if (_isMoving) return;
            _agent.stoppingDistance = 2f;
            _agent.SetDestination(playerTransform.position);
            //Debug.Log("Moving To Player");
            _isMoving = true;
        }

        public void MoveInRanged()
        {
            if (_isMoving) return;
            _agent.stoppingDistance = 1f;
            // Calculate the direction from AI to player.
            var position = playerTransform.position;
            var directionToPlayer = transform.position - position;
            directionToPlayer.Normalize();
            // Calculate the destination 10 units away from the player.
            var destination = position + directionToPlayer * 10f;
            // Set the calculated destination for the NavMesh agent.
            _agent.SetDestination(destination);
            //Debug.Log("Moving To Ranged Position");
            _isMoving = true;
        }

        public void StartPatrolling()
        {
            // Check if patrolling is active.
            if (!_isPatrolling && _agent.remainingDistance <= _agent.stoppingDistance)
            {
                Patrol();
            }
        }

        public void ResetPatrolling()
        {
            _isPatrolling = true;
            Debug.Log("stopped patrolling");
        }

        
        private void Patrol()
        {
            if (!RandomPoint(_centerPoint.position, PatrolRange, out var point)) return;
            Debug.DrawRay(point, Vector3.up, Color.blue, 1f);
            _agent.SetDestination(point);
        }

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
    }
}
