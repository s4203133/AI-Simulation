using UnityEngine;
using TheKiwiCoder;
using UnityEngine.AI;

public class ChooseBestTarget : ActionNode
{
    public EDetectableObjectCategories objectType;
    Vector3 postion => context.transform.position;
    //GameObject closestVisibleObject => context.memory.GetClosestItem(objectType, context.transform);
    //GameObject closestObjectFromMemory => context.sensorySystem.closestObjectFromMemory;
    protected override void OnStart() {
        context.aiAgent.stats.currentAction = actionName;
        blackboard.nodeStack.PushNode(this);
    }

    protected override void OnStop() {
        blackboard.nodeStack.PopNode();
    }

    protected override State OnUpdate() {
        // If one of the targets is null, choose the other one instead
        /*if(context.sensorySystem.closestVisibleObject == null) {
            SetTarget(context.sensorySystem.closestObjectFromMemory);
            return State.Success;
        }
        if (context.sensorySystem.closestObjectFromMemory == null) {
            SetTarget(context.sensorySystem.closestVisibleObject);
            return State.Success;
        }

        // Out of both targets, calculate which one has the shortest path to get to 
        float distToClosestVisibleObj = GetPathLength(closestVisibleObject.transform.position);
        float distToClosestMemoryObj = GetPathLength(closestObjectFromMemory.transform.position);

        // Choose the closest object to be the new target
        if (distToClosestVisibleObj <= distToClosestMemoryObj) {
            SetTarget(context.sensorySystem.closestVisibleObject);
        } else {
            SetTarget(context.sensorySystem.closestObjectFromMemory);
        }*/

        return State.Success;
    }

    private float GetPathLength(Vector3 targetLocation) {
        float totalPathLength = 0;
        NavMeshPath path = new NavMeshPath();
        if (NavMesh.CalculatePath(postion, targetLocation, 1, path)) {
            for (int i = 0; i < path.corners.Length; i++) {
                totalPathLength += Vector3.Distance(path.corners[i], path.corners[i = 1]);
            }
        }
        return totalPathLength;
    }

    private void SetTarget(GameObject newTarget) {
        blackboard.target = newTarget;
        blackboard.moveToPosition = newTarget.transform.position;
    }
}
