using UnityEngine;
using TheKiwiCoder;

public class Breakpoint : ActionNode {
    protected override void OnStart() {
        Debug.Log("Trigging Breakpoint");
        Debug.Break();
        blackboard.nodeStack.PushNode(this);
    }

    protected override void OnStop() {
        blackboard.nodeStack.PopNode(this);
    }

    protected override State OnUpdate() {
        return State.Success;
    }
}
