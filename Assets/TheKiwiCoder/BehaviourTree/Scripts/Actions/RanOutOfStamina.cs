using TheKiwiCoder;

public class RanOutOfStamina : ActionNode
{
    protected override void OnStart() {
        context.aiAgent.stats.currentAction = actionName;
        blackboard.nodeStack.PushNode(this);
    }

    protected override void OnStop() {
        blackboard.nodeStack.PopNode();
    }

    protected override State OnUpdate() {
        if (context.aiAgent.stamina <= 0.05) {
            return State.Success;
        }
        return State.Failure;
    }
}
