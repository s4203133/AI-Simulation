using System.Collections.Generic;

namespace TheKiwiCoder {
    public abstract class CompositeNode : Node {
        public List<Node> children = new List<Node>();

        public override Node Clone() {
            CompositeNode node = Instantiate(this);
            node.children = children.ConvertAll(c => c.Clone());
            return node;
        }

        public abstract void NotifyOfSuccess();

        public abstract void NotifyOfFailure();
    }
}