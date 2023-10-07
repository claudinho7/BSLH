using UnityEngine;

namespace Characters.Monsters.Scripts.GOAP_Main
{
    // ReSharper disable once InconsistentNaming
    public class GOAPPlanner : MonoBehaviour
    {
        private GoalBase[] _goals;
        private ActionBase[] _actions;
        private GoalBase _activeGoal;
        private ActionBase _activeAction;
        
        private void Awake() 
        { 
            _goals = GetComponents<GoalBase>(); 
            _actions = GetComponents<ActionBase>();
        }
        

        public void DoGoap()
        {
            GoalBase bestGoal = null; 
            ActionBase bestAction = null;

            // find the highest priority goal that can be activated
            foreach(var goal in _goals)
            { 
                // first tick the goal
                goal.OnTickGoal();
                
                // can it run?
                if (!goal.CanRun()) 
                    continue;

                // is it a worse priority?
                if (!(bestGoal == null || goal.CalculatePriority() > bestGoal.CalculatePriority())) 
                    continue;

                // find the best cost action
                ActionBase candidateAction = null; 
                foreach(var action in _actions) 
                { 
                    if (!action.GetSupportedGoals().Contains(goal.GetType())) 
                        continue;

                    // found a suitable action
                    if (candidateAction == null || action.Cost() < candidateAction.Cost()) 
                        candidateAction = action; 
                }

                // did we find an action?
                if (candidateAction != null) 
                { 
                    bestGoal = goal; 
                    bestAction = candidateAction; 
                } 
            }

            // if no current goal
            if (_activeGoal == null) 
            { 
                _activeGoal = bestGoal; 
                _activeAction = bestAction;
                
                if (_activeGoal != null) 
                    _activeGoal.OnGoalActivated(_activeAction); 
                if (_activeAction != null) 
                    _activeAction.OnActivated(_activeGoal); 
            } // no change in goal?
            else if (_activeGoal == bestGoal) 
            {
                // action changed?
                if (_activeAction != bestAction) 
                { 
                    _activeAction.OnDeactivated();
                    
                    _activeAction = bestAction;
                    
                    if (_activeAction != null) _activeAction.OnActivated(_activeGoal); 
                } //same action? repeat
                else if (_activeAction == bestAction)
                {
                    if (_activeAction != null) _activeAction.OnActivated(_activeGoal);
                }
            } // new goal or no valid goal
            else if (_activeGoal != bestGoal) 
            { 
                _activeGoal.OnGoalDeactivated(); 
                _activeAction.OnDeactivated();
                
                _activeGoal = bestGoal; 
                _activeAction = bestAction;
                
                if (_activeGoal != null) 
                    _activeGoal.OnGoalActivated(_activeAction); 
                if (_activeAction != null) 
                    _activeAction.OnActivated(_activeGoal); 
            }

            // tick the action
            if (_activeAction != null) 
                _activeAction.OnTick();
        }
    }
}
