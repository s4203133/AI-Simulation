
namespace TheKiwiCoder {

    public class RootNode : Node {
        public Node child;

        protected override void OnStart() {
            blackboard.nodeStack.PushNode(this);
        }

        protected override void OnStop() {

        }

        protected override State OnUpdate() {
            State returnState = child.Update();
            return returnState;
        }

        public override Node Clone() {
            RootNode node = Instantiate(this);
            node.child = child.Clone();
            return node;
        }
    }
}