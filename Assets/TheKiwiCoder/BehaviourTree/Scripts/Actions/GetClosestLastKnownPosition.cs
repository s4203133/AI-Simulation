using UnityEngine;
using TheKiwiCoder;

public class GetClosestLastKnownPosition : ActionNode
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
        if (visibleTarget == null) {
            return State.Failure;
        }

        Vector3 lastKnownLocation = context.memory.GetObjectsLastKnownLocation(visibleTarget);
        if (lastKnownLocation == Vector3.zero) {
            return State.Failure;
        }
        blackboard.moveToPosition = lastKnownLocation;
        blackboard.target = null;
        return State.Success;
    }
}
