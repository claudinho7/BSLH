using System.Collections;
using UnityEngine;

namespace OtherScripts
{
    public class TractorScript : MonoBehaviour
    {
        private Animator _animator;

        public bool canMove;
        
        //anime cache
        private static readonly int MoveAway = Animator.StringToHash("MoveAway");
        private static readonly int MoveBack = Animator.StringToHash("MoveBack");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            if (canMove)
            {
                StartCoroutine(MoveTractor());
            }
        }

        private IEnumerator MoveTractor()
        {
            canMove = false;
            _animator.SetTrigger(MoveAway);

            yield return new WaitForSeconds(10f);
            
            _animator.SetTrigger(MoveBack);
            
            yield return new WaitForSeconds(5f);

            canMove = true;
        }
    }
}
