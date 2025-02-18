using System.Collections.Generic;
using UnityEngine;

namespace GOAP {

    public class Sleep : GoapAction, IAction {

        [SerializeField] private GameObject sleepParticlesPrefab;
        private ParticleSystem sleepParticles;

        string IAction.ActionName() => actionName;

        float IAction.Duration() => duration;

        int IAction.CalculateCost() => cost;

        bool IAction.IsAchievable(GameObject Agent) {
            Initialise(Agent);
            return true;
        }

        bool IAction.StartAction(GameObject Agent) {
            // Stop the nav mesh from moving
            navMeshAgent.isStopped = true;
            animator.SetBool("Moving", false);
            // Remove the fox from other agents memory so rabbits won't try to flee sleeping foxes
            blackboard.isSleeping = true;

            sleepParticles = Instantiate(sleepParticlesPrefab, agent.transform.position + Vector3.up, Quaternion.identity).GetComponent<ParticleSystem>();
            isRunning = true;
            return true;
        }

        bool IAction.UpdateAction(float deltaTime) {
            return true;
        }

        void IAction.ComleteAction() {

        }

        void IAction.StopAction() {
            // Reset variables
            sleepParticles.Stop();
            navMeshAgent.isStopped = false;
            aiAgent.tiredness.ResetValue();
            blackboard.isSleeping = false;
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
