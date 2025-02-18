using UnityEngine;

namespace TheKiwiCoder {
    public class BehaviourTreeRunner : MonoBehaviour {

        // The main behaviour tree asset
        public BehaviourTree tree;
        public GameObject targetPosition;

        // Storage container object to hold game object subsystems
        Context context;

        // Start is called before the first frame update
        void Start() {
            context = CreateBehaviourTreeContext();
            tree = tree.Clone();
            tree.Bind(context);
            tree.blackboard.nodeStack.PushNode(tree.rootNode);
            tree.rootNode.state = Node.State.Running;
        }

        // Update is called once per frame
        void Update() {
            if (tree) {
                tree.Update();
            }

            if (Input.GetKeyDown(KeyCode.Space)) {
                Debug.Log("CURRENT NODES IN STACK:");
                for (int i = 0; i < tree.blackboard.nodeStack.runningNodes.Count; i++) {
                    Debug.Log(tree.blackboard.nodeStack.runningNodes[i].name);
                }
            }
        }

        Context CreateBehaviourTreeContext() {
            return Context.CreateFromGameObject(gameObject);
        }

        private void OnDrawGizmosSelected() {
            if (!tree) {
                return;
            }

            BehaviourTree.Traverse(tree.rootNode, (n) => {
                if (n.drawGizmos) {
                    n.OnDrawGizmos();
                }
            });
        }
    }
}