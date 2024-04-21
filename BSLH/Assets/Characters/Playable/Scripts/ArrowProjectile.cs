using System;
using System.Collections;
using UnityEngine;

namespace Characters.Playable.Scripts
{
    public class ArrowProjectile : MonoBehaviour
    {
        public GameObject hitVFX;
        [SerializeField] private AudioClip hitSound;
        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            StartCoroutine(LifeTime());
        }

        private void OnTriggerEnter(Collider other)
        {
            var newVfx = Instantiate(hitVFX, transform.parent);
            newVfx.transform.position = transform.position;
            
            _audioSource.PlayOneShot(hitSound);
                
            Destroy(gameObject);
        }

        private IEnumerator LifeTime()
        {
            var time = 0;

            while (time <= 15)
            {
                time += 1;
                yield return new WaitForSeconds(1);
            }
            
            Destroy(gameObject);
        }
    }
}
