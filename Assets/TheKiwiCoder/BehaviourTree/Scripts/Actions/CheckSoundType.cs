using TheKiwiCoder;
using UnityEngine;

public class CheckSoundType : ActionNode
{
    [Space(15)]
    public ESoundCategories soundType;

    protected override void OnStart() {
        context.aiAgent.stats.currentAction = actionName;
        blackboard.nodeStack.PushNode(this);
    }

    protected override void OnStop() {
        blackboard.nodeStack.PopNode(this);
    }

    protected override State OnUpdate() {
        if(context.aiAgent.sensorySystem.HasHeardSoundOfType(soundType)) {
            return State.Success;
        }
        return State.Failure;
    }
}
