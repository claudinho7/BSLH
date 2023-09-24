using Characters.Monsters.Scripts.Actions;
using Characters.Monsters.Scripts.Goals;
using UnityEngine;

namespace Characters.Monsters.Scripts.GOAP_Main
{
    // ReSharper disable once InconsistentNaming
    public class GOAPPlanner : MonoBehaviour
    {
        GoalBase[] Goals;
    ActionBase[] Actions;

    GoalBase ActiveGoal;
    ActionBase ActiveAction;

    void Awake()
    {
        Goals = GetComponents<GoalBase>();
        Actions = GetComponents<ActionBase>();
    }

    void Update()
    {
        GoalBase bestGoal = null;
        ActionBase bestAction = null;

        // find the highest priority goal that can be activated
        foreach(var goal in Goals)
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
            foreach(var action in Actions)
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
        if (ActiveGoal == null)
        {
            ActiveGoal = bestGoal;
            ActiveAction = bestAction;

            if (ActiveGoal != null)
                ActiveGoal.OnGoalActivated(ActiveAction);
            if (ActiveAction != null)
                ActiveAction.OnActivated(ActiveGoal);            
        } // no change in goal?
        else if (ActiveGoal == bestGoal)
        {
            // action changed?
            if (ActiveAction != bestAction)
            {
                ActiveAction.OnDeactivated();
                
                ActiveAction = bestAction;

                ActiveAction.OnActivated(ActiveGoal);
            }
        } // new goal or no valid goal
        else if (ActiveGoal != bestGoal)
        {
            ActiveGoal.OnGoalDeactivated();
            ActiveAction.OnDeactivated();

            ActiveGoal = bestGoal;
            ActiveAction = bestAction;

            if (ActiveGoal != null)
                ActiveGoal.OnGoalActivated(ActiveAction);
            if (ActiveAction != null)
                ActiveAction.OnActivated(ActiveGoal);
        }

        // tick the action
        if (ActiveAction != null)
            ActiveAction.OnTick();
    }
    }
}
