using System.Collections.Generic;
using UnityEngine;

namespace GOAP {

    public class FindRabbit : GoapAction, IAction {

        [SerializeField] private int searchRange;
        [SerializeField] private LayerMask rabbitLayer;

        string IAction.ActionName() => actionName;

        float IAction.Duration() => duration;

        int IAction.CalculateCost() => cost;

        bool IAction.IsAchievable(GameObject Agent) {
            Initialise(Agent);
            // Check that a rabbit exists in memory
            if (memory.ItemExistsInMemory(EDetectableObjectCategories.RABBIT)) {
                blackboard.targetObject = memory.GetClosestItem(EDetectableObjectCategories.RABBIT, agent.transform).gameObject;
                // If the rabbit is aware of the fox, then don't allow them to sneak up on their prey, otherwise, do let them
                if (blackboard.targetObject.GetComponent<Rabbit>().combat.feelsThreatened) {
                    blackboard.unawareTarget = false;
                    aiAgent?.ChangeBelief(eConditions.TARGET_IS_UNSUSPECTING, false);
                } else {
                    blackboard.unawareTarget = true;
                    aiAgent?.ChangeBelief(eConditions.TARGET_IS_UNSUSPECTING, true);
                }
                return true;
            }
            // Return false if no rabbits are found
            return false;
        }

        bool IAction.StartAction(GameObject Agent) {
            // Get the closest rabbit in memory
            DetectableObject closestRabbit = memory.GetClosestItem(EDetectableObjectCategories.RABBIT, Agent.transform);

            // If none were found or the rabbit is hidding, don't continue
            if (closestRabbit == null || closestRabbit.GetComponent<Rabbit>().combat.isHidden) {
                return false;
            }

            // Check if the closest rabbit is in view of the fox
            if (memory.ObjectIsInView(closestRabbit)) {
                // If so, set the foxes target distination to their position
                blackboard.targetObject = closestRabbit.gameObject;
                blackboard.targetLocation = closestRabbit.transform.position;
                blackboard.targetObject.GetComponent<Rabbit>().combat.MarkAsPrey(true);
                return true;
            }

            // Otherwise, get the last known position of the rabbit
            Vector3 targetPos = memory.GetObjectsLastKnownLocation(closestRabbit);
            if(targetPos == Vector3.zero) { 
                // If no last known position was found, don't continue
                return false;
            }
            blackboard.targetLocation = targetPos;
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