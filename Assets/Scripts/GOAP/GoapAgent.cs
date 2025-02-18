using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GOAP {

    public class GOAPAgent : MonoBehaviour {

        [Tooltip("The child object of the agent that contains all the GOAP actions (as components)")]
        [SerializeField] private GameObject allActionsHolder;
        private List<IAction> allActions;
        private Queue<IAction> actionQueue;
        private IAction currentAction;
        public string currentActionName;

        public List<GoapGoal> allGoals { private set; get; }
        private GoapGoal currentGoal;

        private bool actionFinishing;

        public GoapBeliefs goalBeliefs { private set; get; }

        private GoapPlanner planner;

        bool planCancelledExternally;

        [Space(25)]
        public bool DebugPlan;

        private void Awake() {
            allActions = allActionsHolder.GetComponents<IAction>().ToList();
            actionQueue = new Queue<IAction>();
            allGoals = new List<GoapGoal>();
            goalBeliefs = GetComponent<GoapBeliefs>();
        }

        private void LateUpdate() {
            if (planCancelledExternally) {  return; }

            // If the agent is currently working on an action, check to see if they are in range and complete it
            if (currentAction != null && currentAction.Running()) {
                // If the currently running action is no longer valid, cancel it remove the current plan
                if (!actionFinishing) {
                    bool actionStillValid = currentAction.UpdateAction(Time.deltaTime);
                    if (!actionStillValid) {
                        currentAction.StopAction();
                        allGoals.Remove(currentGoal);
                        planner = null;
                        actionQueue = null;
                        return;
                    }
                }

                // If the agent is within range of the location where the action takes place, complete it
                if (currentAction.WithinRange()) {
                    if (!actionFinishing) {
                        actionFinishing = true;
                        currentAction.ComleteAction();
                        Invoke("CompleteAction", currentAction.Duration());
                    }
                }
                return;
            }

            // If agent has no plans to work from, generate a new one
            if(planner == null || actionQueue == null) {
                planner = new GoapPlanner();

                // Sort the goals based on their priority value
                var sortedGoals = from entry in allGoals orderby entry.priority descending select entry;

                foreach(var goal in sortedGoals) {
                    actionQueue = planner.Plan(gameObject, allActions, goal, goalBeliefs.allBeliefs, DebugPlan);
                    if(actionQueue != null) {
                        currentGoal = goal;
                        break;
                    }
                }
            }

            // If the agent has finished all actions, remove the current goal they've been working towards
            if(actionQueue != null && actionQueue.Count == 0) {
                allGoals.Remove(currentGoal);
                // A null planner will cause a new plan to be generated next frame
                planner = null;
            }

            // If the agent has still got actions left to perform
            if(actionQueue != null && actionQueue.Count > 0) {
                currentAction = actionQueue.Dequeue();
                currentActionName = currentAction.ActionName();
                // If the action cannot be ran, empty the action queue so a new plan is made
                if (!currentAction.StartAction(gameObject)) {
                    actionQueue = null;
                }
            }
        }

        void CompleteAction() {
            currentAction.StopAction();
            actionFinishing = false;
        }
    }
}