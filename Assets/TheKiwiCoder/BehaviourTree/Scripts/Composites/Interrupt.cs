using TheKiwiCoder;

/// <summary>
/// Interrupt Nodes are called first before the currently running node, and if any succeed, they overwrite the currently running node and its child becomes the new active node in the tree.
/// </summary>
public class Interrupt : CompositeNode
{
    private State childState;

    private void Awake() {
        blackboard.nodeStack.interrupts.Add(this);
    }

    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        for(int i = 0; i < children.Count; i++) {
            childState = children[i].Update();
            if(childState == State.Success) {
                return childState;
            }
        }

        return childState;
    }

    public override void NotifyOfFailure() {
        childState = State.Failure;
    }

    public override void NotifyOfSuccess() {
        childState = State.Success;
    }
}
