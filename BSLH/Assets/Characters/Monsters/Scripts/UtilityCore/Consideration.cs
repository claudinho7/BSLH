using UnityEngine;

namespace Characters.Monsters.Scripts.UtilityCore
{
    public abstract class Consideration : ScriptableObject
    {
        public string considerationName;
        private float _score;
        
        public float Score
        {
            get => _score;
            set => _score = Mathf.Clamp01(value);
        }

        public virtual void Awake()
        {
            Score = 0;
        }

        public abstract float ScoreConsideration(AIController aiController);
    }
}
