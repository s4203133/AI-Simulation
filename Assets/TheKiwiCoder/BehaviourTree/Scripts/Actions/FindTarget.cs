using UnityEngine;
using TheKiwiCoder;

public class FindTarget : ActionNode
{
    public float radius;
    public LayerMask layers;
    private GameObject target;

    protected override void OnStart() {
        context.aiAgent.stats.currentAction = actionName;
        blackboard.nodeStack.PushNode(this);
    }

    protected override void OnStop() {
        blackboard.nodeStack.PopNode(this);
    }

    protected override State OnUpdate() {
        target = GetTarget();
        if(target == null) {
            return State.Failure;
        }
        blackboard.moveToPosition = target.transform.position;
        blackboard.target = target;
        return State.Success;
    }

    GameObject GetTarget() {
        Collider[] nearbyGO = Physics.OverlapSphere(context.transform.position, radius, layers);
        GameObject returnObject;

        if(nearbyGO.Length == 0) {
            return null;
        } else if(nearbyGO.Length == 1) {
            returnObject = nearbyGO[0].gameObject;
        } else {
            returnObject = GetClosestTarget(nearbyGO);
        }

        return returnObject;
    }

    GameObject GetClosestTarget(Collider[] objects) {
        GameObject closestObject = null;
        float closestDistance = float.MaxValue;

        for(int i = 0; i < objects.Length; i++) {
            float dist = Vector3.SqrMagnitude(objects[i].transform.position - context.transform.position);
            if(dist < closestDistance) {
                closestDistance = dist;
                closestObject = objects[i].gameObject;
            }
        }

        return closestObject;
    }
}
