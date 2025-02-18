using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace GOAP {
    public class WanderToPosition : GoapAction, IAction {

        public float wanderRadius;
        public LayerMask groundLayers;

        private Vector3 lastPosition;
        private int samePosition;

        string IAction.ActionName() => actionName;

        float IAction.Duration() => duration;

        int IAction.CalculateCost() => cost;

        bool IAction.IsAchievable(GameObject Agent) {
            return true;
        }

        bool IAction.StartAction(GameObject Agent) {
            Initialise(Agent);
            // Get all nearby walkable ground
            Collider[] surroundingArea = Physics.OverlapSphere(agent.transform.position, wanderRadius, groundLayers);

            // If none was found, don't continue with the action
            if(surroundingArea.Length == 0) {
                return false;
            }

            // Allow the agent to move, and set their target destination a random position
            navMeshAgent.isStopped = false;
            blackboard.targetLocation = surroundingArea[Random.Range(0, surroundingArea.Length)].transform.position;
            navMeshAgent.SetDestination(blackboard.targetLocation + Vector3.up);

            HearingManager.instance.EmitSound(agent.transform.position, soundToPlay, 2, agent);
            isRunning = true;
            animator.SetBool("Moving", true);
            return true;
        }

        bool IAction.UpdateAction(float deltaTime) {
            navMeshAgent.isStopped = false;
            // If the agents position remains the same for a while, abort the action
            if (Vector3.Distance(agent.transform.position, lastPosition) <= Mathf.Epsilon) {
                samePosition++;
                if(samePosition >= 100) {
                    return false;
                }
            }

            navMeshAgent.SetDestination(blackboard.targetLocation);

            // If the path is no longer valid, abort the action
            if (navMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid) {
                navMeshAgent.ResetPath();
                return false;
            }

            lastPosition = agent.transform.position;
            return true;
        }

        void IAction.ComleteAction() {
            animator.SetBool("Moving", false);
        }

        void IAction.StopAction() {
            // Reset variables
            navMeshAgent.isStopped = false;
            HearingManager.instance.EmitSound(agent.transform.position, soundToPlay, 2, agent);
            isRunning = false;
        }

        List<Condition> IAction.GetPreConditions() => preConditions;

        List<Condition> IAction.GetEffects() => postConditions;

        bool IAction.Running() => isRunning;

        bool IAction.WithinRange() {
            // Check if the agent is near their target
            float distanceToTarget = Vector3.Distance(agent.transform.position, blackboard.targetLocation);
            if (distanceToTarget < 2f) {
                return true;
            }
            return false;
        }
    }

}