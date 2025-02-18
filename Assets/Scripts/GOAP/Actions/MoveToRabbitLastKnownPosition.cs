using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace GOAP {
    public class MoveToRabbitLastKnownPosition : GoapAction, IAction {

        bool targetWasntThere;

        public ESoundCategories soundsToListenFor;

        string IAction.ActionName() => actionName;

        float IAction.Duration() => duration;

        int IAction.CalculateCost() => cost;

        bool IAction.IsAchievable(GameObject Agent) {
            // If the agent recently followed a sound but found no target, then return false
            if (targetWasntThere) {
                return false;
            }
            Initialise(Agent);
            return blackboard.hasHeardRabbit;
        }

        bool IAction.StartAction(GameObject Agent) {
            // Set the nav meshes destination to the foxes target location (will have been found from the previous action)
            navMeshAgent.SetDestination(blackboard.targetLocation);
            // Allow nav mesh agent to start moving again
            navMeshAgent.isStopped = false;
            targetWasntThere = false;
            isRunning = true;

            HearingManager.instance.EmitSound(agent.transform.position, soundToPlay, 2, agent);
            animator.SetBool("Moving", true);
            return true;
        }

        bool IAction.UpdateAction(float deltaTime) {
            // If the path is no longer valid, abort the action
            if (navMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid) {
                navMeshAgent.ResetPath();
                return false;
            }

            // If the agent is able to detect a rabbit, then trigger the chase action
            else if (memory.ObjectsOfTypeIsInView(EDetectableObjectCategories.RABBIT)) {
                DetectableObject seenTarget = memory.GetClosestItem(EDetectableObjectCategories.RABBIT, agent.transform);
                if (seenTarget != null) {
                    blackboard.targetObject = seenTarget.gameObject;
                    // Setting target location to transform position will cause this action to succeed when comparing 'WithinRange()', allowing it to move onto the chase action
                    blackboard.targetLocation = transform.position;
                    navMeshAgent.SetDestination(blackboard.targetObject.transform.position);
                }
            }

            return true;
        }

        void IAction.ComleteAction() {
            animator.SetBool("Moving", false);
        }

        void IAction.StopAction() {
            aiAgent.IsUsingStamina(false);
            // Check if a target was found or not
            if (blackboard.targetObject != null) {
                targetWasntThere = false;
            } else {
                targetWasntThere = true;
            }
            Invoke("SearchAgain", 3);
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
            if (distanceToTarget < 3.5f) {
                return true;
            }
            return false;
        }

        private void SearchAgain() {
            targetWasntThere = false;
        }
    }
}