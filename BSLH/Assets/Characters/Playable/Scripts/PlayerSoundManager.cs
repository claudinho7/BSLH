using UnityEngine;

namespace Characters.Playable.Scripts
{
    public class PlayerSoundManager : MonoBehaviour
    {
        [SerializeField] private AudioClip[] steps;
        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void Step()
        {
            var clip = GetRandomClip();
            _audioSource.PlayOneShot(clip);
        }

        private AudioClip GetRandomClip()
        {
            return steps[Random.Range(0, steps.Length)];
        }
    }
}
