using System.Collections.Generic;
using UnityEngine;


namespace GOAP {
    public class EatRabbit : GoapAction, IAction {

        [SerializeField] private ParticleSystem damageParticles;
        private ParticleSystem particles;

        string IAction.ActionName() => actionName;

        float IAction.Duration() => duration;

        int IAction.CalculateCost() => cost;

        bool IAction.IsAchievable(GameObject Agent) {
            Initialise(Agent);
            if (!aiAgent.hunger.isHungry) { return false; }
            return true;
        }

        bool IAction.StartAction(GameObject Agent) {
            // If there's no target, don't continue
            if (blackboard.targetObject == null) {
                return false;
            }

            // Face the agent towards their target
            agent.transform.LookAt(blackboard.targetObject.transform.position);
            HearingManager.instance.EmitSound(agent.transform.position, soundToPlay, 2, agent);
            isRunning = true;

            return true;
        }

        bool IAction.UpdateAction(float deltaTime) {
            return true;
        }

        void IAction.ComleteAction() {
            // If the target object doesn't exist anymore, don't continue
            if(blackboard.targetObject == null) {
                return;
            }

            // Play an animation and sound effect
            animator.SetTrigger("Attack");
            SoundManager.PlaySound(SoundManager.instance.FoxAttack, agent);

            // Get the target rabbit
            AIAgent rabbit = blackboard.targetObject.GetComponent<AIAgent>();
            rabbit.combat.MarkAsPrey(false);

            // If the rabbit is aware of the fox, and isn't resting, give them a chance to dodge the foxes attack
            if (rabbit.combat.feelsThreatened && rabbit.stamina > 0.1 && !rabbit.tiredness.isResting) {
                bool attackFailed = rabbit.combat.WillAgentDodge(aiAgent);
                if (attackFailed) {
                    blackboard.targetObject = null;
                    return;
                }
            }
            // If the rabbit wasn't aware of the fox, or the rabbits dodge failed, then the foxes attack is successful
            particles = Instantiate(damageParticles, blackboard.targetObject.transform.position, Quaternion.identity);
            particles.Pause();
            Invoke("Consume", 0.65f);
        }

        void IAction.StopAction() {
            HearingManager.instance.EmitSound(agent.transform.position, soundToPlay, 2, agent);
            isRunning = false;
        }

        List<Condition> IAction.GetPreConditions() => preConditions;

        List<Condition> IAction.GetEffects() => postConditions;

        bool IAction.Running() => isRunning;

        bool IAction.WithinRange() {
            return true;
        }

        private void Consume() {
            // Play a sound and particle effect
            SoundManager.PlaySound(SoundManager.instance.rabbitKilled, agent);
            particles.Play();
            // Destroy the rabbit if it hasn't been already
            if (blackboard.targetObject != null) {
                Destroy(blackboard.targetObject);
            }
            // Restore hunger, health, and increment kills
            aiAgent.hunger.RestoreHunger(0.5f);
            aiAgent.health.RestoreHealth(50f);
            aiAgent.combat.kills++;
        }
    }

}

