using TheKiwiCoder;

public class IsHiding : ActionNode {
    protected override void OnStart() {
        context.aiAgent.stats.currentAction = actionName;
        blackboard.nodeStack.PushNode(this);
    }

    protected override void OnStop() {
        blackboard.nodeStack.PopNode(this);
    }

    protected override State OnUpdate() {
        if (blackboard.isHiding) {
            return State.Success;
        }
        return State.Failure;
    }
}
