using UnityEngine;
using TheKiwiCoder;

public class RandomPosition : ActionNode {
    public Vector2 min = Vector2.one * -10;
    public Vector2 max = Vector2.one * 10;

    protected override void OnStart() {
        context.aiAgent.stats.currentAction = actionName;
        blackboard.nodeStack.PushNode(this);
    }

    protected override void OnStop() {
        blackboard.nodeStack.PopNode();
    }

    protected override State OnUpdate() {
        blackboard.moveToPosition.x = Random.Range(min.x, max.x);
        blackboard.moveToPosition.z = Random.Range(min.y, max.y);
        return State.Success;
    }
}
