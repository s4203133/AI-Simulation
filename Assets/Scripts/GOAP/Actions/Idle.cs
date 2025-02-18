using System.Collections.Generic;
using UnityEngine;


namespace GOAP {
    public class Idle : GoapAction, IAction {

        string IAction.ActionName() => actionName;

        float IAction.Duration() => duration;

        int IAction.CalculateCost() => cost;

        bool IAction.IsAchievable(GameObject Agent) {
            Initialise(Agent);
            return true;
        }

        bool IAction.StartAction(GameObject Agent) {
            // Stop the agent from moving
            navMeshAgent.isStopped = true;
            isRunning = true;
            animator.SetBool("Moving", false);
            return true;
        }

        bool IAction.UpdateAction(float deltaTime) {
            return true;
        }

        void IAction.ComleteAction() {

        }

        void IAction.StopAction() {
            navMeshAgent.isStopped = false;
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

