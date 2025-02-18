using UnityEngine;

namespace TheKiwiCoder {
    public abstract class Node : ScriptableObject {
        public enum State {
            Running,
            Failure,
            Success
        }

        public State state = State.Running;
        [HideInInspector] public bool started = false;
        [HideInInspector] public string guid;
        [HideInInspector] public Vector2 position;
        [HideInInspector] public Context context;
        [HideInInspector] public Blackboard blackboard;
        [TextArea] public string description;
        public bool drawGizmos = false;

        public enum EventTypes {
            start,
            end,
            success,
            failure
        }

        [Space(15)]
        public bool pauseOnNodeEvent;
        public EventTypes pauseEventType;

        [Space(15)]
        public Node parentNode;

        public State Update() {

            if (!started) {
                Pause(EventTypes.start);
                OnStart();
                started = true;
            }

            state = OnUpdate();

            if (state != State.Running) {
                if (state == State.Success) {
                    Pause(EventTypes.success);
                } else if (state == State.Failure) {
                    Pause(EventTypes.failure);
                }
                NotifyParentOfResult();

                Pause(EventTypes.end);
                OnStop();
                started = false;
            }

            return state;
        }

        public virtual Node Clone() {
            return Instantiate(this);
        }

        public void Abort() {
            BehaviourTree.Traverse(this, (node) => {
                node.started = false;
                node.state = State.Running;
                node.OnStop();
            });
        }

        public void UpdateParentOfState() {
            if (parentNode is CompositeNode) {

            }
        }

        // When the result of a node has been determined, notify the parent node of what is was
        protected void NotifyParentOfResult() {
            if (parentNode is CompositeNode compositeNode) {
                if (state == State.Success)
                    compositeNode.NotifyOfSuccess();
                else if (state == State.Failure)
                    compositeNode.NotifyOfFailure();
            }
        }

        public Node CheckParentsForInterrupt() {
            Node currentNode = null;
            if(this is RootNode) {
                return this;
            }
            while(currentNode is not Interrupt || currentNode is not RootNode) {
                currentNode = currentNode.parentNode;
            }
            return currentNode;
        }

        void Pause(EventTypes type) {
            if (pauseOnNodeEvent) {
                if (pauseEventType == EventTypes.start && type == EventTypes.start) {
                    Debug.Log($"Pausing {description} On Start");
                } else if (pauseEventType == EventTypes.end && type == EventTypes.end) {
                    Debug.Log($"Pausing {description} On End");
                } else if (pauseEventType == EventTypes.success && type == EventTypes.success) {
                    Debug.Log($"Pausing {description} On Success");
                } else if (pauseEventType == EventTypes.failure && type == EventTypes.failure) {
                    Debug.Log($"Pausing {description} On Failure");
                }
                //Debug.Break();
            }
        }

        public virtual void OnDrawGizmos() { }
        protected abstract void OnStart();
        protected abstract void OnStop();
        protected abstract State OnUpdate();
    }
}