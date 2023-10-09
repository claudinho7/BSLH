using UnityEngine;

namespace Characters.Monsters.Scripts.UtilityCore
{
    public class AIBrain : MonoBehaviour
    {
        public bool FinishedDeciding { get; set; }
        public Action BestAction { get; private set; }
        private AIController _aiController;

        private void Start()
        {
            _aiController = GetComponent<AIController>();
        }

        private void Update()
        {
            if (BestAction == null)
            {
                DecideBestAction(_aiController.actionsAvailable);
            }
        }

        //Loop through all available actions
        //Get highest scoring action
        public void DecideBestAction(Action[] actionsAvailable)
        {
            var score = 0f;
            var nextBestActionIndex = 0;

            for (var i = 0; i < actionsAvailable.Length; i++)
            {
                if (!(ScoreAction(actionsAvailable[i]) > score)) continue;
                nextBestActionIndex = i;
                score = actionsAvailable[i].Score;
            }

            BestAction = actionsAvailable[nextBestActionIndex];
            FinishedDeciding = true;
        }
        
        //Loop through all considerations of an action
        //Score all considerations
        //Average the consideration scores and get overall action score
        private float ScoreAction(Action action)
        {
            var score = 1f; //overall score of action

            foreach (var t in action.considerations)
            {
                var considerationScore = t.ScoreConsideration(_aiController);
                score *= considerationScore;

                if (score != 0) continue;
                action.Score = 0;
                return action.Score; //no need to go further
            }
            
            //Averaging the overall score
            var originalScore = score;
            float modFactor = 1 - (1 / action.considerations.Length);
            var makeupValue = (1 - originalScore) * modFactor;

            action.Score = originalScore + (makeupValue * originalScore);

            return action.Score;
        }
    }
}
