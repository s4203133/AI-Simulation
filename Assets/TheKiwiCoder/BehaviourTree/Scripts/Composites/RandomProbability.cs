using UnityEngine;
using TheKiwiCoder;

public class RandomProbability : CompositeNode
{
    public int percentageChance;
    private bool successful;

    protected int current;

    protected override void OnStart() {
        blackboard.nodeStack.PushNode(this);
        current = 0;
        int chance = Random.Range(0, 100);
        if(chance <= percentageChance) {
            successful = true;
        }
    }

    protected override void OnStop() {
        blackboard.nodeStack.PopNode();
    }

    protected override State OnUpdate() {
        if(!successful) {
            return State.Failure;
        }

        for (int i = current; i < children.Count; ++i) {
            current = i;
            var child = children[current];

            switch (child.Update()) {
                case State.Running:
                    return State.Running;
                case State.Failure:
                    return State.Failure;
                case State.Success:
                    child.started = false;
                    continue;
            }
        }

        return State.Success;
    }

    public override void NotifyOfFailure() {
        current++;
    }

    public override void NotifyOfSuccess() {
        current = children.Count;
    }
}
