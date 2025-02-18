using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace TheKiwiCoder {
    [CreateAssetMenu()]
    public class BehaviourTree : ScriptableObject {
        public Node rootNode;
        public Node.State treeState = Node.State.Running;
        public List<Node> nodes = new List<Node>();
        public Blackboard blackboard = new Blackboard();

        public Node.State Update() {
            // Get the currently running node in the stack
            Node currentNode = blackboard.nodeStack.CurrentNode();

            // If the root node is still running, continue to update the currently active node
            if (rootNode.state == Node.State.Running) {
                treeState = currentNode.Update();
            } else {
                // If the root returns success or failure, then the tree has completed a cylce, so restart
                ResetTree();
            }
            return treeState;
        }

        /// <summary>
        /// Empty the stack ready for a new set of nodes to be added next iteration
        /// </summary>
        public void ResetTree() {
            for (int i = 0; i < nodes.Count; i++) {
                nodes[i].state = Node.State.Running;
                blackboard.nodeStack.ClearStack();
            }
        }

        public static List<Node> GetChildren(Node parent) {
            List<Node> children = new List<Node>();

            if (parent is DecoratorNode decorator && decorator.child != null) {
                children.Add(decorator.child);
            }

            if (parent is RootNode rootNode && rootNode.child != null) {
                children.Add(rootNode.child);
            }

            if (parent is CompositeNode composite) {
                return composite.children;
            }

            return children;
        }

        public static void AssignChildrensParent(Node parent) {
            if (parent is DecoratorNode decorator && decorator.child != null) {
                decorator.parentNode = parent;
            }

            if (parent is RootNode rootNode && rootNode.child != null) {
                rootNode.parentNode = parent;
            }

            if (parent is CompositeNode composite && composite.children.Count > 0) {
                foreach (Node child in composite.children) {
                    child.parentNode = parent;
                }
            }
        }

        public static void Traverse(Node node, System.Action<Node> visiter) {
            if (node) {
                visiter.Invoke(node);
                var children = GetChildren(node);
                AssignChildrensParent(node);
                children.ForEach((n) => Traverse(n, visiter));
            }
        }

        public BehaviourTree Clone() {
            BehaviourTree tree = Instantiate(this);
            tree.rootNode = tree.rootNode.Clone();
            tree.nodes = new List<Node>();
            Traverse(tree.rootNode, (n) => {
                tree.nodes.Add(n);
            });

            return tree;
        }

        public void Bind(Context context) {
            Traverse(rootNode, node => {
                node.context = context;
                node.blackboard = blackboard;
            });
        }


        #region Editor Compatibility
#if UNITY_EDITOR

        public Node CreateNode(System.Type type) {
            Node node = ScriptableObject.CreateInstance(type) as Node;
            node.name = type.Name;
            node.guid = GUID.Generate().ToString();

            Undo.RecordObject(this, "Behaviour Tree (CreateNode)");
            nodes.Add(node);

            if (!Application.isPlaying) {
                AssetDatabase.AddObjectToAsset(node, this);
            }

            Undo.RegisterCreatedObjectUndo(node, "Behaviour Tree (CreateNode)");

            AssetDatabase.SaveAssets();
            return node;
        }

        public void DeleteNode(Node node) {
            Undo.RecordObject(this, "Behaviour Tree (DeleteNode)");
            nodes.Remove(node);

            //AssetDatabase.RemoveObjectFromAsset(node);
            Undo.DestroyObjectImmediate(node);

            AssetDatabase.SaveAssets();
        }

        public void AddChild(Node parent, Node child) {
            if (parent is DecoratorNode decorator) {
                Undo.RecordObject(decorator, "Behaviour Tree (AddChild)");
                decorator.child = child;
                child.parentNode = decorator;
                EditorUtility.SetDirty(decorator);
            }

            if (parent is RootNode rootNode) {
                Undo.RecordObject(rootNode, "Behaviour Tree (AddChild)");
                rootNode.child = child;
                child.parentNode = rootNode;
                EditorUtility.SetDirty(rootNode);
            }

            if (parent is CompositeNode composite) {
                Undo.RecordObject(composite, "Behaviour Tree (AddChild)");
                composite.children.Add(child);
                child.parentNode = composite;
                EditorUtility.SetDirty(composite);
            }
        }

        public void RemoveChild(Node parent, Node child) {
            if (parent is DecoratorNode decorator) {
                Undo.RecordObject(decorator, "Behaviour Tree (RemoveChild)");
                decorator.child = null;
                child.parentNode = null;
                EditorUtility.SetDirty(decorator);
            }

            if (parent is RootNode rootNode) {
                Undo.RecordObject(rootNode, "Behaviour Tree (RemoveChild)");
                rootNode.child = null;
                child.parentNode = null;
                EditorUtility.SetDirty(rootNode);
            }

            if (parent is CompositeNode composite) {
                Undo.RecordObject(composite, "Behaviour Tree (RemoveChild)");
                composite.children.Remove(child);
                child.parentNode = null;
                EditorUtility.SetDirty(composite);
            }
        }
#endif
        #endregion Editor Compatibility
    }
}