using UnityEngine;

namespace TheKiwiCoder {
    public class RandomSelector : CompositeNode {
        protected int current;

        protected override void OnStart() {
            blackboard.nodeStack.PushNode(this);
            current = Random.Range(0, children.Count);
        }

        protected override void OnStop() {
            blackboard.nodeStack.PopNode(this);
        }

        protected override State OnUpdate() {
            var child = children[current];
            return child.Update();
        }

        public override void NotifyOfFailure() {

        }

        public override void NotifyOfSuccess() {

        }
    }
}