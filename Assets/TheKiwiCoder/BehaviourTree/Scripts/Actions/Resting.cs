using UnityEngine;
using TheKiwiCoder;

public class Resting : ActionNode
{
    public float duration = 1;
    float startTime;

    protected override void OnStart() {
        context.aiAgent.IsUsingStamina(true);
        context.aiAgent.stats.currentAction = actionName;
        blackboard.nodeStack.PushNode(this);
        startTime = Time.time;
    }

    protected override void OnStop() {
        context.aiAgent.RefillStamina();
        blackboard.nodeStack.PopNode(this);
    }

    protected override State OnUpdate() {
        //context.aiAgent.IsUsingStamina(true);
        if (Time.time - startTime >= duration) {
            context.aiAgent.IsUsingStamina(false);
            context.aiAgent.RefillStamina();
            return State.Success;
        }
        return State.Running;
    }
}
