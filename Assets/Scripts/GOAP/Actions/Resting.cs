using System.Collections.Generic;
using UnityEngine;


namespace GOAP {
    public class Resting : GoapAction, IAction {

        [SerializeField] private float restDuration;
        private float timer;

        string IAction.ActionName() => actionName;

        float IAction.Duration() => duration;

        int IAction.CalculateCost() => cost;

        bool IAction.IsAchievable(GameObject Agent) {
            Initialise(Agent);
            return aiAgent.stamina <= 0.01;
        }

        bool IAction.StartAction(GameObject Agent) {
            // Continue stamina to be used so that this action doesn't get cancelled, and stop the agents movement
            aiAgent.IsUsingStamina(true);
            navMeshAgent.isStopped = true;
            timer = restDuration;
            isRunning = true;
            return true;
        }

        bool IAction.UpdateAction(float deltaTime) {
            // Countdown timer until the action is finnished
            timer -= deltaTime;
            return true;
        }

        void IAction.ComleteAction() {
            // Reset stats
            aiAgent.RefillStamina();
            aiAgent.IsUsingStamina(false);
        }

        void IAction.StopAction() {
            navMeshAgent.isStopped = false;
            isRunning = false;
        }

        List<Condition> IAction.GetPreConditions() => preConditions;

        List<Condition> IAction.GetEffects() => postConditions;

        bool IAction.Running() => isRunning;

        bool IAction.WithinRange() {
            // Once the timer has decreased to 0, finish the action
            if(timer <= 0) {
                return true;
            }
            return false;
        }
    }

}
