using UnityEngine;
using TheKiwiCoder;

public class ClosestTargetInMemory : ActionNode
{
    [Space(15)]
    [SerializeField] private EDetectableObjectCategories objectType;

    protected override void OnStart() {
        context.aiAgent.stats.currentAction = actionName;
        blackboard.nodeStack.PushNode(this);
    }

    protected override void OnStop() {
        blackboard.nodeStack.PopNode();
    }

    protected override State OnUpdate() {
        DetectableObject foundTarget = context.memory.GetClosestItem(objectType, context.transform);

        if(foundTarget == null) {
            return State.Failure;
        }

        blackboard.moveToPosition = foundTarget.transform.position;
        blackboard.target = foundTarget.gameObject;

        return State.Success;
    }
}
