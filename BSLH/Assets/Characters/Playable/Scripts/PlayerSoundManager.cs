using UnityEngine;

namespace Characters.Playable.Scripts
{
    public class PlayerSoundManager : MonoBehaviour
    {
        [SerializeField] private AudioClip[] steps;
        [SerializeField] private AudioClip[] hits;
        [SerializeField] private AudioClip[] swings;
        [SerializeField] private AudioClip[] characterSounds;
        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        //swings
        #region Swings
        public void SwordSwing()
        {
            _audioSource.PlayOneShot(swings[0]);
            _audioSource.volume = 1f;
        }

        public void HammerSwing()
        {
            _audioSource.PlayOneShot(swings[1]);
            _audioSource.volume = 1f;
        }

        public void BowShoot()
        {
            _audioSource.PlayOneShot(swings[3]);
            _audioSource.volume = 1f;
        }

        public void SpearStab()
        {
            _audioSource.PlayOneShot(swings[2]);
            _audioSource.volume = 1f;
        }
        #endregion
        
        //hits
        #region Hits
        public void PlayHit()
        {
            _audioSource.PlayOneShot(hits[0]);
            _audioSource.volume = 1f;
        }
        #endregion
        
        //player sounds

        #region PlayerSounds
        public void PlayWin()
        {
            _audioSource.PlayOneShot(characterSounds[0]);
            _audioSource.volume = 1f;
        }
        public void PlayWin2()
        {
            _audioSource.PlayOneShot(characterSounds[1]);
            _audioSource.volume = 1f;
        }
        public void PlayWin3()
        {
            _audioSource.PlayOneShot(characterSounds[2]);
            _audioSource.volume = 3f;
        }
        public void PlayWin4()
        {
            _audioSource.PlayOneShot(characterSounds[3]);
            _audioSource.volume = 3f;
        }
        public void PlayTripleStrike()
        {
            _audioSource.PlayOneShot(characterSounds[4]);
            _audioSource.volume = 1f;
        }
        public void PlayTripleStab()
        {
            _audioSource.PlayOneShot(characterSounds[5]);
            _audioSource.volume = 1f;
        }
        public void PlaySwordNormal()
        {
            _audioSource.PlayOneShot(characterSounds[6]);
            _audioSource.volume = 1f;
        }
        public void PlaySwordHeavy()
        {
            _audioSource.PlayOneShot(characterSounds[7]);
            _audioSource.volume = 1f;
        }
        public void PlaySpearWalking()
        {
            _audioSource.PlayOneShot(characterSounds[8]);
            _audioSource.volume = 1f;
        }
        public void PlaySpearNormal()
        {
            _audioSource.PlayOneShot(characterSounds[9]);
            _audioSource.volume = 1f;
        }
        public void PlayKick()
        {
            _audioSource.PlayOneShot(characterSounds[10]);
            _audioSource.volume = 1f;
        }
        public void PlaySpearHeavy()
        {
            _audioSource.PlayOneShot(characterSounds[11]);
            _audioSource.volume = 1f;
        }
        public void PlaySpearHeavy2()
        {
            _audioSource.PlayOneShot(characterSounds[12]);
            _audioSource.volume = 1f;
        }
        public void PlayHammerHeavy()
        {
            _audioSource.PlayOneShot(characterSounds[13]);
            _audioSource.volume = 1f;
        }
        public void PlayHammerHeavy2()
        {
            _audioSource.PlayOneShot(characterSounds[14]);
            _audioSource.volume = 1f;
        }
        public void PlayArrows()
        {
            _audioSource.PlayOneShot(characterSounds[15]);
            _audioSource.volume = 1f;
        }
        public void PlayDeath()
        {
            _audioSource.PlayOneShot(characterSounds[16]);
            _audioSource.volume = 1f;
        }
        public void PlayBandage()
        {
            _audioSource.PlayOneShot(characterSounds[17]);
            _audioSource.volume = 1f;
        }
        #endregion
        
        public void Step()
        {
            var clip = GetRandomClip();
            _audioSource.PlayOneShot(clip);
            _audioSource.volume = 0.15f;
        }

        private AudioClip GetRandomClip()
        {
            return steps[Random.Range(0, steps.Length)];
        }
    }
}
