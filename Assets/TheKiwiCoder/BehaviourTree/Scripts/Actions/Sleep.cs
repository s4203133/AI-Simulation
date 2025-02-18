using UnityEngine;
using TheKiwiCoder;

public class Sleep : ActionNode
{
    [Space(15)]
    public float restAmount;
    [SerializeField] private GameObject sleepParticlesPrefab;
    private ParticleSystem sleepParticles;

    protected override void OnStart() {
        context.aiAgent.stats.currentAction = actionName;
        blackboard.nodeStack.PushNode(this);

        context.agent.isStopped = true;
        context.animator.SetBool("Moving", false);

        context.aiAgent.tiredness.isResting = true;

        sleepParticles = Instantiate(sleepParticlesPrefab, context.gameObject.transform.position + Vector3.up, Quaternion.identity).GetComponent<ParticleSystem>();
    }

    protected override void OnStop() {
        context.aiAgent.tiredness.isResting = false;
        sleepParticles.Stop();

        blackboard.nodeStack.PopNode();
    }

    protected override State OnUpdate() {
        if(!context.aiAgent.tiredness.isResting) {
            return State.Success;
        }
        return State.Running;
    }
}
