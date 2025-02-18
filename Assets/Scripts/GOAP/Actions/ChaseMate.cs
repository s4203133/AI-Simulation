using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace GOAP {
    public class ChaseMate : GoapAction, IAction {

        bool hasTarget;

        string IAction.ActionName() => actionName;

        float IAction.Duration() => duration;

        int IAction.CalculateCost() => cost;

        bool IAction.IsAchievable(GameObject Agent) {
            Initialise(Agent);

            // If this agent has recently reproduced, then this isn't a valid action
            if (aiAgent.reproduction.RecentlyReproduced()) {
                return false;
            }

            // If there are no available agents available, then this action can't be run
            List<DetectableObject> validAgents = aiAgent.reproduction.AgentAvailable(memory.GetFoxes);
            if (validAgents.Count == 0) {
                return false;
            }
            return true;
        }

        bool IAction.StartAction(GameObject Agent) {
            // If there is no target object, don't continue
            if (blackboard.targetObject == null) {
                return false;
            }

            // Allow the agent to move, and set their destination to be the last known position of their target
            navMeshAgent.isStopped = false;
            blackboard.targetLocation = memory.GetObjectsLastKnownLocation(blackboard.targetObject.GetComponent<DetectableObject>());
            navMeshAgent.SetDestination(blackboard.targetLocation);
            hasTarget = false;

            HearingManager.instance.EmitSound(agent.transform.position, soundToPlay, 2, agent);
            isRunning = true;
            animator.SetBool("Moving", true);

            return true;
        }

        bool IAction.UpdateAction(float deltaTime) {
            // If the agent had a target but lost it, cancel the action
            if (hasTarget && blackboard.targetObject == null) {
                return false;
            }

            // If the agent has sights on aiAgent, continue to move towards their position
            if(hasTarget) {
                blackboard.targetLocation = blackboard.targetObject.transform.position;
                navMeshAgent.SetDestination(blackboard.targetLocation);
                return true;
            }
            // If, while moving to the last known position, the agent sees a aiAgent, set them as the new target to move to
            if (memory.ObjectsOfTypeIsInView(EDetectableObjectCategories.FOX)) {
                List<DetectableObject> validAgents = aiAgent.reproduction.AgentAvailable(memory.GetFoxes);
                DetectableObject closestaiAgent = memory.ClosestObject(validAgents, agent.transform.position);
                if (closestaiAgent != null) {
                    blackboard.targetObject = closestaiAgent.gameObject;
                    blackboard.targetLocation = closestaiAgent.transform.position;
                    navMeshAgent.SetDestination(blackboard.targetLocation);
                    hasTarget = true;
                    return true;
                }
            }

            // Otherwise, continue moving the the last known position of the target
            navMeshAgent.SetDestination(blackboard.targetLocation);

            // If the path is no longer valid, abort the action
            if (navMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid) {
                return false;
            }

            return true;
        }

        void IAction.ComleteAction() {
            // If no tartget was found, clear whatever target object exists in the blackboard
            if (!hasTarget) {
                blackboard.targetObject = null;
            }
            animator.SetBool("Moving", false);
        }

        void IAction.StopAction() {
            hasTarget = false;
            navMeshAgent.isStopped = true;
            HearingManager.instance.EmitSound(agent.transform.position, soundToPlay, 2, agent);
            isRunning = false;
        }

        List<Condition> IAction.GetPreConditions() => preConditions;

        List<Condition> IAction.GetEffects() => postConditions;

        bool IAction.Running() => isRunning;

        bool IAction.WithinRange() {
            // Check if the agent is near their target
            float distanceToTarget = Vector3.Distance(agent.transform.position, blackboard.targetLocation);
            if (distanceToTarget <= 6f) {
                return true;
            }
            return false;
        }
    }

}
