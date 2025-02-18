using UnityEngine;
using TheKiwiCoder;

public class EatFood : ActionNode
{
    [Space(15)]
    public ParticleSystem eatingParticlePrefab;
    private ParticleSystem eatingParticles;
    public ParticleSystem disapearPrefab;

    public float duration;
    float startTime;

    protected override void OnStart() {
        context.aiAgent.stats.currentAction = actionName;
        blackboard.nodeStack.PushNode(this);
        startTime = Time.time;
    }

    protected override void OnStop() {
        eatingParticles = null;
        blackboard.nodeStack.PopNode();
    }

    protected override State OnUpdate() {
        if(blackboard.target == null || !blackboard.target.activeInHierarchy) {
            return State.Failure;
        }

        if(eatingParticles == null) {
            SoundManager.PlaySound(SoundManager.instance.eating, context.gameObject);
            eatingParticles = Instantiate(eatingParticlePrefab, blackboard.target.transform.position, Quaternion.identity);
        }
        
        if (Time.time - startTime >= duration) {
            Food foodEaten = blackboard.target.GetComponent<Food>();
            if (foodEaten != null) {
                context.aiAgent.hunger.RestoreHunger(foodEaten.hungerToRestore);
                context.aiAgent.health.RestoreHealth(foodEaten.healthToRestore);
                SoundManager.PlaySound(SoundManager.instance.agentEating, context.gameObject);
                Instantiate(disapearPrefab, blackboard.target.transform.position, Quaternion.identity);
                foodEaten.Eat();
            } else {
                return State.Failure;
            }
            return State.Success;
        }
        return State.Running;
    }
}
