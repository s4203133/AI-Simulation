using System.Collections.Generic;
using UnityEngine;

namespace GOAP {
    public class FollowRabbitScent : GoapAction, IAction {

        private ScentTrail trail;
        private int index;
        private bool reachedEnd;

        string IAction.ActionName() => actionName;

        float IAction.Duration() => duration;

        int IAction.CalculateCost() => cost;

        bool IAction.IsAchievable(GameObject Agent) {
            Initialise(Agent);
            if (sensorySystem.senseOfSmell.currentTrailFollowing == null || // If the agent doesn't hasn't caught a scent trail
                sensorySystem.senseOfSmell.currentTrailFollowing.scentTrail.Count == 0 || // If the current trail doesn't have any nodes in
                !blackboard.caughtSenseOfSmell) { // If the agen't isn't aware of a scent
                return false;
            }
            return true;
        }

        bool IAction.StartAction(GameObject Agent) {
            // If there's no trail to follow, don't continue
            if(sensorySystem.senseOfSmell.currentTrailFollowing == null) {
                return false;
            }

            reachedEnd = false;
            navMeshAgent.isStopped = false;
            isRunning = true;

            // Cache the trail
            trail = sensorySystem.senseOfSmell.currentTrailFollowing;
            if (trail.scentTrail.Count > 1) {
                // If there is more than one node in the trail, get which one the agent has encountered, and set their destination
                index = GetProgressThroughTrail();
                navMeshAgent.SetDestination(trail.scentTrail[index].transform.position);
            } else if(trail.scentTrail.Count == 1) {
                // If there's only one node in the trail, then the agent has already found it so they're at the end
                reachedEnd = true;
            }

            // Clear the target object, as the fox doesn't know which object is at the end of the trail
            blackboard.targetObject = null;
            animator.SetBool("Moving", true);
            return true;
        }

        bool IAction.UpdateAction(float deltaTime) {
            // If the agent has reached the end then the action is successful
            if (reachedEnd) {
                return true;
            }

            // If the fox detects a rabbit while following the trail, mark it as the new target
            if (memory.ItemExistsInMemory(EDetectableObjectCategories.RABBIT)) {
                DetectableObject closestRabbit = memory.GetClosestItem(EDetectableObjectCategories.RABBIT, agent.transform);
                blackboard.targetLocation = closestRabbit.transform.position;
                blackboard.targetObject = closestRabbit.gameObject;
                navMeshAgent.SetDestination(closestRabbit.transform.position);
                reachedEnd = true;
                return true;
            }

            // Update the scent trail
            trail = sensorySystem.senseOfSmell.currentTrailFollowing;
            if (!blackboard.caughtSenseOfSmell || trail.scentTrail == null || trail.scentTrail.Count == 0) {
                return false;
            }

            // If theres only one scent node in the list, then return true as we've already found it
            if (trail.scentTrail.Count == 1) {
                reachedEnd = true;
                return true;
            }

            // Get the distance to the current node
            float dist = Vector3.Distance(agent.transform.position, navMeshAgent.destination);
            if (dist < 3f) {
                // Move to the next Node
                index++;
                // If we reached the end of the trail, then the agent has succeeded
                if (index >= trail.scentTrail.Count) {
                    reachedEnd = true;
                    return true;
                }
                // Otherwise continue with the trail
                navMeshAgent.SetDestination(trail.scentTrail[index].transform.position);
            }

            return true;
        }
            

        void IAction.ComleteAction() {
            animator.SetBool("Moving", false);
            sensorySystem.senseOfSmell.FoundEndOfTrail();
        }

        void IAction.StopAction() {
            // If the agent reached the last known position without coming across a target, then they failed the action
            sensorySystem.senseOfSmell.FoundEndOfTrail();
            navMeshAgent.isStopped = false;
            HearingManager.instance.EmitSound(agent.transform.position, soundToPlay, 2, agent);
            isRunning = false;
        }

        List<Condition> IAction.GetPreConditions() => preConditions;

        List<Condition> IAction.GetEffects() => postConditions;

        bool IAction.Running() => isRunning;

        bool IAction.WithinRange() {
            return reachedEnd;
        }

        private int GetProgressThroughTrail() {
            index = 0;
            // If there's no found node, then return the start of the trail
            if(sensorySystem.senseOfSmell.foundNode == null) {
                return index;
            }
            // Find the scent node in the trail that the fox encountered, and return it's index. The trail will be followed from this point
            ScentNode currentNode = sensorySystem.senseOfSmell.foundNode;
            for (int i = 0; i < trail.scentTrail.Count; i++) {
                if (currentNode == trail.scentTrail[i]) {
                    index = i;
                    break;
                }
            }
            return index;
        }
    }

}