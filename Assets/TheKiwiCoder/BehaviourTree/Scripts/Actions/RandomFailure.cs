using UnityEngine;

namespace TheKiwiCoder {
    public class RandomFailure : ActionNode {

        [Range(0, 1)]
        public float chanceOfFailure = 0.5f;

        protected override void OnStart() {
            context.aiAgent.stats.currentAction = actionName;
            blackboard.nodeStack.PushNode(this);
        }

        protected override void OnStop() {
            blackboard.nodeStack.PopNode(this);
        }

        protected override State OnUpdate() {
            float value = Random.value;
            if (value > chanceOfFailure) {
                return State.Failure;
            }
            return State.Success;
        }
    }
}