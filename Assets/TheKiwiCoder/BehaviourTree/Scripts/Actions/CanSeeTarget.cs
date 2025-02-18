using UnityEngine;
using TheKiwiCoder;

public class CanSeeTarget : ActionNode
{
    [Space(15)]
    public EDetectableObjectCategories targetObjects;

    protected override void OnStart() {
        context.aiAgent.stats.currentAction = actionName;
        blackboard.nodeStack.PushNode(this);
    }

    protected override void OnStop() {
        blackboard.nodeStack.PopNode(this);
    }

    protected override State OnUpdate() {
        DetectableObject visibleTarget = context.memory.GetClosestItem(targetObjects, context.transform);
        if(visibleTarget == null) {
            return State.Failure;
        }
        blackboard.moveToPosition = visibleTarget.transform.position;
        blackboard.target = visibleTarget.gameObject;
        return State.Success;
    }
}
