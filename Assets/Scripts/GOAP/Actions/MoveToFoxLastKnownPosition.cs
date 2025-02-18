using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace GOAP {
    public class MoveToFoxLastKnownPosition : GoapAction, IAction {

        string IAction.ActionName() => actionName;

        float IAction.Duration() => duration;

        int IAction.CalculateCost() => cost;

        bool IAction.IsAchievable(GameObject Agent) {
            Initialise(Agent);
            return true;
        }

        bool IAction.StartAction(GameObject Agent) {
            // Set the nav meshes destination to the foxes target location (will have been found from the previous action)
            navMeshAgent.SetDestination(blackboard.targetLocation);
            navMeshAgent.isStopped = false;
            isRunning = true;

            HearingManager.instance.EmitSound(agent.transform.position, ESoundCategories.foxMoving, 2, agent);

            animator.SetBool("Moving", true);
            return true;
        }

        bool IAction.UpdateAction(float deltaTime) {
            // If the path is no longer valid, abort the action
            if (navMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid) {
                navMeshAgent.ResetPath();
                return false;
            }

            // If the agent has already seen a target, update their target destination to its position
            if (blackboard.targetObject != null) {
                blackboard.targetLocation = blackboard.targetObject.transform.position;
                navMeshAgent.SetDestination(blackboard.targetLocation);
            }
            // If the agent is able to detect a fox, change their destination to be the foxes location
            else if (memory.ObjectsOfTypeIsInView(EDetectableObjectCategories.FOX)) {
                DetectableObject obj = memory.GetClosestItem(EDetectableObjectCategories.FOX, agent.transform);
                //DetectableObject seenTarget = GetTargetFox();

                // If an object was found, update the target object and location
                if(obj != null) {
                    blackboard.targetObject = obj.gameObject;
                    blackboard.targetLocation = obj.transform.position;
                    navMeshAgent.SetDestination(blackboard.targetLocation);
                }
            }

            return true;
        }

        void IAction.ComleteAction() {
            animator.SetBool("Moving", false);
        }

        void IAction.StopAction() {
            // Reset and stop the nav mesh from moving
            navMeshAgent.isStopped = false;
            isRunning = false;
        }

        List<Condition> IAction.GetPreConditions() => preConditions;

        List<Condition> IAction.GetEffects() => postConditions;

        bool IAction.Running() => isRunning;

        bool IAction.WithinRange() {
            // Check if the agent is near their target
            float distanceToTarget = Vector3.Distance(agent.transform.position, blackboard.targetLocation);
            if (distanceToTarget < 5f) {
                return true;
            }
            return false;
        }

        DetectableObject GetTargetFox() {
            List<DetectableObject> targets = FilterRejectedFoxes();
            float minDist = float.MaxValue;
            DetectableObject target = null;
            for(int i = 0; i < targets.Count; i++) {
                float dist = Vector3.Distance(targets[i].transform.position, agent.transform.position);
                if(dist < minDist) {
                    minDist = dist;
                    target = targets[i];
                }
            }
            return target;
        }

        List<DetectableObject> FilterRejectedFoxes() {
            List<DetectableObject> allFoxes = memory.GetFoxes;
            List<DetectableObject> targets = new List<DetectableObject>();
            for (int i = 0; i < allFoxes.Count; i++) {
                Fox otherFox = allFoxes[i].GetComponent<Fox>();
                if (aiAgent.reproduction.HasBeenRejectedByThisAgent(otherFox)) {
                    continue;
                } else {
                    targets.Add(allFoxes[i]);
                }
            }
            return targets;
        }
    }

}
