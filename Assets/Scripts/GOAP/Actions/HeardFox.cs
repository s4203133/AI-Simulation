using System.Collections.Generic;
using UnityEngine;

namespace GOAP {

    public class HeardFox : GoapAction, IAction {
        public ESoundCategories soundsToListenFor;

        string IAction.ActionName() => actionName;

        float IAction.Duration() => duration;

        int IAction.CalculateCost() => cost;

        bool IAction.IsAchievable(GameObject Agent) {
            Initialise(Agent);
            return blackboard.hasHeardFox;
        }

        bool IAction.StartAction(GameObject Agent) {
            // Get all sounds of the given type
            List<HeardSound> sounds = sensorySystem.hearingSensor.GetHeardSoundsOfType(soundsToListenFor);
            // Find the closest sound the agent has heard for them to move towards
            float minDist = float.MaxValue;
            Vector3 location = Vector3.zero;
            int count = sounds.Count;
            for (int i = 0; i < count; i++) {
                float dist = Vector3.Distance(Agent.transform.position, sounds[i].location);
                if (dist < minDist) {
                    minDist = dist;
                    location = sounds[i].location;
                }
            }
            // Set location but null the target, as the agent woukdn't know what the object is, only it's location
            blackboard.targetLocation = location;
            blackboard.targetObject = null;

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
    }
}