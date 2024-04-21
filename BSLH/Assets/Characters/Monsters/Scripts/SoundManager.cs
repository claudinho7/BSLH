using UnityEngine;
using UnityEngine.AI;

namespace Characters.Monsters.Scripts
{
    public class SoundManager : MonoBehaviour
    {
        [Header("Sound")]
        private AudioSource _audioSource;
        public AudioClip[] sfxGorgon;
        public AudioClip[] sfxSatyr;
        public AudioClip[] sfxGargoyle;
        
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }
        

        public void PlayDead()
        {
            _audioSource.clip = gameObject.name switch
            {
                "Gorgon" => sfxGorgon[0],
                "Satyr" => sfxSatyr[0],
                "Gargoyle" => sfxGargoyle[0],
                _ => _audioSource.clip
            };
            _audioSource.Play();
        }
        
        public void PlayMeleeBasic()
        {
            _audioSource.clip = gameObject.name switch
            {
                "Gorgon" => sfxGorgon[1],
                "Satyr" => sfxSatyr[1],
                "Gargoyle" => sfxGargoyle[1],
                _ => _audioSource.clip
            };
            _audioSource.Play();
        }
        
        public void PlayMeleeSpecial()
        {
            _audioSource.clip = gameObject.name switch
            {
                "Gorgon" => sfxGorgon[2],
                "Satyr" => sfxSatyr[2],
                "Gargoyle" => sfxGargoyle[2],
                _ => _audioSource.clip
            };
            _audioSource.Play();
        }
        
        public void PlayRangedBasic()
        {
            _audioSource.clip = gameObject.name switch
            {
                "Gorgon" => sfxGorgon[3],
                "Satyr" => sfxSatyr[3],
                "Gargoyle" => sfxGargoyle[3],
                _ => _audioSource.clip
            };
            _audioSource.Play();
        }
        
        public void PlayRangedSpecial()
        {
            _audioSource.clip = gameObject.name switch
            {
                "Gorgon" => sfxGorgon[4],
                "Satyr" => sfxSatyr[4],
                "Gargoyle" => sfxGargoyle[4],
                _ => _audioSource.clip
            };
            _audioSource.Play();
        }
        
        public void PlayUltimate()
        {
            _audioSource.clip = gameObject.name switch
            {
                "Gorgon" => sfxGorgon[5],
                "Satyr" => sfxSatyr[5],
                "Gargoyle" => sfxGargoyle[5],
                _ => _audioSource.clip
            };
            _audioSource.Play();
        }
    }
}
