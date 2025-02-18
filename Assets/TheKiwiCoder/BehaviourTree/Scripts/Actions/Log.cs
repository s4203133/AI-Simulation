using UnityEngine;

namespace TheKiwiCoder {
    public class Log : ActionNode {
        public string message;

        protected override void OnStart() {
            context.aiAgent.stats.currentAction = actionName;
            blackboard.nodeStack.PushNode(this);
        }

        protected override void OnStop() {
            blackboard.nodeStack.PopNode(this);
        }

        protected override State OnUpdate() {
            Debug.Log($"{message}");
            return State.Success;
        }
    }
}