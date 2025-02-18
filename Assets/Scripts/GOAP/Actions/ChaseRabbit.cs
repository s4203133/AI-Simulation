using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace GOAP {
    public class ChaseRabbit : GoapAction, IAction {

        Rabbit targetRabbit;

        bool canRetry = true;
        bool hadTarget = false;

        string IAction.ActionName() => actionName;

        float IAction.Duration() => duration;

        int IAction.CalculateCost() => cost;

        bool IAction.IsAchievable(GameObject Agent) {
            // If the agent has recently completed this action, don't allow it to perform again
            if (!canRetry) {
                return false;
            }
            Initialise(Agent);
            return true;
        }

        bool IAction.StartAction(GameObject Agent) {
            // If the agent has recently completed this action, don't continue
            if (!canRetry) {
                return false;
            }

            // If the agent doesn't have a last known position don't continue
            if (blackboard.targetLocation == Vector3.zero) {
                navMeshAgent.ResetPath();
                return false;
            }

            // If the agent has a target object, get it's rabbit component
            if(blackboard.targetObject != null) {
                targetRabbit = blackboard.targetObject.GetComponent<Rabbit>();
                hadTarget = true;
            }

            // Allow the agent to move and set it's destination
            navMeshAgent.isStopped = false;
            navMeshAgent.SetDestination(blackboard.targetLocation);

            HearingManager.instance.EmitSound(agent.transform.position, soundToPlay, 2, agent);
            isRunning = true;

            animator.SetBool("Moving", true);
            return true;
        }

        bool IAction.UpdateAction(float deltaTime) {
            // If the agent lost the last known position or lost it's target, don't continue
            if(blackboard.targetLocation == Vector3.zero || (hadTarget && blackboard.targetObject == null)) {
                return false;
            }
            navMeshAgent.isStopped = false;

            // If, while moving to the rabbits last known position, the agent sees a rabbit, set them as the new target to move to
            if (blackboard.targetObject != null) {
                // If the target rabbit has entered cover, then they are no longer visible to the aiAgent, so return false
                if (targetRabbit != null) {
                    if (targetRabbit.combat.isHidden) {
                        return false;
                    }
                }

                // If the agent runs out of stamina while chasing their target, then return false
                if (aiAgent.stamina <= 0) {
                    aiAgent.ChangeBelief(eConditions.RUN_OUT_OF_STAMINA, true);
                    return false;
                }

                // Otherwise update the aiAgents destination to the rabbits predicted location (persue behaviour)
                blackboard.targetLocation = blackboard.targetObject.transform.position;
                navMeshAgent.SetDestination(targetRabbit.PredictedDestination());
                aiAgent.IsUsingStamina(true);

            } else if (memory.ObjectsOfTypeIsInView(EDetectableObjectCategories.RABBIT)) {
                // Check if the agent is now able to detect a rabbit in the view range, and move towards it if so
                DetectableObject closestRabbit = memory.GetClosestItem(EDetectableObjectCategories.RABBIT, agent.transform);
                blackboard.targetObject = closestRabbit.gameObject;
                blackboard.targetLocation = closestRabbit.transform.position;
                targetRabbit = closestRabbit.GetComponent<Rabbit>();
                navMeshAgent.SetDestination(targetRabbit.PredictedDestination());
                aiAgent.IsUsingStamina(true);
                hadTarget = true;
            }

            // If the path is no longer valid, abort the action
            if (navMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid) {
                return false;
            }

            return true;
        }

        void IAction.ComleteAction() {
            animator.SetBool("Moving", false);
        }

        void IAction.StopAction() {
            // Stop the nav mesh agent
            navMeshAgent.isStopped = true;
            //If the fox found a rabbit, set it ot be the target object
            if (targetRabbit != null) {
                blackboard.targetObject = targetRabbit.gameObject;
            }
            // Reset Variables
            targetRabbit = null;
            hadTarget = false;
            canRetry = false;
            aiAgent.IsUsingStamina(false);
            Invoke("AllowRetry", 2);
            isRunning = false;
        }

        List<Condition> IAction.GetPreConditions() => preConditions;

        List<Condition> IAction.GetEffects() => postConditions;

        bool IAction.Running() => isRunning;

        bool IAction.WithinRange() {
            // Check if the agent is near their target
            float distanceToTarget = 5;
            if (blackboard.targetObject != null) {
                distanceToTarget = Vector3.Distance(agent.transform.position, blackboard.targetObject.transform.position);
            } else {
                distanceToTarget = Vector3.Distance(agent.transform.position, blackboard.targetLocation);
            }

            if (distanceToTarget < 4f) {
                return true;
            }
            return false;
        }

        void AllowRetry() {
            canRetry = true;
        }
    }

}