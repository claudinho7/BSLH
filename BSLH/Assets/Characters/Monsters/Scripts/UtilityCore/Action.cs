using UnityEngine;

namespace Characters.Monsters.Scripts.UtilityCore
{
    public abstract class Action : ScriptableObject
    {
        public string actionName;
        public float score;
        
        public Consideration[] considerations;
        
        public float Score
        {
            get => score;
            set => score = Mathf.Clamp01(value);
        }
        
        public virtual void Awake()
        {
            Score = 0;
        }

        public abstract void Execute(AIController aiController);
    }
}
