using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace GOAP {
    public class SneakUpOnRabbit : GoapAction, IAction {

        private float agentNormalSpeed;
        private float agentSneakSpeed;   

        Rabbit targetRabbit;
        DetectableObject detectableRabbit;

        string IAction.ActionName() => actionName;

        float IAction.Duration() => duration;

        int IAction.CalculateCost() => cost;

        bool IAction.IsAchievable(GameObject Agent) {
            Initialise(Agent);
            // If there's no rabbits in view, then this action isn't achievable
            if (!memory.ObjectsOfTypeIsInView(EDetectableObjectCategories.RABBIT)) {
                return false;
            }

            bool returnValue = false;
            if (blackboard.targetObject != null) {
                // If the blackboards target object is a rabbit that isn't aware of the fox, then the action is achievable
               if(blackboard.targetObject.TryGetComponent(out targetRabbit)) {
                   if (!targetRabbit.combat.feelsThreatened) {
                        blackboard.unawareTarget = true;
                        returnValue = true;
                   }
               }
            }
            targetRabbit = null;
            return returnValue;
        }

        bool IAction.StartAction(GameObject Agent) {
            // If there's no target object, don't continue
            if (blackboard.targetObject == null) {
                return false;
            }

            // Stop the nav mesh from moving
            navMeshAgent.isStopped = false;

            // Set the speed of the nav mesh to be slower
            agentNormalSpeed = navMeshAgent.speed;
            agentSneakSpeed = agentNormalSpeed * 0.65f;
            navMeshAgent.speed = agentSneakSpeed;
            aiAgent.isSneaking = true;
            
            // Set the destination of the fox to be behind their prey
            navMeshAgent.SetDestination(blackboard.targetObject.transform.position - (blackboard.targetObject.transform.forward * -4));

            targetRabbit = blackboard.targetObject.GetComponent<Rabbit>();
            detectableRabbit = blackboard.targetObject.GetComponent<DetectableObject>();

            isRunning = true;

            return true;
        }

        bool IAction.UpdateAction(float deltaTime) {
            // If the target object doesnt't exist anymore, don't continue
            if (blackboard.targetObject == null) {
                return false;
            }

            // If the target rabbit becomes alert, then setting the destination to this agents position will make the action succeed and continue to the next one
            if (targetRabbit.combat.feelsThreatened) {
                navMeshAgent.SetDestination(agent.transform.position);
                return false;
            }
             
            // If the rabbit has entered a hiding zone, then the fox can no longer see them
            if (targetRabbit.combat.isHidden) {
                return false;
            }

            // If the fox can no longer see there prey, don't continue
            if (!memory.ObjectIsInView(detectableRabbit)) {
                return false;
            }

            // Continue to update the foxes target position to be behind the fox
            navMeshAgent.SetDestination(blackboard.targetObject.transform.position - (blackboard.targetObject.transform.forward * -3));

            // If the path is no longer valid, abort the action
            if (navMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid) {
                return false;
            }

            return true;
        }


        void IAction.ComleteAction() {

        }

        void IAction.StopAction() {
            // Reset the speed and variables
            navMeshAgent.speed = agentNormalSpeed;
            aiAgent.isSneaking = false;
            navMeshAgent.isStopped = true;
            // Check the foxes target is still valid
            if (targetRabbit != null) {
                blackboard.targetObject = targetRabbit.gameObject;
            }
            targetRabbit = null;
            detectableRabbit = null;
            blackboard.unawareTarget = false;
            isRunning = false;
        }

        List<Condition> IAction.GetPreConditions() => preConditions;

        List<Condition> IAction.GetEffects() => postConditions;

        bool IAction.Running() => isRunning;

        bool IAction.WithinRange() {
            // Check if the agent is near their target
            float distanceToTarget = Vector3.Distance(agent.transform.position, navMeshAgent.destination);
            if (distanceToTarget < 3.5f) {
                return true;
            }
            return false;
        }
    }

}

