using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GOAP {

    public class Reproduce : GoapAction, IAction {

        string IAction.ActionName() => actionName;

        float IAction.Duration() => duration;

        int IAction.CalculateCost() => cost;

        bool IAction.IsAchievable(GameObject Agent) {
            Initialise(Agent);
            // Check that a fox exists in memory
            return memory.ItemExistsInMemory(EDetectableObjectCategories.FOX);
        }

        bool IAction.StartAction(GameObject Agent) {
            // If there's no target object, don't continue
            if(blackboard.targetObject == null) {
                return false;
            }

            // If the target object is already pregnant, don't continue
            if (blackboard.targetObject.GetComponent<Pregnancy>().isPregnant) {
                blackboard.targetObject = null;
                return false;
            }

            // If the target partner has rejected this agent in the past, don't continue
            Fox otherFox = blackboard.targetObject.GetComponent<Fox>();
            if (aiAgent.reproduction.HasBeenRejectedByThisAgent(otherFox)) {
                blackboard.targetObject = null;
                return false;
            }

            // If the target partner rejected this agent now, don't continue
            if (!otherFox.reproduction.EvalatePartner(aiAgent)) {
                aiAgent.reproduction.Rejection(otherFox);
                blackboard.targetObject = null;
                return false;
            }

            // Otherwise, face towards the found partner, and begin reproduction
            navMeshAgent.isStopped = true;
            agent.transform.LookAt(blackboard.targetObject.transform.position);
            aiAgent.reproduction.SetReproductionPartner(blackboard.targetObject);

            // Set the other agents reproduction variables
            blackboard.targetObject.GetComponent<NavMeshAgent>().isStopped = true;
            blackboard.targetObject.transform.LookAt(agent.transform.position);
            otherFox.reproduction.SetReproductionPartner(agent);

            isRunning = true;
            return true;
        }

        bool IAction.UpdateAction(float deltaTime) {
            // If the agent no longer exists, cancel the action
            if (blackboard.targetObject == null) {
                return false;
            }
            return true;
        }

        void IAction.ComleteAction() {
            // Reset the reproduction values on the agents
            aiAgent.reproduction.ResetValue();
            blackboard.targetObject.GetComponent<AIAgent>().reproduction.ResetValue();
        }

        void IAction.StopAction() {
            if(blackboard.targetObject == null) {
                return;
            }
            // Allow the agent to move again
            navMeshAgent.isStopped = false;
            aiAgent.reproduction.SetReproductionPartner(null);

            // If the partner does not exist anymore, pregnate this agent
            agent.GetComponent<Pregnancy>().Pregnate(CreatureType.fox, blackboard.targetObject.GetComponent<AIAgent>());

            // Allow the partner agent to move again
            blackboard.targetObject.GetComponent<NavMeshAgent>().isStopped = false;
            blackboard.targetObject.GetComponent<Fox>().reproduction.SetReproductionPartner(null);

            // Choose a random partner to pregnate
            float chance = Random.Range(0f, 1f);
            if(chance <= 0.5f) {
                agent.GetComponent<Pregnancy>().Pregnate(CreatureType.fox, blackboard.targetObject.GetComponent<AIAgent>());
            } else {
                blackboard.targetObject.GetComponent<Pregnancy>().Pregnate(CreatureType.fox, aiAgent);
            }

            // Reset the target object
            blackboard.targetObject = null;

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
