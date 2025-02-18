using TheKiwiCoder;

public class IsTired : ActionNode
{
    protected override void OnStart() {
        context.aiAgent.stats.currentAction = actionName;
        blackboard.nodeStack.PushNode(this);
    }

    protected override void OnStop() {
        blackboard.nodeStack.PopNode();
    }

    protected override State OnUpdate() {
        if (blackboard.isTired) {
            return State.Success;
        }
        return State.Failure;
    }
}
