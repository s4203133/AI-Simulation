using System.Collections.Generic;
using System.Linq;

namespace TheKiwiCoder {
    public class Parallel : CompositeNode {
        List<State> childrenLeftToExecute = new List<State>();
        bool stillRunning = false;
        bool childFailed;
        bool lastChild;
        bool nodeComplete;
        State childStatus;

        protected override void OnStart() {
            blackboard.nodeStack.PushNode(this);
            childrenLeftToExecute.Clear();
            children.ForEach(a => {
                childrenLeftToExecute.Add(State.Running);
            });
        }

        protected override void OnStop() {
            blackboard.nodeStack.PopNode(this);
        }

        protected override State OnUpdate() {
            stillRunning = false;
            for (int i = 0; i < childrenLeftToExecute.Count(); ++i) {
                if (nodeComplete) {
                    lastChild = false;
                    nodeComplete = false;
                    return State.Success;
                }
                if (i == childrenLeftToExecute.Count() - 1) {
                    lastChild = true;
                }

                if (childrenLeftToExecute[i] == State.Running) {
                    childStatus = children[i].Update();

                    if (childStatus == State.Failure || childFailed) {
                        childFailed = false;
                        AbortRunningChildren();
                        return State.Failure;
                    }

                    if (childStatus == State.Running) {
                        stillRunning = true;
                    }

                    childrenLeftToExecute[i] = childStatus;
                }
            }

            return stillRunning ? State.Running : State.Success;
        }

        void AbortRunningChildren() {
            for (int i = 0; i < childrenLeftToExecute.Count(); ++i) {
                if (childrenLeftToExecute[i] == State.Running) {
                    children[i].Abort();
                }
            }
        }

        public override void NotifyOfFailure() {
            childStatus = State.Failure;
            childFailed = true;
        }

        public override void NotifyOfSuccess() {
            childStatus = State.Success;
            stillRunning = false;
            if (lastChild) {
                nodeComplete = true;
            }
        }
    }
}