using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GOAP {

    public class GoTo : GoapAction, IAction {

        string IAction.ActionName() => actionName;

        float IAction.Duration() => duration;

        int IAction.CalculateCost() => cost;

        bool IAction.IsAchievable(GameObject Agent) {
            Initialise(Agent);
            return true;
        }

        bool IAction.StartAction(GameObject Agent) {
            navMeshAgent.SetDestination(blackboard.targetLocation);
            // If the path isn't valid, return false so it doesn't get ran
            if (navMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid) {
                return false;
            }
            isRunning = true;
            return true;
        }

        bool IAction.UpdateAction(float deltaTime) {
            navMeshAgent.SetDestination(blackboard.targetLocation);

            // If the path is no longer valid, abort the action
            if(navMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid) {
                return false;
            }

            // If the agent is near their target, set isRunning to false to regester the action as complete
            float distanceToTarget = Vector3.Distance(agent.transform.position, blackboard.targetLocation);
            if (distanceToTarget < 2f) {
                isRunning = false;
            }

            return true;
        }

        void IAction.ComleteAction() {
            
        }

        void IAction.StopAction() {
            isRunning = false;
        }

        List<Condition> IAction.GetPreConditions() => preConditions;

        List<Condition> IAction.GetEffects() => postConditions;

        bool IAction.Running() => isRunning;

        bool IAction.WithinRange() {
            return true;
        }
    }

}