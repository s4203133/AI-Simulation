using System.Collections.Generic;
using TheKiwiCoder;

public class NodeStack {

    public List<Node> runningNodes = new List<Node>();
    public List<Interrupt> interrupts = new List<Interrupt>();
    private int numberOfNodes = 0;

    public Node CurrentNode() {
        return runningNodes[0];
    }

    public Node ParentNode() {
        // If there is only one node (root node), then just return that
        if (runningNodes.Count == 1) {
            return runningNodes[0];
        }
        // Otherwise, return the next node up (the parent node)
        return runningNodes[1];
    }

    public void PushNode(Node node) {
        runningNodes.Insert(0, node);
    }

    public void PopNode() {
        if (runningNodes.Count > 1) {
            runningNodes.RemoveAt(0);
        }
    }

    public void PopNode(Node node) {
        runningNodes.Remove(node);
    }

    public void ClearStack() {
        numberOfNodes = runningNodes.Count;
        if (numberOfNodes == 1) {
            return;
        }
        for(int i = 1; i < numberOfNodes; i++) {
            runningNodes.RemoveAt(i);
        }
    }
}
