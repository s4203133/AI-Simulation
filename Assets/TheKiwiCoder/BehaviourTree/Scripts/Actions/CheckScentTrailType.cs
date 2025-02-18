using TheKiwiCoder;
using UnityEngine;

public class CheckScentTrailType : ActionNode
{
    [Space(15)]
    public EDetectableObjectCategories scentType;

    protected override void OnStart() {
        context.aiAgent.stats.currentAction = actionName;
        blackboard.nodeStack.PushNode(this);
    }

    protected override void OnStop() {
        blackboard.nodeStack.PopNode(this);
    }

    protected override State OnUpdate() {
        if(context.aiAgent.sensorySystem.senseOfSmell.ScentTrailIsOfType(scentType)) {
            blackboard.trailToFollow = context.aiAgent.sensorySystem.senseOfSmell.currentTrailFollowing;
            return State.Success;
        }
        return State.Failure;
    }
}
