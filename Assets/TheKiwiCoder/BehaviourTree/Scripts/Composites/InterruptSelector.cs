
namespace TheKiwiCoder {
    public class InterruptSelector : Selector {

        protected override void OnStart() {
            blackboard.nodeStack.PushNode(this);
        }

        protected override void OnStop() {
            blackboard.nodeStack.PopNode(this);
        }

        protected override State OnUpdate() {
            int previous = current;
            base.OnStart();
            var status = base.OnUpdate();
            if (previous != current) {
                if (children[previous].state == State.Running) {
                    children[previous].Abort();
                }
            }

            return status;
        }
    }
}