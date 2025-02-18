
public class CompareTiredness : CompareFloat
{
    protected override void OnStart() {
        context.aiAgent.stats.currentAction = actionName;
        blackboard.nodeStack.PushNode(this);
    }

    protected override void OnStop() {
        blackboard.nodeStack.PopNode();
    }

    protected override State OnUpdate() {
        //return Compare(context.tiredValue);
        return State.Failure;
    }
}
