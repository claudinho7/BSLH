using System.Collections;
using UnityEngine;

namespace Characters.Monsters.Scripts
{
    public class EnemyProjectile : MonoBehaviour
    {
        private Vector3 _target;
        public GameObject splashVFX;
        private void Start()
        {
            var objects = GameObject.FindGameObjectWithTag("Player");
            _target = objects.transform.position + new Vector3(0, 1, 0);

            StartCoroutine(LifeTime());
        }

        private void Update()
        {
            transform.position = Vector3.MoveTowards(transform.position, _target, 20 * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                var newVfx = Instantiate(splashVFX, transform.parent);
                newVfx.transform.position = transform.position;
                
                Destroy(gameObject);
            }
        }

        private IEnumerator LifeTime()
        {
            var time = 0;

            while (time <= 5)
            {
                time += 1;
                yield return new WaitForSeconds(1);
            }
            
            Destroy(gameObject);
        }
    }
}
