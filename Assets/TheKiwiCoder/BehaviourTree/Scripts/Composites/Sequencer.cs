
namespace TheKiwiCoder {
    public class Sequencer : CompositeNode {
        protected int current;

        protected override void OnStart() {
            blackboard.nodeStack.PushNode(this);
            current = 0;
        }

        protected override void OnStop() {
            blackboard.nodeStack.PopNode();
        }

        protected override State OnUpdate() {
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

        public override void NotifyOfSuccess() {
            current++;
        }

        public override void NotifyOfFailure() {
            current = children.Count;
        }
    }
}