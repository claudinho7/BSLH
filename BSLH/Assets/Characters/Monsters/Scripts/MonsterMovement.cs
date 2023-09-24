using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Characters.Monsters.Scripts
{ 
    public class MonsterMovement : MonoBehaviour
    {
        public NavMeshAgent agent;
        public Transform playerTransform; // Reference to the player's Transform component

        //for random patrol
        private const float PatrolRange = 15f;
        private Transform _centerPoint;
        private bool _isPatrolling; // Track whether patrolling is active.
        
        private readonly WaitForSeconds _raycastInterval = new(0.5f); // Adjust the interval as needed
        [SerializeField] private float proximityDistance;
        public bool playerSeen;
        public bool canHitMelee;
        public bool canHitRanged;



        private void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            _centerPoint = this.transform;
            
            // Start the coroutine that performs the raycast
            StartCoroutine(RaycastToPlayer());
        }

        public void MoveInMelee()
        {
            agent.stoppingDistance = 3f;
            agent.SetDestination(playerTransform.position);
            Debug.Log("should go to player");
        }

        public void MoveInRanged()
        {
            agent.stoppingDistance = 10f;
            agent.SetDestination(playerTransform.position);
        }

        public void StartPatrolling()
        {
            _isPatrolling = true;
            
            // Check if patrolling is active.
            if (_isPatrolling && agent.remainingDistance <= agent.stoppingDistance)
            {
                Patrol();
            }
        }

        public void StopPatrolling()
        {
            _isPatrolling = false;
            Debug.Log("stopped patrolling");
        }

        
        private void Patrol()
        {
            if (!RandomPoint(_centerPoint.position, PatrolRange, out var point)) return;
            Debug.DrawRay(point, Vector3.up, Color.blue, 1f);
            agent.SetDestination(point);
        }

        private static bool RandomPoint(Vector3 center, float range, out Vector3 result)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;

            if (NavMesh.SamplePosition(randomPoint, out var hit, 1f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }

            result = Vector3.zero;
            return false;
        }

        //check if is moving
        public bool IsMoving => agent.velocity.magnitude > float.Epsilon;
        
        private IEnumerator RaycastToPlayer()
        {
            while (true)
            {
                // Calculate the direction from AI to player
                var position = transform.position;
                var directionToPlayer = playerTransform.position - position;

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
                        Debug.Log("Player is in line of sight!");
                    }
                    else
                    {
                        // Something else is in between the AI and the player
                        playerSeen = true;
                        canHitRanged = false;
                        Debug.Log("Something is blocking the line of sight.");

                        var distance = Vector3.Distance(position, playerTransform.position);

                        if (distance < 3f)
                        {
                            canHitMelee = true;
                        }
                        else
                        {
                            canHitMelee = false;
                        }
                    }
                }
                else
                {
                    // The raycast did not hit anything
                    Debug.Log("No objects in sight.");
                    playerSeen = false;
                    canHitMelee = false;
                    canHitRanged = false;
                }

                yield return _raycastInterval; // Wait for the specified interval before performing the next raycast
            }
        }
    }
}
