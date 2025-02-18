using System.Collections.Generic;
using UnityEngine;


namespace GOAP {
    public class SearchForMate : GoapAction, IAction {

        string IAction.ActionName() => actionName;

        float IAction.Duration() => duration;

        int IAction.CalculateCost() => cost;

        bool IAction.IsAchievable(GameObject Agent) {
            Initialise(Agent);
            // If the agent recently reproduced, then this action isn't valid
            if (aiAgent.reproduction.RecentlyReproduced()) {
                return false;
            }

            // If there are no agents to reproduce with, then this action can't be performed
            List<DetectableObject> validAgents = aiAgent.reproduction.AgentAvailable(memory.GetFoxes);
            if(validAgents.Count == 0) {
                return false;
            }

            return true;
        }

        bool IAction.StartAction(GameObject Agent) {
            // Get all valid agents to reproduce with
            List<DetectableObject> validAgents = aiAgent.reproduction.AgentAvailable(memory.GetFoxes);

            // If none were found, don't continue
            if (validAgents.Count == 0) {
                return false; 
            }

            // Get the cloest fox out of the valid targets
            DetectableObject closestFox = ClosestObject(validAgents, agent.transform.position);

            if (closestFox == null) {
                return false;
            }

            // Set the found fox to be the target destination
            blackboard.targetObject = closestFox.gameObject;
            blackboard.targetLocation = closestFox.transform.position;
            navMeshAgent.SetDestination(blackboard.targetLocation);

            isRunning = true;
            return true;
        }

        bool IAction.UpdateAction(float deltaTime) {
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

        private DetectableObject ClosestObject(List<DetectableObject> objects, Vector3 position) {
            DetectableObject returnObject = null;
            float numberObjects = objects.Count;
            float minDist = float.MaxValue;

            // Get the distance to every target and return the closest one
            for (int i = 0; i < numberObjects; i++) {
                DetectableObject obj = objects[i];
                float dist = (obj.transform.position - position).sqrMagnitude;

                // If distance to target is closer than the current closest target
                if (dist < minDist) {
                    returnObject = obj;
                    minDist = dist;
                }
            }
            return returnObject;
        }
    }

}
