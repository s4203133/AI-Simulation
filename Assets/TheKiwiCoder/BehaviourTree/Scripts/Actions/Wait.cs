using UnityEngine;

namespace TheKiwiCoder {
    public class Wait : ActionNode {
        public float duration = 1;
        float startTime;

        protected override void OnStart() {
            context.aiAgent.stats.currentAction = actionName;
            blackboard.nodeStack.PushNode(this);
            startTime = Time.time;
        }

        protected override void OnStop() {
            blackboard.nodeStack.PopNode();
        }

        protected override State OnUpdate() {
            if (blackboard.feelsThreatened) {
                return State.Failure;
            }
            if (Time.time - startTime >= duration) {
                return State.Success;
            }
            return State.Running;
        }
    }
}
