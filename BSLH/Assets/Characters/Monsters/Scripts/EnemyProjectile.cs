using System.Collections;
using UnityEngine;

namespace Characters.Monsters.Scripts
{
    public class EnemyProjectile : MonoBehaviour
    {
        public bool isAoe;
        private Vector3 _target;
        public GameObject splashVFX;
        [SerializeField] private AudioClip hitSound;
        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            var objects = GameObject.FindGameObjectWithTag("Player");
            _target = objects.transform.position + new Vector3(0, 1, 0);

            if (!isAoe)
            {
                StartCoroutine(LifeTime());
            }
            else
            {
                transform.position = objects.transform.position;
            }
        }

        private void Update()
        {
            if (!isAoe)
            {
                transform.position = Vector3.MoveTowards(transform.position, _target, 20 * Time.deltaTime);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!isAoe)
            {
                var newVfx = Instantiate(splashVFX, transform.parent);
                newVfx.transform.position = transform.position;
                
                _audioSource.PlayOneShot(hitSound);
                
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
